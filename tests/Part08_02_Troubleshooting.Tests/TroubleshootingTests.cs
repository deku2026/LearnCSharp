using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Part08_02_Troubleshooting.Tests;

public sealed class TroubleshootingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TroubleshootingTests(WebApplicationFactory<Program> factory)
    {
        // Point downstream HttpClient back into the same TestServer
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // handler filled after factory created — use custom factory below
            });
        });
    }

    private HttpClient CreateWiredClient(out WebApplicationFactory<Program> wiredFactory)
    {
        WebApplicationFactory<Program>? local = null;
        local = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddHttpClient("downstream")
                    .ConfigurePrimaryHttpMessageHandler(() => local!.Server.CreateHandler())
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://troubleshoot/"));
            });
        });
        wiredFactory = local;
        return local.CreateClient();
    }

    [Fact]
    public async Task Inject_fail_makes_checkout_503()
    {
        var client = CreateWiredClient(out var factory);
        await using (factory)
        {
            // Resilience handler retries — inject enough failures to exhaust retries
            var fault = await client.PostAsJsonAsync("/diag/fault", new { delayMs = 0, failCount = 20 });
            Assert.Equal(HttpStatusCode.OK, fault.StatusCode);

            var checkout = await client.GetAsync("/api/orders/checkout");
            Assert.Equal(HttpStatusCode.ServiceUnavailable, checkout.StatusCode);
        }
    }

    [Fact]
    public async Task Inject_delay_increases_metrics_max()
    {
        var client = CreateWiredClient(out var factory);
        await using (factory)
        {
            await client.PostAsJsonAsync("/diag/fault", new { delayMs = 50, failCount = 0 });
            var res = await client.GetAsync("/api/orders/checkout");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var metrics = await client.GetFromJsonAsync<JsonElement>("/diag/metrics");
            Assert.True(metrics.GetProperty("maxMs").GetDouble() >= 40);
        }
    }

    [Fact]
    public async Task Correlation_id_echoed_on_checkout()
    {
        var client = CreateWiredClient(out var factory);
        await using (factory)
        {
            await client.PostAsJsonAsync("/diag/fault", new { delayMs = 0, failCount = 0 });
            using var req = new HttpRequestMessage(HttpMethod.Get, "/api/orders/checkout");
            req.Headers.Add("X-Correlation-Id", "troubleshoot-cid");
            var res = await client.SendAsync(req);
            Assert.True(res.Headers.TryGetValues("X-Correlation-Id", out var values));
            Assert.Contains("troubleshoot-cid", values!);
        }
    }

    [Fact]
    public async Task Metrics_endpoint_has_fingerprint_hints()
    {
        var client = _factory.CreateClient();
        var metrics = await client.GetFromJsonAsync<JsonElement>("/diag/metrics");
        Assert.True(metrics.TryGetProperty("fingerprintHints", out _));
    }
}
