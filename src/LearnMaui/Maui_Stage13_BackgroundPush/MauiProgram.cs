namespace LearnMaui.Maui_Stage13_BackgroundPush;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<IBackgroundScheduler, InMemoryBackgroundScheduler>();
        builder.Services.AddSingleton<BackgroundSyncCoordinator>();
        builder.Services.AddSingleton<INotificationPlatformBridge, AndroidNotificationPlatformBridge>();
        builder.Services.AddSingleton<PushRegistrationStore>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
