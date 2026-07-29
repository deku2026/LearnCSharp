namespace LearnMaui.Maui_Stage21_BlazorHybrid;

public static class MauiProgram
{
    static MauiProgram()
    {
        AppContext.SetSwitch("HybridWebView.InvokeJavaScriptThrowsExceptions", true);
    }

    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<HybridBridge>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Services.AddHybridWebViewDeveloperTools();
#endif

        return builder.Build();
    }
}
