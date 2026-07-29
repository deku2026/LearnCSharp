namespace LearnMaui.Maui_Stage19_TrimmingAOT;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<AotFriendlyViewFactory>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
