using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Step04_RoutingEndpoints.Tests;

public sealed class RoutingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public RoutingTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Student_by_id_constraint_works()
    {
        var res = await _client.GetAsync("/students/1001");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("张三", json.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Student_invalid_id_not_matched()
    {
        var res = await _client.GetAsync("/students/0");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task Student_number_regex_constraint()
    {
        var ok = await _client.GetAsync("/students/by-number/2024001001");
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        var bad = await _client.GetAsync("/students/by-number/abc");
        Assert.Equal(HttpStatusCode.NotFound, bad.StatusCode);
    }

    [Fact]
    public async Task Route_group_and_link_generator()
    {
        var health = await _client.GetAsync("/api/v1/health");
        Assert.Equal(HttpStatusCode.OK, health.StatusCode);

        var links = await _client.GetFromJsonAsync<JsonElement>("/links/health");
        Assert.Contains("/api/v1/health", links.GetProperty("healthUri").GetString());
    }

    [Fact]
    public async Task Default_route_value()
    {
        var res = await _client.GetFromJsonAsync<JsonElement>("/catalog");
        Assert.Equal("books", res.GetProperty("category").GetString());
    }

    [Fact]
    public async Task Catch_all_path()
    {
        var res = await _client.GetFromJsonAsync<JsonElement>("/files/a/b/c.txt");
        Assert.Equal("a/b/c.txt", res.GetProperty("path").GetString());
    }
}
