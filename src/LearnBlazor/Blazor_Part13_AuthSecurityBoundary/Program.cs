using LearnBlazor.Blazor_Part13_AuthSecurityBoundary.Auth;
using LearnBlazor.Blazor_Part13_AuthSecurityBoundary.Components;
using Microsoft.AspNetCore.Components.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication("simple")
    .AddCookie("simple", o =>
    {
        o.LoginPath = "/login";
        o.Cookie.Name = "Part13.Auth";
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthStateProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddCascadingAuthenticationState();

WebApplication app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
