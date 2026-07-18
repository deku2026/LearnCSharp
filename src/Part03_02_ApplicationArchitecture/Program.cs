using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CatalogModule>();
builder.Services.AddSingleton<OrdersModule>();
builder.Services.AddSingleton<StudentsModule>();
// Module communication via public interfaces only (no DDD ceremony)
builder.Services.AddSingleton<ICatalogQueries>(sp => sp.GetRequiredService<CatalogModule>());
builder.Services.AddSingleton<IStudentQueries>(sp => sp.GetRequiredService<StudentsModule>());

var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { lab = "Part03_02 Modular monolith skeleton", modules = new[]{"Catalog","Orders","Students"} }));
app.MapGet("/catalog/products", (CatalogModule m) => m.List());
app.MapGet("/students", (StudentsModule m) => m.List());
app.MapPost("/orders", (CreateOrder req, OrdersModule orders, ICatalogQueries catalog, IStudentQueries students) =>
{
    if (students.Get(req.StudentNumber) is null) return Results.BadRequest(new { error = "unknown student" });
    if (catalog.GetPrice(req.Sku) is not decimal price) return Results.BadRequest(new { error = "unknown sku" });
    var order = orders.Place(req.StudentNumber, req.Sku, req.Qty, price);
    return Results.Created($"/orders/{order.Id}", order);
});
app.MapGet("/orders/{id:guid}", (Guid id, OrdersModule m) => m.Get(id) is { } o ? Results.Ok(o) : Results.NotFound());
app.Run();
public partial class Program;
public sealed record CreateOrder(string StudentNumber, string Sku, int Qty);
public sealed record OrderDto(Guid Id, string Student, string Sku, int Qty, decimal Total);
public interface ICatalogQueries { decimal? GetPrice(string sku); }
public interface IStudentQueries { object? Get(string number); }
public sealed class CatalogModule : ICatalogQueries {
  private readonly ConcurrentDictionary<string,(string Name,decimal Price)> _p = new(){ ["CUP-001"]=("马克杯",29.9m),["BK-LA"]=("线性代数",45m)};
  public object List()=>_p.Select(kv=>new{sku=kv.Key,name=kv.Value.Name,price=kv.Value.Price});
  public decimal? GetPrice(string sku)=>_p.TryGetValue(sku,out var p)?p.Price:null;
}
public sealed class StudentsModule : IStudentQueries {
  private readonly ConcurrentDictionary<string,string> _s = new(){ ["2024001001"]="张三",["2024001002"]="李四"};
  public object List()=>_s.Select(kv=>new{number=kv.Key,name=kv.Value});
  public object? Get(string number)=>_s.TryGetValue(number,out var n)? new{number,name=n}:null;
}
public sealed class OrdersModule {
  private readonly ConcurrentDictionary<Guid,OrderDto> _o=new();
  public OrderDto Place(string student,string sku,int qty,decimal price){ var id=Guid.NewGuid(); var o=new OrderDto(id,student,sku,qty,price*qty); _o[id]=o; return o; }
  public OrderDto? Get(Guid id)=>_o.GetValueOrDefault(id);
}
