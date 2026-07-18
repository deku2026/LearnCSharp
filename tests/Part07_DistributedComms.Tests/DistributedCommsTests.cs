using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Part07.Inventory.Grpc;
using Xunit;

namespace Part07_DistributedComms.Tests;

public sealed class DistributedCommsTests : IAsyncLifetime
{
    private WebApplicationFactory<Part07.Inventory.AssemblyMarker> _inventory = null!;
    private WebApplicationFactory<Part07.Catalog.AssemblyMarker> _catalog = null!;
    private WebApplicationFactory<Part07.Gateway.AssemblyMarker> _gateway = null!;

    public Task InitializeAsync()
    {
        _inventory = new WebApplicationFactory<Part07.Inventory.AssemblyMarker>();
        var invHandler = _inventory.Server.CreateHandler();

        // Route Catalog's HttpClient/gRPC traffic into Inventory TestServer without re-registering gRPC clients
        _catalog = new WebApplicationFactory<Part07.Catalog.AssemblyMarker>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.ConfigureAll<HttpClientFactoryOptions>(options =>
                {
                    options.HttpMessageHandlerBuilderActions.Add(b =>
                    {
                        b.PrimaryHandler = invHandler;
                    });
                });
            });
        });

        _gateway = new WebApplicationFactory<Part07.Gateway.AssemblyMarker>();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _catalog.DisposeAsync();
        await _inventory.DisposeAsync();
        await _gateway.DisposeAsync();
    }

    [Fact]
    public async Task Inventory_rest_stock()
    {
        var client = _inventory.CreateClient();
        var res = await client.GetAsync("/api/stock/CUP-001");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Inventory_grpc_get_stock()
    {
        var channel = GrpcChannel.ForAddress("http://inventory", new GrpcChannelOptions
        {
            HttpHandler = _inventory.Server.CreateHandler()
        });
        var grpc = new InventoryService.InventoryServiceClient(channel);
        var reply = await grpc.GetStockAsync(new StockRequest { Sku = "CUP-001" });
        Assert.True(reply.Quantity > 0);
    }

    [Fact]
    public async Task Catalog_orders_via_grpc_reserve()
    {
        var client = _catalog.CreateClient();
        var res = await client.PostAsJsonAsync("/api/orders", new
        {
            studentNumber = "2024001001",
            sku = "CUP-001",
            quantity = 2
        });
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task Catalog_availability_grpc()
    {
        var client = _catalog.CreateClient();
        var res = await client.GetAsync("/api/products/CUP-001/availability");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("grpc", json.GetProperty("via").GetString());
    }

    [Fact]
    public async Task Inventory_health_endpoints()
    {
        var client = _inventory.CreateClient();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/live")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/ready")).StatusCode);
    }

    [Fact]
    public async Task Gateway_requires_auth_for_proxy_and_issues_token()
    {
        var client = _gateway.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/catalog/products/x/availability")).StatusCode);

        var tokRes = await client.PostAsJsonAsync("/auth/token", new { userName = "zhang", password = "campus123" });
        Assert.Equal(HttpStatusCode.OK, tokRes.StatusCode);
        var token = (await tokRes.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("access_token").GetString();
        Assert.False(string.IsNullOrEmpty(token));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var proxied = await client.GetAsync("/api/inventory/stock/CUP-001");
        Assert.NotEqual(HttpStatusCode.Unauthorized, proxied.StatusCode);
    }

    [Fact]
    public async Task Insufficient_stock_conflict()
    {
        var client = _catalog.CreateClient();
        var res = await client.PostAsJsonAsync("/api/orders", new
        {
            studentNumber = "2024001001",
            sku = "BK-LA",
            quantity = 10_000
        });
        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
    }
}
