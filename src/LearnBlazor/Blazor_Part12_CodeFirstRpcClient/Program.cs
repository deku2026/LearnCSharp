using LearnBlazor.Blazor_Part12_CodeFirstRpcClient.Components;
using LearnBlazor.Blazor_Part12_CodeFirstRpcClient.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<ItemsClient>((sp, c) =>
{
    IConfiguration config = sp.GetRequiredService<IConfiguration>();
    c.BaseAddress = new Uri(config["Api:Base"] ?? "http://localhost:5199");
})
.AddStandardResilienceHandler();

WebApplication app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
