using LearnBlazor.Blazor_Part04_FrontendStateArchitecture.Components;
using LearnBlazor.Blazor_Part04_FrontendStateArchitecture.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<CounterState>();
builder.Services.AddScoped<CartState>();
builder.Services.AddScoped<ThemeState>();

WebApplication app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
