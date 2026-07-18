using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Step01_HostingAndStartup.Services;
using Xunit;

namespace Step01_HostingAndStartup.Tests;

public sealed class HostingModelTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HostingModelTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Keep tests fast: shorten heartbeat via configuration override
            builder.UseSetting("Heartbeat:IntervalSeconds", "1");
            builder.UseSetting("Greeting", "Hello from TEST host");
        });
    }

    [Fact]
    public async Task Root_returns_environment_and_content_web_roots()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("environment", out var env));
        Assert.False(string.IsNullOrWhiteSpace(env.GetString()));

        Assert.True(root.TryGetProperty("contentRoot", out var contentRoot));
        Assert.False(string.IsNullOrWhiteSpace(contentRoot.GetString()));

        Assert.True(root.TryGetProperty("webRoot", out var webRoot));
        var webRootPath = webRoot.GetString();
        Assert.False(string.IsNullOrWhiteSpace(webRootPath));
        Assert.Contains("wwwroot", webRootPath, StringComparison.OrdinalIgnoreCase);

        Assert.True(root.TryGetProperty("message", out var message));
        Assert.Equal("Hello from TEST host", message.GetString());
    }

    [Fact]
    public async Task HostInfo_exposes_environment_flags_and_greeting()
    {
        var client = _factory.CreateClient();
        var payload = await client.GetFromJsonAsync<JsonElement>("/host-info");

        Assert.True(payload.TryGetProperty("environmentName", out _)
                    || payload.TryGetProperty("EnvironmentName", out _));

        // System.Text.Json default for anonymous types is camelCase
        Assert.Equal("Hello from TEST host", payload.GetProperty("greeting").GetString());
        Assert.True(payload.GetProperty("contentRootPath").GetString()!.Length > 0);
        Assert.Contains("wwwroot", payload.GetProperty("webRootPath").GetString()!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task StudentsPing_returns_campus_sample_payload()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/students/ping");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(doc.RootElement.GetProperty("ok").GetBoolean());
        Assert.Equal("2024001001", doc.RootElement.GetProperty("sample").GetProperty("studentNumber").GetString());
    }

    [Fact]
    public async Task StaticFile_from_web_root_is_served()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/readme.txt");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Web root", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task BackgroundWorker_uses_scope_and_increments_probe()
    {
        // Factory starts the host (and hosted services) when first request is made / client created
        using var client = _factory.CreateClient();
        _ = await client.GetAsync("/host-info");

        var probe = _factory.Services.GetRequiredService<StartupProbe>();

        // Heartbeat interval forced to 1s in fixture — wait for at least one tick
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (probe.Current < 1 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(100);
        }

        Assert.True(probe.Current >= 1, $"Expected at least one heartbeat tick, got {probe.Current}");
    }

    [Fact]
    public void Program_exposes_partial_class_for_web_application_factory()
    {
        // Compile-time + factory construction already prove this; keep an explicit assertion for lab docs.
        Assert.NotNull(typeof(Program));
        Assert.True(typeof(Program).IsClass);
    }
}

/// <summary>
/// Configuration layering: Production environment must not use Development appsettings overlay.
/// </summary>
public sealed class EnvironmentLayeringTests
{
    [Fact]
    public async Task Production_environment_uses_production_greeting_when_not_overridden()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting(WebHostDefaults.EnvironmentKey, Environments.Production);
            });

        var client = factory.CreateClient();
        using var doc = JsonDocument.Parse(await (await client.GetAsync("/")).Content.ReadAsStringAsync());
        var message = doc.RootElement.GetProperty("message").GetString();

        Assert.Equal("Hello from PRODUCTION (appsettings.Production.json)", message);
        Assert.Equal(Environments.Production, doc.RootElement.GetProperty("environment").GetString());
    }

    [Fact]
    public async Task Development_environment_uses_development_greeting()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting(WebHostDefaults.EnvironmentKey, Environments.Development);
            });

        var client = factory.CreateClient();
        using var doc = JsonDocument.Parse(await (await client.GetAsync("/")).Content.ReadAsStringAsync());
        var message = doc.RootElement.GetProperty("message").GetString();

        Assert.Equal("Hello from DEV (appsettings.Development.json)", message);
    }
}
