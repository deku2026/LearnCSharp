using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

const string signingKey = "Step07-CampusShop-Dev-Signing-Key-32bytes!";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "campusshop-step07",
            ValidAudience = "campusshop-clients",
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("StudentOrAdmin", p => p.RequireRole("Student", "Admin"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new { lab = "Step07 AuthN/AuthZ" }));

app.MapPost("/auth/token", (LoginRequest req) =>
{
    if (string.IsNullOrWhiteSpace(req.UserName) || string.IsNullOrWhiteSpace(req.Password))
    {
        return Results.BadRequest(new { error = "username/password required" });
    }

    // Lab users: admin/admin123 → Admin; others → Student if password student123
    string[] roles = req.UserName.Equals("admin", StringComparison.OrdinalIgnoreCase) && req.Password == "admin123"
        ? ["Admin"]
        : req.Password == "student123"
            ? ["Student"]
            : [];

    if (roles.Length == 0)
    {
        return Results.Unauthorized();
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, req.UserName),
        new(JwtRegisteredClaimNames.Sub, req.UserName)
    };
    claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: "campusshop-step07",
        audience: "campusshop-clients",
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: creds);

    return Results.Ok(new
    {
        access_token = new JwtSecurityTokenHandler().WriteToken(token),
        token_type = "Bearer",
        roles
    });
});

app.MapGet("/api/me", (ClaimsPrincipal user) => Results.Ok(new
{
    name = user.Identity?.Name,
    isAuthenticated = user.Identity?.IsAuthenticated ?? false,
    roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
})).RequireAuthorization();

app.MapGet("/api/admin/dashboard", () => Results.Ok(new { area = "admin" }))
    .RequireAuthorization("AdminOnly");

app.MapGet("/api/students/me/orders", () => Results.Ok(new { orders = Array.Empty<object>() }))
    .RequireAuthorization("StudentOrAdmin");

app.Run();

public partial class Program;
public sealed record LoginRequest(string UserName, string Password);
