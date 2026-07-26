using LearnBlazor.Blazor_Part16_PackagingEngineering.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

app.UseExceptionHandler("/error", createScopeForErrors: true);
app.UseHsts();
app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 静态资产缓存头（生产用更长 max-age + immutable）
        ctx.Context.Response.Headers.CacheControl = "public, max-age=3600";
    },
});
app.UseAntiforgery();

app.MapHealthChecks("/healthz");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();