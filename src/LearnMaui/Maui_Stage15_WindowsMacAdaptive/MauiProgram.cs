namespace LearnMaui.Maui_Stage15_WindowsMacAdaptive;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<WindowWorkspaceRegistry>();
        builder.Services.AddSingleton<IDesktopPlatformService, DesktopPlatformService>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
