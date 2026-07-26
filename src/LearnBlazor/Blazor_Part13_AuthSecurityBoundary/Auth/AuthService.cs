using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace LearnBlazor.Blazor_Part13_AuthSecurityBoundary.Auth;

public sealed class AuthService
{
    private readonly IHttpContextAccessor _http;
    private static readonly Dictionary<string, User> Users = new()
    {
        ["alice"] = new User("alice", new[] { "admin" }),
        ["bob"] = new User("bob", new[] { "user" }),
    };

    public AuthService(IHttpContextAccessor http) => _http = http;

    public bool IsAuthenticated => _http.HttpContext?.User.Identity?.IsAuthenticated == true;

    public bool IsAdmin => _http.HttpContext?.User.IsInRole("admin") == true;

    public string? CurrentName => _http.HttpContext?.User.Identity?.Name;

    public async Task<bool> LoginAsync(string username, string password)
    {
        if (!Users.TryGetValue(username, out User? user) || password != "password")
        {
            return false;
        }
        ClaimsPrincipal principal = user.ToPrincipal();
        await _http.HttpContext!.SignInAsync(
            "simple",
            principal,
            new AuthenticationProperties { IsPersistent = true });
        return true;
    }

    public async Task LogoutAsync()
    {
        if (_http.HttpContext is not null)
        {
            await _http.HttpContext.SignOutAsync("simple");
        }
    }
}
