using System.Collections.Concurrent;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<TenantStore>();
var app = builder.Build();
app.Use(async (ctx, next) => {
  var tenant = ctx.Request.Headers["X-Tenant"].FirstOrDefault() ?? "public";
  ctx.Items["Tenant"] = tenant;
  await next();
});
app.MapGet("/", () => Results.Ok(new { lab="Part04_03 MultiTenancy" }));
app.MapGet("/api/items", (HttpContext http, TenantStore store) => {
  var t = (string)http.Items["Tenant"]!;
  return Results.Ok(store.List(t));
});
app.MapPost("/api/items", (HttpContext http, ItemDto dto, TenantStore store) => {
  var t = (string)http.Items["Tenant"]!;
  store.Add(t, dto.Name);
  return Results.Created($"/api/items", new { tenant=t, dto.Name });
});
app.MapGet("/health", () => Results.Ok(new { status="Healthy" }));
app.Run();
public partial class Program;
public record ItemDto(string Name);
public sealed class TenantStore {
  private readonly ConcurrentDictionary<string, List<string>> _d = new();
  public IReadOnlyList<string> List(string t)=>_d.GetOrAdd(t,_=>new());
  public void Add(string t,string name){ var list=_d.GetOrAdd(t,_=>new()); lock(list) list.Add(name); }
}
