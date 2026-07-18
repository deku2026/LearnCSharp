using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Step09_IntegrationTesting.Data;
using Testcontainers.PostgreSql;
using Xunit;

namespace Step09_IntegrationTesting.Tests;

/// <summary>
/// Real HTTP + real PostgreSQL (Testcontainers). Not EF InMemory.
/// Also validates against local docker when TEST_USE_LOCAL_PG=1.
/// </summary>
public sealed class CatalogIntegrationTests : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        string connectionString;
        if (string.Equals(Environment.GetEnvironmentVariable("TEST_USE_LOCAL_PG"), "1", StringComparison.Ordinal))
        {
            connectionString = "Host=localhost;Port=5432;Database=lab_step09_tests;Username=dotnet;Password=dotnet_dev";
        }
        else
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:18.4-alpine")
                .WithDatabase("lab_step09")
                .WithUsername("dotnet")
                .WithPassword("dotnet_dev")
                .Build();
            await _container.StartAsync();
            connectionString = _container.GetConnectionString();
        }

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:Default", connectionString);
        });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }

    [Fact]
    public async Task Products_list_returns_seeded_items()
    {
        var list = await _client.GetFromJsonAsync<JsonElement[]>("/api/products");
        Assert.NotNull(list);
        Assert.True(list!.Length >= 2);
    }

    [Fact]
    public async Task Create_product_requires_auth()
    {
        var res = await _client.PostAsJsonAsync("/api/products", new { sku = "X", name = "Y", price = 1 });
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Create_product_with_test_user_persists()
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/products");
        req.Headers.Add("X-Test-User", "admin");
        req.Content = JsonContent.Create(new { sku = "PEN-01", name = "中性笔", price = 3.5 });
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var list = await _client.GetFromJsonAsync<JsonElement[]>("/api/products");
        Assert.Contains(list!, p => p.GetProperty("sku").GetString() == "PEN-01");
    }

    [Fact]
    public async Task Validation_failure_on_bad_price()
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/products");
        req.Headers.Add("X-Test-User", "admin");
        req.Content = JsonContent.Create(new { sku = "BAD", name = "x", price = 0 });
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Secure_ping_with_test_auth()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/api/secure/ping");
        req.Headers.Add("X-Test-User", "admin");
        Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(req)).StatusCode);
    }
}
