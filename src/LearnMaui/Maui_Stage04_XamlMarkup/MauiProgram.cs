using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;

namespace LearnMaui.Maui_Stage04_XamlMarkup;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup();

        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<XamlDemoView>();
        builder.Services.AddTransient<MarkupDemoView>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
