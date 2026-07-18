using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Step05_MinimalApiAndControllers.Tests;

public sealed class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public ApiTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Products_list_seeded()
    {
        var list = await _client.GetFromJsonAsync<JsonElement[]>("/api/products");
        Assert.NotNull(list);
        Assert.True(list!.Length >= 2);
    }

    [Fact]
    public async Task Products_crud_roundtrip()
    {
        var create = await _client.PostAsJsonAsync("/api/products", new { name = "U盘", price = 59.9, sku = "USB-32" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetGuid();

        var get = await _client.GetAsync($"/api/products/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var del = await _client.DeleteAsync($"/api/products/{id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/products/{id}")).StatusCode);
    }

    [Fact]
    public async Task Products_post_empty_name_filtered()
    {
        var res = await _client.PostAsJsonAsync("/api/products", new { name = "", price = 1, sku = "X" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Students_controller_crud()
    {
        var list = await _client.GetFromJsonAsync<JsonElement[]>("/api/students");
        Assert.True(list!.Length >= 2);

        var create = await _client.PostAsJsonAsync("/api/students", new
        {
            studentNumber = "2024999001",
            fullName = "测试生",
            major = "网络工程"
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var get = await _client.GetAsync("/api/students/2024999001");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task Sse_ticks_stream()
    {
        var body = await _client.GetStringAsync("/api/stream/ticks");
        Assert.Contains("tick-0", body);
        Assert.Contains("tick-2", body);
    }
}
