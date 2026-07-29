namespace LearnMaui.Maui_Stage17_TestingSystem;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddTransient<TestingDemoView>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
