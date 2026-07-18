using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var redis = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6380,abortConnect=false";
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = redis);
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ProductSource>();

var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { lab = "Part04_02 Caching", redis }));

app.MapGet("/api/products/{sku}", async (string sku, IDistributedCache cache, ProductSource src, ILogger<Program> log) =>
{
    var key = $"product:{sku}";
    var cached = await cache.GetStringAsync(key);
    if (cached is not null)
    {
        log.LogInformation("CACHE HIT {Key}", key);
        return Results.Content(cached, "application/json");
    }
    log.LogInformation("CACHE MISS {Key}", key);
    try
    {
        var p = src.Get(sku);
        if (p is null) return Results.NotFound();
        var json = JsonSerializer.Serialize(p);
        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        return Results.Content(json, "application/json");
    }
    catch (Exception ex)
    {
        // Redis failure should degrade: still serve source
        log.LogWarning(ex, "Cache write failed; serving source");
        var p = src.Get(sku);
        return p is null ? Results.NotFound() : Results.Ok(p);
    }
});

app.MapDelete("/api/products/{sku}/cache", async (string sku, IDistributedCache cache) =>
{
    await cache.RemoveAsync($"product:{sku}");
    return Results.NoContent();
});

app.Run();
public partial class Program;
public sealed class ProductSource {
  private readonly ConcurrentDictionary<string,(string Name,decimal Price)> _d = new(StringComparer.OrdinalIgnoreCase){
    ["CUP-001"]=("校园马克杯",29.9m),["BK-LA"]=("线性代数",45m)
  };
  private int _loads; public int Loads => _loads;
  public object? Get(string sku){ Interlocked.Increment(ref _loads); return _d.TryGetValue(sku,out var p)? new { sku, p.Name, p.Price }:null; }
}
