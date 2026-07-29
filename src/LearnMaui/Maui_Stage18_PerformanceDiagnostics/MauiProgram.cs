namespace LearnMaui.Maui_Stage18_PerformanceDiagnostics;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<PerformanceTelemetry>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
