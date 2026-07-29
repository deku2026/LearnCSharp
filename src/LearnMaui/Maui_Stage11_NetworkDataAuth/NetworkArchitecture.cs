using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LearnMaui.Maui_Stage11_NetworkDataAuth;

public enum NetworkOutcome
{
    Success,
    Unauthorized,
    NotFound,
    Validation,
    TransientFailure,
    Canceled,
    Failed,
}

public sealed record NetworkResult<T>(
    NetworkOutcome Outcome,
    T? Value = default,
    string? Message = null,
    HttpStatusCode? StatusCode = null);

public sealed record Article(Guid Id, string Title, DateTimeOffset UpdatedAt);

public sealed record ArticleDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("updatedAt")] DateTimeOffset UpdatedAt);

public sealed record ArticlePageDto(
    [property: JsonPropertyName("items")] IReadOnlyList<ArticleDto> Items,
    [property: JsonPropertyName("nextCursor")] string? NextCursor);

[JsonSerializable(typeof(ArticlePageDto))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip)]
internal sealed partial class ApiJsonContext : JsonSerializerContext;

public sealed record ArticlePage(IReadOnlyList<Article> Items, string? NextCursor);

public interface IArticlesClient
{
    Task<NetworkResult<ArticlePage>> GetPageAsync(
        string? cursor,
        CancellationToken cancellationToken);
}

public sealed class ArticlesClient(HttpClient httpClient) : IArticlesClient
{
    public async Task<NetworkResult<ArticlePage>> GetPageAsync(
        string? cursor,
        CancellationToken cancellationToken)
    {
        string requestUri = cursor is null
            ? "v1/articles"
            : $"v1/articles?cursor={Uri.EscapeDataString(cursor)}";
        try
        {
            using HttpRequestMessage request = new(HttpMethod.Get, requestUri);
            using HttpResponseMessage response =
                await httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new(NetworkOutcome.Unauthorized, StatusCode: response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new(NetworkOutcome.NotFound, StatusCode: response.StatusCode);
            }

            if ((int)response.StatusCode >= 500)
            {
                return new(
                    NetworkOutcome.TransientFailure,
                    Message: "Server is temporarily unavailable.",
                    StatusCode: response.StatusCode);
            }

            response.EnsureSuccessStatusCode();
            await using Stream stream =
                await response.Content.ReadAsStreamAsync(cancellationToken);
            ArticlePageDto? dto = await JsonSerializer.DeserializeAsync(
                stream,
                ApiJsonContext.Default.ArticlePageDto,
                cancellationToken);
            if (dto is null)
            {
                return new(NetworkOutcome.Failed, Message: "Response body was empty.");
            }

            Article[] articles = dto.Items
                .Select(item => new Article(item.Id, item.Title, item.UpdatedAt))
                .ToArray();
            return new(
                NetworkOutcome.Success,
                new ArticlePage(articles, dto.NextCursor),
                StatusCode: response.StatusCode);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return new(NetworkOutcome.Canceled);
        }
        catch (HttpRequestException exception)
        {
            return new(
                NetworkOutcome.TransientFailure,
                Message: exception.Message,
                StatusCode: exception.StatusCode);
        }
        catch (JsonException exception)
        {
            return new(NetworkOutcome.Failed, Message: $"Invalid JSON: {exception.Message}");
        }
    }
}

public sealed record TokenSnapshot(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt)
{
    public bool IsExpiringSoon =>
        ExpiresAt <= DateTimeOffset.UtcNow.AddMinutes(1);
}

public interface ITokenVault
{
    ValueTask<TokenSnapshot?> GetAsync(CancellationToken cancellationToken);

    ValueTask SetAsync(TokenSnapshot? token, CancellationToken cancellationToken);
}

public sealed class MemoryTokenVault : ITokenVault
{
    private TokenSnapshot? _token;

    public ValueTask<TokenSnapshot?> GetAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(_token);
    }

    public ValueTask SetAsync(TokenSnapshot? token, CancellationToken cancellationToken)
    {
        _token = token;
        return ValueTask.CompletedTask;
    }
}

public sealed class TokenRefreshCoordinator(ITokenVault tokenVault)
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public async Task<TokenSnapshot?> GetValidTokenAsync(
        CancellationToken cancellationToken)
    {
        TokenSnapshot? current = await tokenVault.GetAsync(cancellationToken);
        if (current is null || !current.IsExpiringSoon)
        {
            return current;
        }

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            current = await tokenVault.GetAsync(cancellationToken);
            if (current is null || !current.IsExpiringSoon)
            {
                return current;
            }

            // A real implementation posts the refresh token to the issuer.
            TokenSnapshot refreshed = current with
            {
                AccessToken = $"refreshed-{Guid.NewGuid():N}",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
            };
            await tokenVault.SetAsync(refreshed, cancellationToken);
            return refreshed;
        }
        finally
        {
            _refreshLock.Release();
        }
    }
}

public sealed class AuthHeaderHandler(TokenRefreshCoordinator refreshCoordinator)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        TokenSnapshot? token =
            await refreshCoordinator.GetValidTokenAsync(cancellationToken);
        if (token is not null)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

public sealed record PkcePair(string Verifier, string Challenge)
{
    public static PkcePair Create()
    {
        byte[] random = RandomNumberGenerator.GetBytes(32);
        string verifier = Base64Url(random);
        byte[] digest = SHA256.HashData(Encoding.ASCII.GetBytes(verifier));
        return new PkcePair(verifier, Base64Url(digest));
    }

    private static string Base64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

public sealed class StubHttpHandler(
    HttpStatusCode statusCode,
    string json) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        HttpResponseMessage response = new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
            RequestMessage = request,
        };
        return Task.FromResult(response);
    }
}
