using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Part10_Aspire.Tests;

public sealed class ServiceDefaultsTests : IClassFixture<WebApplicationFactory<Part10_Aspire.Api.AssemblyMarker>>
{
    private readonly HttpClient _client;

    public ServiceDefaultsTests(WebApplicationFactory<Part10_Aspire.Api.AssemblyMarker> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Root_describes_lab()
    {
        var json = await _client.GetFromJsonAsync<JsonElement>("/");
        Assert.Contains("Service Defaults", json.GetProperty("lab").GetString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Live_and_ready_health()
    {
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/health/live")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/health/ready")).StatusCode);
    }

    [Fact]
    public async Task Students_ping()
    {
        var res = await _client.GetAsync("/api/students/ping");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
