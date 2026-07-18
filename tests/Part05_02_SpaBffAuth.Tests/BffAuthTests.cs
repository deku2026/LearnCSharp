using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Part05_02_SpaBffAuth.Tests;

public sealed class BffAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BffAuthTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private HttpClient CreateClient() => _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        HandleCookies = true
    });

    [Fact]
    public async Task Education_endpoint_lists_storage_risks()
    {
        var client = CreateClient();
        var json = await client.GetFromJsonAsync<JsonElement>("/api/education/token-storage");
        Assert.True(json.GetProperty("storages").GetArrayLength() >= 4);
        Assert.True(json.GetProperty("bffBenefits").GetArrayLength() >= 3);
    }

    [Fact]
    public async Task Pkce_token_endpoint_requires_verifier()
    {
        var client = CreateClient();
        var bad = await client.PostAsJsonAsync("/idp/token", new { code = "abc", codeVerifier = "" });
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);

        var ok = await client.PostAsJsonAsync("/idp/token", new { code = "abc", codeVerifier = "verifier-123" });
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
    }

    [Fact]
    public async Task Bff_me_without_login_is_401()
    {
        var client = CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/bff/me")).StatusCode);
    }

    [Fact]
    public async Task Bff_mutating_without_csrf_is_403()
    {
        var client = CreateClient();
        // login also requires CSRF by middleware for /bff/* posts except we excluded /bff/login
        var login = await client.PostAsJsonAsync("/bff/login", new { userName = "student", password = "campus123" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        // orders is GET - use logout POST without CSRF header
        using var req = new HttpRequestMessage(HttpMethod.Post, "/bff/logout");
        // cookie is present, but no X-CSRF
        var res = await client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task Bff_login_then_me_and_orders_with_csrf()
    {
        var client = CreateClient();
        var login = await client.PostAsJsonAsync("/bff/login", new { userName = "student", password = "campus123" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var meReq = new HttpRequestMessage(HttpMethod.Get, "/bff/me");
        var me = await client.SendAsync(meReq);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
        var meBody = await me.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("student", meBody.GetProperty("name").GetString());
        Assert.True(meBody.GetProperty("hasServerAccessToken").GetBoolean());

        using var ordersReq = new HttpRequestMessage(HttpMethod.Get, "/bff/api/orders");
        var orders = await client.SendAsync(ordersReq);
        Assert.Equal(HttpStatusCode.OK, orders.StatusCode);

        using var logout = new HttpRequestMessage(HttpMethod.Post, "/bff/logout");
        logout.Headers.Add("X-CSRF", "1");
        Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(logout)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/bff/me")).StatusCode);
    }

    [Fact]
    public async Task Insecure_spa_login_returns_token_to_browser()
    {
        var client = CreateClient();
        var res = await client.PostAsJsonAsync("/spa/insecure-demo/login", new { userName = "x", password = "campus123" });
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrEmpty(body.GetProperty("access_token").GetString()));
        Assert.Contains("XSS", body.GetProperty("warning").GetString()!, StringComparison.OrdinalIgnoreCase);
    }
}
