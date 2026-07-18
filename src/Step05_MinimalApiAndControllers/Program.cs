using Step05_MinimalApiAndControllers.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ProductStore>();
builder.Services.AddSingleton<StudentStore>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

var products = app.MapGroup("/api/products").WithTags("Products");

products.MapGet("/", (ProductStore store) => TypedResults.Ok(store.GetAll()));

products.MapGet("/{id:guid}", (Guid id, ProductStore store) =>
    store.TryGet(id, out var p) ? TypedResults.Ok(p) : Results.NotFound());

products.MapPost("/", (CreateProductRequest req, ProductStore store) =>
{
    var product = store.Add(req.Name, req.Price, req.Sku);
    return TypedResults.Created($"/api/products/{product.Id}", product);
}).AddEndpointFilter(async (ctx, next) =>
{
    if (ctx.Arguments.OfType<CreateProductRequest>().FirstOrDefault() is { Name: null or "" })
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["name"] = ["Name is required"]
        });
    }

    return await next(ctx);
});

products.MapPut("/{id:guid}", (Guid id, CreateProductRequest req, ProductStore store) =>
    store.TryUpdate(id, req.Name, req.Price, req.Sku, out var p)
        ? TypedResults.Ok(p)
        : Results.NotFound());

products.MapDelete("/{id:guid}", (Guid id, ProductStore store) =>
    store.Remove(id) ? TypedResults.NoContent() : Results.NotFound());

app.MapGet("/api/stream/ticks", async (HttpContext http, CancellationToken ct) =>
{
    http.Response.Headers.ContentType = "text/event-stream";
    for (var i = 0; i < 3 && !ct.IsCancellationRequested; i++)
    {
        await http.Response.WriteAsync($"data: tick-{i}\n\n", ct);
        await http.Response.Body.FlushAsync(ct);
        await Task.Delay(20, ct);
    }
});

app.MapGet("/", () => Results.Ok(new
{
    lab = "Step05 Minimal API (main) + Controllers (compare)",
    products = "/api/products",
    students = "/api/students"
}));

app.Run();

public partial class Program;
