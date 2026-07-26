using LearnBlazor.Blazor_Part12_Api.Models;
using LearnBlazor.Blazor_Part12_Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddSingleton<ItemStore>();

WebApplication app = builder.Build();

app.UseCors();

app.MapGet("/api/items", (ItemStore store) => store.GetAll());
app.MapGet("/api/items/{id:int}", (ItemStore store, int id) =>
{
    Item? item = store.Get(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});
app.MapPost("/api/items", (ItemStore store, Item body) => Results.Created($"/api/items/{body.Id}", store.Add(body.Title)));

app.MapGet("/api/items/error", () => Results.Json(new { error = "simulated server error" }, statusCode: 500));
app.MapGet("/api/items/slow", async () =>
{
    await Task.Delay(TimeSpan.FromSeconds(2));
    return Results.Ok(new[] { new Item(1, "slow", false) });
});

app.MapGet("/healthz", () => Results.Ok("ok"));

app.Run();
