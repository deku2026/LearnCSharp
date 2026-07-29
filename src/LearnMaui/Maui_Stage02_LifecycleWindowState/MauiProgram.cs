using Microsoft.Maui.LifecycleEvents;

namespace LearnMaui.Maui_Stage02_LifecycleWindowState;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android =>
                {
                    android.OnCreate((_, _) => LifecycleJournal.RecordPlatform("Android.OnCreate"));
                    android.OnStop(_ => LifecycleJournal.RecordPlatform("Android.OnStop"));
                    android.OnResume(_ => LifecycleJournal.RecordPlatform("Android.OnResume"));
                });
#endif
            });

        builder.Services.AddSingleton<LifecycleJournal>();
        builder.Services.AddSingleton<StateStore>();
        builder.Services.AddTransient<PageWorkController>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
