using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Step06_BindingValidationProblemDetails.Tests;

public sealed class ValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public ValidationTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Valid_order_binds_route_query_header_body()
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/orders/2024001001?coupon=CAMPUS10");
        req.Headers.Add("X-Request-Id", "req-1");
        req.Content = JsonContent.Create(new { sku = "BK-LA-01", quantity = 2 });
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("2024001001", body.GetProperty("studentNumber").GetString());
        Assert.Equal("CAMPUS10", body.GetProperty("coupon").GetString());
        Assert.Equal("req-1", body.GetProperty("requestId").GetString());
    }

    [Fact]
    public async Task Invalid_quantity_returns_validation_problem()
    {
        var res = await _client.PostAsJsonAsync("/api/orders/2024001001", new { sku = "BK", quantity = 0 });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        Assert.Contains("quantity", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Cross_field_validation_for_limited_sku()
    {
        var res = await _client.PostAsJsonAsync("/api/orders/2024001001", new { sku = "XX-LIMITED", quantity = 2 });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Crash_returns_problem_details()
    {
        var res = await _client.GetAsync("/api/crash");
        Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
        var body = await res.Content.ReadAsStringAsync();
        Assert.Contains("lab crash", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NotFound_status_code_pages()
    {
        var res = await _client.GetAsync("/api/missing");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}
