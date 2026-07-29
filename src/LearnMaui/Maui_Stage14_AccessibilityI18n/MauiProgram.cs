namespace LearnMaui.Maui_Stage14_AccessibilityI18n;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<LocalizedText>();
        builder.Services.AddTransient<AccessibilityDemoView>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
