using LearnBlazor.Blazor_Part10_JsInteropWebView.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

WebApplication app = builder.Build();

// CSP：开发用 Report-Only 不阻断框架脚本；上线前用真实 CSP 并配 connect-src 'self'
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.ContentSecurityPolicyReportOnly =
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';";
    await next();
});

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
