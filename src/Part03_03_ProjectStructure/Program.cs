// Demonstrates composition root only in Api host (see folders Domain/Application/Infrastructure/Contracts)
using Part03_03_ProjectStructure.Application;
using Part03_03_ProjectStructure.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { lab="Part03_03 Project structure", layers=new[]{"Api","Application","Domain","Infrastructure","Contracts"} }));
app.MapGet("/products", (IProductService svc) => svc.List());
app.Run();
public partial class Program;
