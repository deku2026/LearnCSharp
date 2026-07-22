using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Campus.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Step02_DIConfigOptions.Tests;

public sealed class DiOptionsTests
{
    [Fact]
    public async Task Di_demo_shows_lifetime_rules()
    {
        await using CampusWebApplicationFactory<Program> factory = new CampusWebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        JsonElement first = await client.GetFromJsonAsync<JsonElement>("/di-demo");
        JsonElement second = await client.GetFromJsonAsync<JsonElement>("/di-demo");
        Assert.False(first.GetProperty("transient").GetProperty("same").GetBoolean());
        Assert.True(first.GetProperty("scoped").GetProperty("same").GetBoolean());
        Assert.NotEqual(
            first.GetProperty("scoped").GetProperty("first").GetGuid(),
            second.GetProperty("scoped").GetProperty("first").GetGuid());
        Assert.Equal(
            first.GetProperty("singleton").GetGuid(),
            second.GetProperty("singleton").GetGuid());
    }

    [Fact]
    public async Task Options_endpoint_returns_bound_values()
    {
        await using CampusWebApplicationFactory<Program> factory = new CampusWebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/options");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonElement json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Step02-DI", json.GetProperty("options").GetProperty("labName").GetString());
    }

    [Fact]
    public void Invalid_options_fail_on_start()
    {
        using CampusWebApplicationFactory<Program> factory = new CampusWebApplicationFactory<Program>()
            .WithSetting("CampusLab:LabName", "invalid");

        Exception ex = Assert.ThrowsAny<Exception>(() =>
        {
            using IServiceScope scope = factory.Services.CreateScope();
            _ = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<Step02_DIConfigOptions.CampusLabOptions>>().Value;
        });

        Assert.NotNull(ex);
    }

    [Fact]
    public async Task Scrutor_decorator_wraps_inner_counter()
    {
        await using CampusWebApplicationFactory<Program> factory = new CampusWebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        JsonElement json = await client.GetFromJsonAsync<JsonElement>("/counter");
        Assert.True(json.GetProperty("decorated").GetBoolean());
        Assert.True(json.GetProperty("value").GetInt32() >= 1);
    }

    [Fact]
    public async Task Enumerable_resolves_all_notifier_implementations()
    {
        await using CampusWebApplicationFactory<Program> factory = new CampusWebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        JsonElement json = await client.GetFromJsonAsync<JsonElement>("/notifiers");
        Assert.Equal(2, json.GetProperty("count").GetInt32());
    }

    [Theory]
    [InlineData("email", "EmailChannel")]
    [InlineData("sms", "SmsChannel")]
    public async Task Keyed_service_resolves_by_key(string key, string expectedType)
    {
        await using CampusWebApplicationFactory<Program> factory = new CampusWebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        JsonElement json = await client.GetFromJsonAsync<JsonElement>($"/channels/{key}");
        Assert.Equal(expectedType, json.GetProperty("type").GetString());
    }

    [Fact]
    public async Task Captive_demo_resolves_scoped_via_factory()
    {
        await using CampusWebApplicationFactory<Program> factory = new CampusWebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        JsonElement json = await client.GetFromJsonAsync<JsonElement>("/captive-demo");
        JsonElement ids = json.GetProperty("resolvedIds");
        Assert.Equal(2, ids.GetArrayLength());
        Assert.NotEqual(ids[0].GetGuid(), ids[1].GetGuid());
    }

    [Fact]
    public void Captive_dependency_is_rejected_by_scope_validation()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddScoped<Step02_DIConfigOptions.ScopedMarker>();
        services.AddSingleton<Step02_DIConfigOptions.CaptiveSingleton>();

        AggregateException exception = Assert.Throws<AggregateException>(() =>
            services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true,
            }));
        Assert.Contains(
            "Cannot consume scoped service",
            exception.ToString(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task Options_watcher_reports_current_value()
    {
        await using CampusWebApplicationFactory<Program> factory = new CampusWebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        JsonElement json = await client.GetFromJsonAsync<JsonElement>("/options/watcher");
        Assert.False(string.IsNullOrWhiteSpace(json.GetProperty("lastSeenLabName").GetString()));
    }
}
