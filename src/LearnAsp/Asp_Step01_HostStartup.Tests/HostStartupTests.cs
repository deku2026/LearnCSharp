using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Campus.Contracts;
using Campus.Testing;

namespace Step01_HostStartup.Tests;

public sealed class HostStartupTests : IClassFixture<CampusWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HostStartupTests(CampusWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Root_returns_ok()
    {
        HttpResponseMessage response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonElement payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Hello from Development", payload.GetProperty("greeting").GetString());
    }

    [Fact]
    public async Task Env_returns_environment_name()
    {
        EnvInfoDto? info = await _client.GetFromJsonAsync<EnvInfoDto>("/env");
        Assert.NotNull(info);
        Assert.False(string.IsNullOrWhiteSpace(info.EnvironmentName));
        Assert.False(string.IsNullOrWhiteSpace(info.ContentRootPath));
    }

    [Fact]
    public async Task Heartbeat_endpoint_is_reachable()
    {
        HttpResponseMessage response = await _client.GetAsync("/heartbeat-count");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Heartbeat_increments_over_time_and_reports_scoped_id()
    {
        // Allow the hosted service to tick at least once.
        await Task.Delay(1500);

        JsonElement first = await _client.GetFromJsonAsync<JsonElement>("/heartbeat-count");
        long firstCount = first.GetProperty("count").GetInt64();
        bool firstHasScoped = first.GetProperty("lastScopedId").ValueKind == JsonValueKind.String;
        Guid firstScopedId = firstHasScoped ? first.GetProperty("lastScopedId").GetGuid() : Guid.Empty;

        await Task.Delay(2200);

        JsonElement second = await _client.GetFromJsonAsync<JsonElement>("/heartbeat-count");
        long secondCount = second.GetProperty("count").GetInt64();
        bool secondHasScoped = second.GetProperty("lastScopedId").ValueKind == JsonValueKind.String;
        Guid secondScopedId = secondHasScoped ? second.GetProperty("lastScopedId").GetGuid() : Guid.Empty;

        Assert.True(secondCount > firstCount, $"expected second {secondCount} > first {firstCount}");
        // Each tick creates a new scoped TickRecorder via IServiceScopeFactory — ids differ.
        Assert.True(secondHasScoped, "second tick should have a scoped id");
        Assert.True(firstHasScoped, "first tick should have a scoped id");
        Assert.NotEqual(firstScopedId, secondScopedId);
    }

    [Fact]
    public async Task Webroot_endpoint_returns_paths()
    {
        JsonElement info = await _client.GetFromJsonAsync<JsonElement>("/webroot");
        Assert.False(string.IsNullOrWhiteSpace(info.GetProperty("contentRoot").GetString()));
    }
}
