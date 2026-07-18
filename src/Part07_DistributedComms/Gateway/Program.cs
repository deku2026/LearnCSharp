using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// YARP gateway: single entry, auth at edge, route to catalog/inventory

var builder = WebApplication.CreateBuilder(args);

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Part07-Gateway-Signing-Key-32bytes!"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = "part07-gateway",
            ValidAudience = "campus-clients",
            IssuerSigningKey = key
        };
    });
builder.Services.AddAuthorization();

var catalog = builder.Configuration["Services:Catalog"] ?? "http://localhost:5701";
var inventory = builder.Configuration["Services:Inventory"] ?? "http://localhost:5702";

builder.Services.AddReverseProxy()
    .LoadFromMemory(
        [
            new Yarp.ReverseProxy.Configuration.RouteConfig
            {
                RouteId = "catalog",
                ClusterId = "catalog",
                Match = new Yarp.ReverseProxy.Configuration.RouteMatch { Path = "/api/catalog/{**catch-all}" },
                Transforms = [new Dictionary<string, string> { ["PathPattern"] = "/api/{**catch-all}" }]
            },
            new Yarp.ReverseProxy.Configuration.RouteConfig
            {
                RouteId = "inventory",
                ClusterId = "inventory",
                Match = new Yarp.ReverseProxy.Configuration.RouteMatch { Path = "/api/inventory/{**catch-all}" },
                Transforms = [new Dictionary<string, string> { ["PathPattern"] = "/api/{**catch-all}" }]
            }
        ],
        [
            new Yarp.ReverseProxy.Configuration.ClusterConfig
            {
                ClusterId = "catalog",
                Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                {
                    ["d1"] = new() { Address = catalog }
                }
            },
            new Yarp.ReverseProxy.Configuration.ClusterConfig
            {
                ClusterId = "inventory",
                Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                {
                    ["d1"] = new() { Address = inventory }
                }
            }
        ]);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "gateway",
    routes = new[] { "/api/catalog/** → catalog", "/api/inventory/** → inventory" },
    auth = "JWT at edge"
}));

app.MapPost("/auth/token", (LoginDto dto) =>
{
    if (dto.Password != "campus123")
    {
        return Results.Unauthorized();
    }

    var token = new JwtSecurityToken(
        "part07-gateway",
        "campus-clients",
        [new Claim(ClaimTypes.Name, dto.UserName)],
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
    return Results.Ok(new { access_token = new JwtSecurityTokenHandler().WriteToken(token) });
});

app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "Healthy" }));

// Protect proxied APIs at the gateway (auth sink-down)
app.MapReverseProxy().RequireAuthorization();

app.Run();

public partial class Program;

namespace Part07.Gateway
{
    public sealed class AssemblyMarker;
}

public sealed record LoginDto(string UserName, string Password);
