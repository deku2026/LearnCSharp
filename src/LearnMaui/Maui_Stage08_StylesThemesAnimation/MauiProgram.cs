namespace LearnMaui.Maui_Stage08_StylesThemesAnimation;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddTransient<InteractionDemoView>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
