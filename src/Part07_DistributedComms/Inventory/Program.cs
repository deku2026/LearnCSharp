using System.Collections.Concurrent;
using Grpc.Core;
using Part07.Inventory.Grpc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddSingleton<StockStore>();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck("stock", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("in-memory stock"), tags: ["ready"]);

var app = builder.Build();

app.MapGrpcService<InventoryGrpcService>();
app.MapGet("/", () => Results.Ok(new { service = "inventory", protocol = "gRPC + health" }));
app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "Healthy", dependency = "stock-store" }));
// REST mirror for gateway demos
app.MapGet("/api/stock/{sku}", (string sku, StockStore store) =>
    store.TryGet(sku, out var q) ? Results.Ok(new { sku, quantity = q }) : Results.NotFound());

app.Run();

public partial class Program;

namespace Part07.Inventory
{
    public sealed class AssemblyMarker;
}

public sealed class StockStore
{
    private readonly ConcurrentDictionary<string, int> _stock = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CUP-001"] = 100,
        ["BK-LA"] = 20
    };

    // Fault injection for resilience demos
    public int FailNextCalls { get; set; }

    public bool TryGet(string sku, out int qty)
    {
        if (FailNextCalls > 0)
        {
            FailNextCalls--;
            throw new InvalidOperationException("injected inventory failure");
        }

        return _stock.TryGetValue(sku, out qty);
    }

    public bool TryReserve(string sku, int quantity, out int remaining, out string message)
    {
        remaining = 0;
        message = "";
        if (FailNextCalls > 0)
        {
            FailNextCalls--;
            message = "injected failure";
            return false;
        }

        while (true)
        {
            if (!_stock.TryGetValue(sku, out var current))
            {
                message = "unknown sku";
                return false;
            }

            if (current < quantity)
            {
                remaining = current;
                message = "insufficient stock";
                return false;
            }

            if (_stock.TryUpdate(sku, current - quantity, current))
            {
                remaining = current - quantity;
                message = "ok";
                return true;
            }
        }
    }
}

public sealed class InventoryGrpcService(StockStore store) : InventoryService.InventoryServiceBase
{
    public override Task<StockReply> GetStock(StockRequest request, ServerCallContext context)
    {
        if (!store.TryGet(request.Sku, out var qty))
        {
            throw new RpcException(new Status(StatusCode.NotFound, "sku not found"));
        }

        return Task.FromResult(new StockReply { Sku = request.Sku, Quantity = qty });
    }

    public override Task<ReserveReply> Reserve(ReserveRequest request, ServerCallContext context)
    {
        var ok = store.TryReserve(request.Sku, request.Quantity, out var remaining, out var message);
        return Task.FromResult(new ReserveReply { Ok = ok, Message = message, Remaining = remaining });
    }
}
