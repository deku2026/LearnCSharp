namespace LearnMaui.Maui_Stage03_UIThreadAsync;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddTransient<AsyncWorkCoordinator>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
