using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Step07_AuthNAuthZ.Tests;

public sealed class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public AuthTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    private async Task<string> TokenAsync(string user, string pass)
    {
        var res = await _client.PostAsJsonAsync("/auth/token", new { userName = user, password = pass });
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("access_token").GetString()!;
    }

    [Fact]
    public async Task Me_without_token_is_401()
    {
        var res = await _client.GetAsync("/api/me");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Student_can_access_me_and_student_orders_but_not_admin()
    {
        var token = await TokenAsync("zhangsan", "student123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/me")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/students/me/orders")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await _client.GetAsync("/api/admin/dashboard")).StatusCode);
    }

    [Fact]
    public async Task Admin_can_access_admin_dashboard()
    {
        var token = await TokenAsync("admin", "admin123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/admin/dashboard")).StatusCode);
    }

    [Fact]
    public async Task Bad_password_is_401()
    {
        var res = await _client.PostAsJsonAsync("/auth/token", new { userName = "x", password = "nope" });
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}
