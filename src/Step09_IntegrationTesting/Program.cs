using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Step09_IntegrationTesting.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("Default")
             ?? "Host=localhost;Port=5432;Database=lab_step09;Username=dotnet;Password=dotnet_dev";
    opt.UseNpgsql(cs);
});

builder.Services.AddAuthentication("Test")
    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
builder.Services.AddAuthorization();

var app = builder.Build();

// Auto schema + seed (eShop-style startup; EnsureCreated for lab simplicity when no migration assembly yet;
// Part04 will demonstrate full MigrateAsync + migration files)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Products.AnyAsync())
    {
        db.Products.AddRange(
            new ProductEntity { Sku = "CUP-001", Name = "校园马克杯", Price = 29.9m },
            new ProductEntity { Sku = "BK-LA-01", Name = "线性代数", Price = 45m });
        await db.SaveChangesAsync();
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/products", async (CatalogDbContext db) =>
    await db.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync());

app.MapPost("/api/products", async (CreateProductDto dto, CatalogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["name"] = string.IsNullOrWhiteSpace(dto.Name) ? ["required"] : [],
            ["price"] = dto.Price <= 0 ? ["must be > 0"] : []
        }.Where(kv => kv.Value.Length > 0).ToDictionary(kv => kv.Key, kv => kv.Value));
    }

    var entity = new ProductEntity { Sku = dto.Sku, Name = dto.Name, Price = dto.Price };
    db.Products.Add(entity);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{entity.Id}", entity);
}).RequireAuthorization();

app.MapGet("/api/secure/ping", () => Results.Ok(new { ok = true })).RequireAuthorization();

app.Run();

public partial class Program;

public sealed record CreateProductDto(string Sku, string Name, decimal Price);

namespace Step09_IntegrationTesting.Data
{
    public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
    {
        public DbSet<ProductEntity> Products => Set<ProductEntity>();
    }

    public sealed class ProductEntity
    {
        public int Id { get; set; }
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
    }
}

/// <summary>Test auth handler used by integration tests (and optional lab header).</summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-User", out var user) || string.IsNullOrWhiteSpace(user))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user!),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
