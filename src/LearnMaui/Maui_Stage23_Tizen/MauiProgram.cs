namespace LearnMaui.Maui_Stage23_Tizen;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<IOptionalPlatformCapabilities, AndroidFallbackCapabilities>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
