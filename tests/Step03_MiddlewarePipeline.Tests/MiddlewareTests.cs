using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Step03_MiddlewarePipeline.Tests;

public sealed class MiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MiddlewareTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Public_path_works_without_api_key()
    {
        var res = await _client.GetAsync("/public/hello");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Secure_path_requires_api_key_short_circuit()
    {
        var res = await _client.GetAsync("/secure/ping");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        Assert.Contains("X-Api-Key", await res.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Secure_path_with_key_returns_ok_and_timing_headers()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/secure/ping");
        req.Headers.Add("X-Api-Key", "lab");
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.True(res.Headers.Contains("X-Elapsed-Ms") || res.Headers.Contains("X-Request-Id"));
        Assert.True(res.Headers.Contains("X-On-Starting"));
    }

    [Fact]
    public async Task Boom_is_handled_by_global_exception_middleware()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/boom");
        req.Headers.Add("X-Api-Key", "lab");
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
        var body = await res.Content.ReadAsStringAsync();
        Assert.Contains("boom", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UseWhen_branch_sets_header()
    {
        var res = await _client.GetAsync("/branch/info");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.True(res.Headers.TryGetValues("X-Branch", out var values));
        Assert.Contains("use-when", values!);
    }

    [Fact]
    public async Task MapShortCircuit_robots_returns_404()
    {
        var res = await _client.GetAsync("/robots.txt");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}
