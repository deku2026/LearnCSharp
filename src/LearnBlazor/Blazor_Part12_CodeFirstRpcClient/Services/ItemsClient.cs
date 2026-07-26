using System.Net;
using System.Text.Json;
using LearnBlazor.Blazor_Part12_CodeFirstRpcClient.Contracts;
using LearnBlazor.Blazor_Part12_CodeFirstRpcClient.Models;

namespace LearnBlazor.Blazor_Part12_CodeFirstRpcClient.Services;

public sealed class ItemsClient
{
    private readonly HttpClient _http;
    private readonly ILogger<ItemsClient> _logger;

    public ItemsClient(HttpClient http, ILogger<ItemsClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<(IReadOnlyList<Item>? Items, ApiError Error)> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            HttpResponseMessage resp = await _http.GetAsync("/api/items", ct);
            if (!resp.IsSuccessStatusCode)
            {
                return (null, MapStatus(resp.StatusCode));
            }
            await using Stream stream = await resp.Content.ReadAsStreamAsync(ct);
            IReadOnlyList<Item>? items = await JsonSerializer.DeserializeAsync<List<Item>>(stream, cancellationToken: ct);
            return (items ?? new List<Item>(), ApiError.None);
        }
        catch (TaskCanceledException) when (ct.IsCancellationRequested)
        {
            return (null, ApiError.Cancelled);
        }
        catch (TaskCanceledException)
        {
            return (null, ApiError.Timeout);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "network failure");
            return (null, ApiError.Network);
        }
    }

    private static ApiError MapStatus(HttpStatusCode code) => code switch
    {
        HttpStatusCode.NotFound => ApiError.NotFound,
        HttpStatusCode.RequestTimeout => ApiError.Timeout,
        _ => (int)code >= 500 ? ApiError.Server : ApiError.Network,
    };
}
