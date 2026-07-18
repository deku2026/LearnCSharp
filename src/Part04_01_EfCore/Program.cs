using Microsoft.EntityFrameworkCore;
using Part04_01_EfCore.Data;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Host=localhost;Port=5432;Database=lab_part04_ef;Username=dotnet;Password=dotnet_dev";
builder.Services.AddDbContext<CampusDb>(o => o.UseNpgsql(cs).LogTo(Console.WriteLine, LogLevel.Information));

var app = builder.Build();

// eShop-style migrate+seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusDb>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Categories.AnyAsync())
    {
        var books = new Category { Name = "教材" };
        var life = new Category { Name = "生活" };
        db.Categories.AddRange(books, life);
        await db.SaveChangesAsync();
        db.Products.AddRange(
            new Product { Sku = "BK-ALG", Name = "算法导论", Price = 88, Stock = 20, CategoryId = books.Id },
            new Product { Sku = "BK-LA", Name = "线性代数", Price = 45, Stock = 30, CategoryId = books.Id },
            new Product { Sku = "CUP-01", Name = "校园马克杯", Price = 29.9m, Stock = 100, CategoryId = life.Id });
        db.Students.AddRange(
            new Student { StudentNumber = "2024001001", FullName = "张三", Major = "计算机", EnrollmentYear = 2024 },
            new Student { StudentNumber = "2024001002", FullName = "李四", Major = "软件工程", EnrollmentYear = 2024 });
        await db.SaveChangesAsync();
    }
}

app.MapGet("/", () => Results.Ok(new { lab = "Part04_01 EF Core", db = "lab_part04_ef" }));

// Projection DTO + keyset (no N+1: Select without Include of collections)
app.MapGet("/api/products", async (CampusDb db, int? afterId, int pageSize = 20) =>
{
    pageSize = Math.Clamp(pageSize, 1, 50);
    var q = db.Products.AsNoTracking().OrderBy(p => p.Id).AsQueryable();
    if (afterId is int a) q = q.Where(p => p.Id > a);
    var items = await q.Take(pageSize)
        .Select(p => new { p.Id, p.Sku, p.Name, p.Price, p.Stock, Category = p.Category!.Name })
        .ToListAsync();
    return Results.Ok(items);
});

// N+1 demo endpoint (bad) vs fixed
app.MapGet("/api/categories/bad", async (CampusDb db) =>
{
    var cats = await db.Categories.AsNoTracking().ToListAsync();
    // N+1: one query per category for products
    var result = new List<object>();
    foreach (var c in cats)
    {
        var products = await db.Products.AsNoTracking().Where(p => p.CategoryId == c.Id).Select(p => p.Name).ToListAsync();
        result.Add(new { c.Name, products });
    }
    return Results.Ok(new { mode = "n+1", result });
});

app.MapGet("/api/categories/good", async (CampusDb db) =>
{
    var result = await db.Categories.AsNoTracking()
        .Select(c => new { c.Name, products = c.Products.Select(p => p.Name).ToList() })
        .ToListAsync();
    return Results.Ok(new { mode = "projection", result });
});

// Optimistic concurrency
app.MapPut("/api/products/{id:int}/stock", async (int id, int stock, uint rowVersion, CampusDb db) =>
{
    var p = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
    if (p is null) return Results.NotFound();
    p.Stock = stock;
    p.RowVersion = rowVersion; // client token
    try
    {
        await db.SaveChangesAsync();
        return Results.Ok(new { p.Id, p.Stock, p.RowVersion });
    }
    catch (DbUpdateConcurrencyException)
    {
        return Results.Conflict(new { title = "Concurrency conflict", id });
    }
});

app.MapGet("/api/students", async (CampusDb db) =>
    await db.Students.AsNoTracking().Select(s => new { s.StudentNumber, s.FullName, s.Major, s.EnrollmentYear }).ToListAsync());

app.Run();
public partial class Program;
