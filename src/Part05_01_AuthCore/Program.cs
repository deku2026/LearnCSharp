using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Part05-CampusShop-Signing-Key-32b!!"));
builder.Services.AddCors(o => o.AddPolicy("spa", p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddRateLimiter(o =>
{
    o.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 20;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueLimit = 0;
    });
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateIssuerSigningKey = true, ValidateLifetime = true,
        ValidIssuer = "part05", ValidAudience = "spa", IssuerSigningKey = key
    };
});
builder.Services.AddAuthorization(o => o.AddPolicy("Admin", p => p.RequireRole("Admin")));
var app = builder.Build();
app.UseCors("spa");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapPost("/auth/token", (LoginDto dto) =>
{
    if (dto.Password != "campus123") return Results.Unauthorized();
    var role = dto.UserName == "admin" ? "Admin" : "Student";
    var token = new JwtSecurityToken("part05", "spa",
        [new Claim(ClaimTypes.Name, dto.UserName), new Claim(ClaimTypes.Role, role)],
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
    return Results.Ok(new { access_token = new JwtSecurityTokenHandler().WriteToken(token), role });
}).RequireRateLimiting("fixed");
app.MapGet("/api/admin", () => Results.Ok(new { area = "admin" })).RequireAuthorization("Admin").RequireRateLimiting("fixed");
app.MapGet("/api/me", (ClaimsPrincipal u) => Results.Ok(new { u.Identity?.Name })).RequireAuthorization().RequireRateLimiting("fixed");
app.Run();
public partial class Program;
public record LoginDto(string UserName, string Password);
