using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Step02_DiConfigOptions.Services;
using Xunit;

namespace Step02_DiConfigOptions.Tests;

public sealed class DiConfigOptionsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DiConfigOptionsTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Lifetimes_within_request_match_expected_reuse()
    {
        var client = _factory.CreateClient();
        using var doc = JsonDocument.Parse(await (await client.GetAsync("/di/lifetimes")).Content.ReadAsStringAsync());
        var root = doc.RootElement;

        Assert.False(root.GetProperty("transient").GetProperty("same").GetBoolean());
        Assert.True(root.GetProperty("scoped").GetProperty("same").GetBoolean());
        Assert.NotEqual(Guid.Empty, root.GetProperty("singleton").GetGuid());
    }

    [Fact]
    public async Task Singleton_same_across_two_requests()
    {
        var client = _factory.CreateClient();
        var a = JsonDocument.Parse(await (await client.GetAsync("/di/lifetimes")).Content.ReadAsStringAsync())
            .RootElement.GetProperty("singleton").GetGuid();
        var b = JsonDocument.Parse(await (await client.GetAsync("/di/lifetimes")).Content.ReadAsStringAsync())
            .RootElement.GetProperty("singleton").GetGuid();
        Assert.Equal(a, b);
    }

    [Fact]
    public async Task Scoped_differs_across_two_requests()
    {
        var client = _factory.CreateClient();
        var a = JsonDocument.Parse(await (await client.GetAsync("/di/lifetimes")).Content.ReadAsStringAsync())
            .RootElement.GetProperty("scoped").GetProperty("first").GetGuid();
        var b = JsonDocument.Parse(await (await client.GetAsync("/di/lifetimes")).Content.ReadAsStringAsync())
            .RootElement.GetProperty("scoped").GetProperty("first").GetGuid();
        Assert.NotEqual(a, b);
    }

    [Fact]
    public async Task Writers_returns_both_implementations()
    {
        var client = _factory.CreateClient();
        var list = await client.GetFromJsonAsync<JsonElement[]>("/di/writers");
        Assert.NotNull(list);
        Assert.Equal(2, list!.Length);
        var names = list.Select(x => x.GetProperty("name").GetString()).OrderBy(x => x).ToArray();
        Assert.Equal(["console", "file"], names);
    }

    [Theory]
    [InlineData("alipay", "alipay:")]
    [InlineData("wechat", "wechat:")]
    public async Task Keyed_payment_gateway_resolves(string gateway, string prefix)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/di/pay/{gateway}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains(prefix, body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Students_decorator_still_returns_names()
    {
        var client = _factory.CreateClient();
        var names = await client.GetFromJsonAsync<string[]>("/di/students");
        Assert.NotNull(names);
        Assert.Contains("张三", names!);
    }

    [Fact]
    public async Task Options_shop_is_bound_and_valid()
    {
        var client = _factory.CreateClient();
        var shop = await client.GetFromJsonAsync<JsonElement>("/options/shop");
        Assert.Equal("校园小店(开发)", shop.GetProperty("shopName").GetString());
        Assert.Equal(1025, shop.GetProperty("smtpPort").GetInt32());
    }

    [Fact]
    public async Task Development_greeting_uses_environment_layer()
    {
        var client = _factory.CreateClient();
        var payload = await client.GetFromJsonAsync<JsonElement>("/config/greeting");
        Assert.Equal("dev-greeting", payload.GetProperty("greeting").GetString());
    }

    [Fact]
    public void FakeDbContext_is_registered_as_scoped()
    {
        using var scope1 = _factory.Services.CreateScope();
        using var scope2 = _factory.Services.CreateScope();
        var a = scope1.ServiceProvider.GetRequiredService<FakeDbContext>().InstanceId;
        var b = scope1.ServiceProvider.GetRequiredService<FakeDbContext>().InstanceId;
        var c = scope2.ServiceProvider.GetRequiredService<FakeDbContext>().InstanceId;
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }
}

public sealed class OptionsValidateOnStartTests
{
    [Fact]
    public void Invalid_options_fail_host_start()
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("CampusShop:SmtpPort", "0");
            builder.UseSetting("CampusShop:ShopName", "");
            builder.UseSetting("CampusShop:SmtpHost", "");
        });

        var ex = Assert.ThrowsAny<Exception>(() =>
        {
            using var client = factory.CreateClient();
            _ = client.GetAsync("/").GetAwaiter().GetResult();
        });

        Assert.True(
            ex.Message.Contains("CampusShop", StringComparison.OrdinalIgnoreCase)
            || ex.InnerException?.Message.Contains("Options", StringComparison.OrdinalIgnoreCase) == true
            || ex.GetType().Name.Contains("Options", StringComparison.OrdinalIgnoreCase)
            || ex.ToString().Contains("SmtpPort", StringComparison.OrdinalIgnoreCase)
            || ex.ToString().Contains("validation", StringComparison.OrdinalIgnoreCase),
            $"Expected options validation failure, got: {ex}");
    }
}
