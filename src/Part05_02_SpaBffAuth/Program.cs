using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

// Part05_02 · SPA / BFF auth lab
// Simulates: SPA public client (PKCE notes) + BFF (HttpOnly session cookie, tokens server-side)

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection().SetApplicationName("Part05_02_SpaBffAuth");
builder.Services.AddSingleton<ServerSessionStore>();
builder.Services.AddSingleton<TokenEducationNotes>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "campus.bff.session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Strict; // first layer CSRF defense
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(o =>
{
    // Intentional: SPA on different origin would need CORS; BFF same-origin avoids it.
    o.AddPolicy("dev-spa", p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("dev-spa");
app.UseAuthentication();
app.UseAuthorization();

// --- CSRF: require custom header on mutating BFF calls (forces preflight / same-origin apps) ---
app.Use(async (ctx, next) =>
{
    if (HttpMethods.IsPost(ctx.Request.Method)
        || HttpMethods.IsPut(ctx.Request.Method)
        || HttpMethods.IsDelete(ctx.Request.Method))
    {
        // Login is public; protect BFF session-bearing APIs
        if (ctx.Request.Path.StartsWithSegments("/bff")
            && !ctx.Request.Path.StartsWithSegments("/bff/login")
            && !ctx.Request.Path.StartsWithSegments("/bff/csrf"))
        {
            if (!ctx.Request.Headers.TryGetValue("X-CSRF", out var csrf) || csrf != "1")
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    title = "CSRF check failed",
                    detail = "BFF cookie auth requires custom header X-CSRF:1 (SameSite+custom header pattern)."
                });
                return;
            }
        }
    }

    await next();
});

app.MapGet("/", () => Results.Redirect("/index.html"));

app.MapGet("/api/education/token-storage", (TokenEducationNotes notes) => Results.Ok(notes.GetAll()));

// Simulated IdP: issues opaque access token after "authorization code + PKCE" exchange (lab simplified)
app.MapPost("/idp/token", (PkceTokenRequest req, ServerSessionStore store) =>
{
    if (string.IsNullOrWhiteSpace(req.Code) || string.IsNullOrWhiteSpace(req.CodeVerifier))
    {
        return Results.BadRequest(new { error = "code and code_verifier required (PKCE)" });
    }

    // Lab: accept any non-empty verifier; real IdP verifies S256(challenge)
    var accessToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
    var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
    return Results.Ok(new
    {
        token_type = "Bearer",
        expires_in = 3600,
        access_token = accessToken,
        refresh_token = refreshToken,
        scope = "api.open",
        note = "PKCE protects the code exchange, NOT browser token storage afterwards."
    });
});

// --- BFF: confidential-style server keeps tokens; browser only gets session cookie ---
app.MapPost("/bff/login", async (BffLoginRequest req, HttpContext http, ServerSessionStore sessions) =>
{
    if (req.UserName is not ("student" or "admin") || req.Password != "campus123")
    {
        return Results.Unauthorized();
    }

    // Simulate code+PKCE then store tokens server-side
    var access = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
    var sessionId = Guid.NewGuid().ToString("N");
    sessions.Save(sessionId, new ServerSession(
        req.UserName,
        req.UserName == "admin" ? "Admin" : "Student",
        access,
        DateTimeOffset.UtcNow.AddHours(1)));

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, req.UserName),
        new(ClaimTypes.Role, req.UserName == "admin" ? "Admin" : "Student"),
        new("sid", sessionId)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity),
        new AuthenticationProperties { IsPersistent = true });

    return Results.Ok(new
    {
        ok = true,
        mode = "BFF",
        user = req.UserName,
        message = "Tokens stay on server session store; browser only has HttpOnly session cookie."
    });
});

app.MapPost("/bff/logout", async (HttpContext http, ServerSessionStore sessions) =>
{
    var sid = http.User.FindFirstValue("sid");
    if (sid is not null)
    {
        sessions.Remove(sid);
    }

    await http.SignOutAsync();
    return Results.Ok(new { ok = true });
}).RequireAuthorization();

app.MapGet("/bff/me", (ClaimsPrincipal user, ServerSessionStore sessions) =>
{
    var sid = user.FindFirstValue("sid");
    var session = sid is null ? null : sessions.Get(sid);
    return Results.Ok(new
    {
        name = user.Identity?.Name,
        role = user.FindFirstValue(ClaimTypes.Role),
        hasServerAccessToken = session?.AccessToken is not null,
        accessTokenPreview = session is null ? null : session.AccessToken[..8] + "…",
        note = "Browser never receives the full access token in BFF mode."
    });
}).RequireAuthorization();

// BFF proxies to "remote API" attaching server-side access token
app.MapGet("/bff/api/orders", (ClaimsPrincipal user, ServerSessionStore sessions) =>
{
    var sid = user.FindFirstValue("sid");
    var session = sid is null ? null : sessions.Get(sid);
    if (session is null)
    {
        return Results.Unauthorized();
    }

    // Stand-in for remote API call with Authorization: Bearer <server-token>
    return Results.Ok(new
    {
        caller = session.UserName,
        authorizationUsed = "Bearer " + session.AccessToken[..8] + "…",
        orders = new[]
        {
            new { id = "CS-1001", total = 59.8m, item = "校园马克杯 x2" }
        }
    });
}).RequireAuthorization();

// Anti-pattern demo endpoint: documents SPA storing token in localStorage (do NOT do for high-value)
app.MapPost("/spa/insecure-demo/login", (BffLoginRequest req) =>
{
    if (req.Password != "campus123")
    {
        return Results.Unauthorized();
    }

    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
    return Results.Ok(new
    {
        access_token = token,
        warning = "Returning token to browser JS is the SPA+token model. XSS can steal it from localStorage/memory.",
        saferAlternative = "BFF + HttpOnly cookie + server-side token store"
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.Run();

public partial class Program;

public sealed record BffLoginRequest(string UserName, string Password);
public sealed record PkceTokenRequest(string Code, string CodeVerifier, string? RedirectUri, string? ClientId);
public sealed record ServerSession(string UserName, string Role, string AccessToken, DateTimeOffset ExpiresAt);

public sealed class ServerSessionStore
{
    private readonly ConcurrentDictionary<string, ServerSession> _sessions = new();

    public void Save(string id, ServerSession session) => _sessions[id] = session;
    public ServerSession? Get(string id) => _sessions.TryGetValue(id, out var s) ? s : null;
    public void Remove(string id) => _sessions.TryRemove(id, out _);
}

public sealed class TokenEducationNotes
{
    public object GetAll() => new
    {
        storages = new object[]
        {
            new { place = "localStorage", xssReadable = true, survivesRefresh = true, risk = "Highest for tokens" },
            new { place = "sessionStorage", xssReadable = true, survivesRefresh = false, risk = "Still XSS-readable" },
            new { place = "JS memory", xssReadable = true, survivesRefresh = false, risk = "Better than localStorage, not safe" },
            new { place = "HttpOnly cookie (session id)", xssReadable = false, survivesRefresh = true, risk = "CSRF concern → SameSite + custom header" }
        },
        bffBenefits = new[]
        {
            "Access/refresh tokens never touch browser JS",
            "XSS cannot steal bearer token from storage",
            "SPA talks same-origin to BFF (less CORS pain)",
            "Confidential client secrets stay on server"
        },
        bffCosts = new[]
        {
            "Extra hop (BFF)",
            "Must protect cookie CSRF",
            "Session store + Data Protection key ring for multi-instance"
        },
        modernGuidance = "IETF BCP discourages browser-based OAuth token handling for high-value apps → prefer BFF."
    };
}
