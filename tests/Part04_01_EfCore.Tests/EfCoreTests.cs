using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Xunit;

namespace Part04_01_EfCore.Tests;

public class EfCoreTests : IAsyncLifetime
{
    private PostgreSqlContainer? _pg;
    private WebApplicationFactory<Program> _f = null!;
    private HttpClient _c = null!;

    public async Task InitializeAsync()
    {
        string cs;
        if (string.Equals(Environment.GetEnvironmentVariable("TEST_USE_LOCAL_PG"), "1", StringComparison.Ordinal))
        {
            cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                 ?? "Host=localhost;Port=5432;Database=lab_part04_ef;Username=dotnet;Password=dotnet_dev";
        }
        else
        {
            _pg = new PostgreSqlBuilder()
                .WithImage("postgres:18.4-alpine")
                .WithDatabase("lab_part04_ef")
                .WithUsername("dotnet")
                .WithPassword("dotnet_dev")
                .Build();
            await _pg.StartAsync();
            cs = _pg.GetConnectionString();
        }

        _f = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("ConnectionStrings:Default", cs));
        _c = _f.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _c.Dispose();
        await _f.DisposeAsync();
        if (_pg is not null)
        {
            await _pg.DisposeAsync();
        }
    }

    [Fact]
    public async Task Products_projected()
    {
        var list = await _c.GetFromJsonAsync<JsonElement[]>("/api/products");
        Assert.True(list!.Length >= 3);
        Assert.True(list[0].TryGetProperty("category", out _));
    }

    [Fact]
    public async Task Students_seeded()
    {
        var list = await _c.GetFromJsonAsync<JsonElement[]>("/api/students");
        Assert.Contains(list!, s => s.GetProperty("studentNumber").GetString() == "2024001001");
    }

    [Fact]
    public async Task Good_categories_no_throw()
    {
        var res = await _c.GetAsync("/api/categories/good");
        res.EnsureSuccessStatusCode();
    }
}
