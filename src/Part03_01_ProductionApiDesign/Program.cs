using System.Collections.Concurrent;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ProductRepo>();
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(o =>
{
    o.GroupNameFormat = "'v'VVV";
    o.SubstituteApiVersionInUrl = true;
});
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();

var v1 = app.NewVersionedApi("Products").MapGroup("/api/v{version:apiVersion}/products")
    .HasApiVersion(1, 0);

// Keyset pagination + ETag + DTO separation
v1.MapGet("/", (ProductRepo repo, [FromQuery] int? afterId, [FromQuery] int pageSize = 10) =>
{
    pageSize = Math.Clamp(pageSize, 1, 50);
    var page = repo.Keyset(afterId, pageSize);
    return Results.Ok(new { items = page.Items, nextAfterId = page.NextAfterId });
});

v1.MapGet("/{id:int}", (int id, ProductRepo repo, HttpContext http) =>
{
    var p = repo.Get(id);
    if (p is null) return Results.NotFound();
    var etag = $"\"{p.Version}\"";
    http.Response.Headers.ETag = etag;
    if (http.Request.Headers.IfNoneMatch == etag) return Results.StatusCode(StatusCodes.Status304NotModified);
    return Results.Ok(new ProductDto(p.Id, p.Sku, p.Name, p.Price));
});

v1.MapPost("/", async (CreateProductDto dto, ProductRepo repo, HttpRequest req) =>
{
    // Idempotency-Key
    if (req.Headers.TryGetValue("Idempotency-Key", out var key) && repo.TryGetIdempotent(key!, out var existing))
        return Results.Ok(existing);
    var created = repo.Add(dto.Sku, dto.Name, dto.Price);
    var result = new ProductDto(created.Id, created.Sku, created.Name, created.Price);
    if (req.Headers.ContainsKey("Idempotency-Key"))
        repo.RememberIdempotent(req.Headers["Idempotency-Key"]!, result);
    return Results.Created($"/api/v1/products/{created.Id}", result);
});

v1.MapPut("/{id:int}", (int id, UpdateProductDto dto, ProductRepo repo, HttpRequest req) =>
{
    var current = repo.Get(id);
    if (current is null) return Results.NotFound();
    var etag = $"\"{current.Version}\"";
    if (req.Headers.IfMatch.Count > 0 && req.Headers.IfMatch != etag)
        return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
    var updated = repo.Update(id, dto.Name, dto.Price);
    return Results.Ok(new ProductDto(updated!.Id, updated.Sku, updated.Name, updated.Price));
});

app.MapGet("/", () => Results.Ok(new { lab = "Part03_01 Production API Design", openapi = "/openapi/v1.json" }));
app.Run();
public partial class Program;

public sealed record ProductDto(int Id, string Sku, string Name, decimal Price);
public sealed record CreateProductDto(string Sku, string Name, decimal Price);
public sealed record UpdateProductDto(string Name, decimal Price);
public sealed class ProductEntity { public int Id; public string Sku=""; public string Name=""; public decimal Price; public int Version; }
public sealed class ProductRepo {
  private int _id; private readonly ConcurrentDictionary<int, ProductEntity> _d=new();
  private readonly ConcurrentDictionary<string, ProductDto> _idem=new();
  public ProductRepo(){ Add("CUP-001","校园马克杯",29.9m); Add("BK-01","算法导论",88m); Add("PEN-01","中性笔",3.5m); }
  public ProductEntity Add(string sku,string name,decimal price){ var e=new ProductEntity{Id=Interlocked.Increment(ref _id),Sku=sku,Name=name,Price=price,Version=1}; _d[e.Id]=e; return e; }
  public ProductEntity? Get(int id)=>_d.GetValueOrDefault(id);
  public ProductEntity? Update(int id,string name,decimal price){ if(!_d.TryGetValue(id,out var e)) return null; e.Name=name; e.Price=price; e.Version++; return e; }
  public (IReadOnlyList<ProductDto> Items,int? NextAfterId) Keyset(int? afterId,int size){
    var q=_d.Values.OrderBy(x=>x.Id).AsEnumerable(); if(afterId is int a) q=q.Where(x=>x.Id>a);
    var take=q.Take(size+1).Select(x=>new ProductDto(x.Id,x.Sku,x.Name,x.Price)).ToList();
    int? next=null; if(take.Count>size){ next=take[size-1].Id; take=take.Take(size).ToList(); }
    return (take,next);
  }
  public bool TryGetIdempotent(string key, out ProductDto dto)=>_idem.TryGetValue(key,out dto!);
  public void RememberIdempotent(string key, ProductDto dto)=>_idem[key]=dto;
}
