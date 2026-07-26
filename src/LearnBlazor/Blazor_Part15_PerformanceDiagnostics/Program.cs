using LearnBlazor.Blazor_Part15_PerformanceDiagnostics.Components;
using LearnBlazor.Blazor_Part15_PerformanceDiagnostics.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<StreamService>();

WebApplication app = builder.Build();

app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 静态资产缓存头（生产用更长 max-age）
        ctx.Context.Response.Headers.CacheControl = "public, max-age=3600";
    },
});
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
