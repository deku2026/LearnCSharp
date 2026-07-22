using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Part05_2_SpaAuth;

public sealed class BffAccessTokenService(
    IHttpClientFactory httpClientFactory,
    IOptions<BffOptions> options,
    TimeProvider timeProvider,
    ILogger<BffAccessTokenService> logger)
{
    public const string HttpContextItemName = "Campus.Bff.AccessToken";
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _sessionLocks = new();
    private readonly BffOptions _options = options.Value;

    public async Task<string?> GetValidAccessTokenAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        string sessionKey = httpContext.Request.Cookies.FirstOrDefault(
            cookie => cookie.Key.Contains("campus-bff", StringComparison.Ordinal)).Value;
        if (string.IsNullOrWhiteSpace(sessionKey))
        {
            return null;
        }

        SemaphoreSlim gate = _sessionLocks.GetOrAdd(sessionKey, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken);
        try
        {
            AuthenticateResult authentication = await httpContext.AuthenticateAsync(BffSchemes.Cookie);
            if (!authentication.Succeeded ||
                authentication.Principal is null ||
                authentication.Properties is null)
            {
                return null;
            }

            AuthenticationProperties properties = authentication.Properties;
            string? accessToken = properties.GetTokenValue("access_token");
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            if (TryGetExpiry(properties, out DateTimeOffset expiresAt) &&
                expiresAt > timeProvider.GetUtcNow().AddSeconds(_options.RefreshBeforeExpirySeconds))
            {
                return accessToken;
            }

            return await RefreshAsync(
                httpContext,
                authentication.Principal,
                properties,
                cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task RevokeCurrentSessionAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        AuthenticateResult authentication = await httpContext.AuthenticateAsync(BffSchemes.Cookie);
        string? refreshToken = authentication.Properties?.GetTokenValue("refresh_token");
        if (!string.IsNullOrWhiteSpace(refreshToken) &&
            !string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            using HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.Authority.TrimEnd('/')}/protocol/openid-connect/revoke")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _options.ClientId,
                    ["client_secret"] = _options.ClientSecret,
                    ["token"] = refreshToken,
                    ["token_type_hint"] = "refresh_token",
                }),
            };

            using HttpResponseMessage response = await httpClientFactory.CreateClient("oidc-backchannel")
                .SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Keycloak token revocation returned status {StatusCode}.",
                    (int)response.StatusCode);
            }
        }

        await httpContext.SignOutAsync(BffSchemes.Cookie);
    }

    private async Task<string?> RefreshAsync(
        HttpContext httpContext,
        ClaimsPrincipal principal,
        AuthenticationProperties properties,
        CancellationToken cancellationToken)
    {
        string? refreshToken = properties.GetTokenValue("refresh_token");
        if (string.IsNullOrWhiteSpace(refreshToken) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            await httpContext.SignOutAsync(BffSchemes.Cookie);
            return null;
        }

        using HttpRequestMessage request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.Authority.TrimEnd('/')}/protocol/openid-connect/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["refresh_token"] = refreshToken,
            }),
        };

        using HttpResponseMessage response = await httpClientFactory.CreateClient("oidc-backchannel")
            .SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "The BFF could not refresh the server-side session; status {StatusCode}.",
                (int)response.StatusCode);
            await httpContext.SignOutAsync(BffSchemes.Cookie);
            return null;
        }

        TokenResponse? refreshed = await response.Content.ReadFromJsonAsync<TokenResponse>(
            cancellationToken);
        if (refreshed is null || string.IsNullOrWhiteSpace(refreshed.AccessToken))
        {
            await httpContext.SignOutAsync(BffSchemes.Cookie);
            return null;
        }

        Dictionary<string, AuthenticationToken> tokens = properties.GetTokens().ToDictionary(token => token.Name, StringComparer.Ordinal);
        tokens["access_token"] = new AuthenticationToken
        {
            Name = "access_token",
            Value = refreshed.AccessToken,
        };
        if (!string.IsNullOrWhiteSpace(refreshed.RefreshToken))
        {
            tokens["refresh_token"] = new AuthenticationToken
            {
                Name = "refresh_token",
                Value = refreshed.RefreshToken,
            };
        }

        tokens["expires_at"] = new AuthenticationToken
        {
            Name = "expires_at",
            Value = timeProvider.GetUtcNow()
                .AddSeconds(refreshed.ExpiresIn)
                .ToString("O", CultureInfo.InvariantCulture),
        };
        properties.StoreTokens(tokens.Values);
        await httpContext.SignInAsync(BffSchemes.Cookie, principal, properties);
        return refreshed.AccessToken;
    }

    private static bool TryGetExpiry(
        AuthenticationProperties properties,
        out DateTimeOffset expiresAt) =>
        DateTimeOffset.TryParse(
            properties.GetTokenValue("expires_at"),
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out expiresAt);

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);
}

public sealed class BffAccessTokenMiddleware(
    RequestDelegate next,
    BffAccessTokenService accessTokens)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string? token = await accessTokens.GetValidAccessTokenAsync(
            context,
            context.RequestAborted);
        if (string.IsNullOrWhiteSpace(token))
        {
            await BffProblemDetails.WriteAsync(
                context.Response,
                StatusCodes.Status401Unauthorized,
                "The BFF session is missing or expired.",
                "bff_session_required",
                context.RequestAborted);
            return;
        }

        context.Items[BffAccessTokenService.HttpContextItemName] = token;
        await next(context);
    }
}
