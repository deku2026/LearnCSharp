using System.Diagnostics;
using Grpc.Net.ClientFactory;
using Part07.Inventory.Grpc;

var builder = WebApplication.CreateBuilder(args);

var inventoryBase = builder.Configuration["Services:Inventory"] ?? "http://localhost:5702";

builder.Services.AddGrpcClient<InventoryService.InventoryServiceClient>(o =>
{
    o.Address = new Uri(inventoryBase);
}).AddStandardResilienceHandler(); // timeout + retry + circuit breaker (Polly v8 pipeline)

builder.Services.AddHttpClient("inventory-rest", c =>
{
    c.BaseAddress = new Uri(inventoryBase.TrimEnd('/') + "/");
}).AddStandardResilienceHandler();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

app.Use(async (ctx, next) =>
{
    // Propagate correlation id (W3C traceparent is automatic with OTel; here we also set custom)
    if (!ctx.Request.Headers.ContainsKey("X-Correlation-Id"))
    {
        ctx.Request.Headers["X-Correlation-Id"] = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
    }

    ctx.Response.Headers["X-Correlation-Id"] = ctx.Request.Headers["X-Correlation-Id"].ToString();
    await next();
});

app.MapGet("/", () => Results.Ok(new { service = "catalog", calls = "inventory via gRPC + resilient HttpClient" }));
app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "Healthy" }));

app.MapGet("/api/products/{sku}/availability", async (string sku, InventoryService.InventoryServiceClient grpc) =>
{
    try
    {
        var reply = await grpc.GetStockAsync(new StockRequest { Sku = sku });
        return Results.Ok(new { sku = reply.Sku, quantity = reply.Quantity, via = "grpc" });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Inventory gRPC failed", detail: ex.Message, statusCode: 503);
    }
});

app.MapPost("/api/orders", async (PlaceOrderDto dto, InventoryService.InventoryServiceClient grpc) =>
{
    var reserve = await grpc.ReserveAsync(new ReserveRequest
    {
        Sku = dto.Sku,
        Quantity = dto.Quantity,
        OrderId = Guid.NewGuid().ToString("N")
    });
    if (!reserve.Ok)
    {
        return Results.Conflict(new { reserve.Message, reserve.Remaining });
    }

    return Results.Created($"/api/orders/{dto.Sku}", new
    {
        dto.StudentNumber,
        dto.Sku,
        dto.Quantity,
        reserve.Remaining,
        via = "catalog->inventory gRPC"
    });
});

app.MapGet("/api/products/{sku}/availability-rest", async (string sku, IHttpClientFactory http) =>
{
    var client = http.CreateClient("inventory-rest");
    using var res = await client.GetAsync($"api/stock/{sku}");
    var body = await res.Content.ReadAsStringAsync();
    return Results.Content(body, "application/json");
});

app.Run();

public partial class Program;

namespace Part07.Catalog
{
    public sealed class AssemblyMarker;
}

public sealed record PlaceOrderDto(string StudentNumber, string Sku, int Quantity);
