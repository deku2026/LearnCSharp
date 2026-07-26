using System.Globalization;
using LearnBlazor.Blazor_Part08_AccessibilityI18n.Components;
using Microsoft.AspNetCore.Localization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddLocalization();
builder.Services.AddHttpContextAccessor();

WebApplication app = builder.Build();

CultureInfo[] supported = new[] { new CultureInfo("en"), new CultureInfo("zh-CN") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supported,
    SupportedUICultures = supported,
});

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();