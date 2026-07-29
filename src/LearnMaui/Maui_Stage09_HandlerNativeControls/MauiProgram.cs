namespace LearnMaui.Maui_Stage09_HandlerNativeControls;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureMauiHandlers(HandlerCustomization.Configure);
        HandlerCustomization.ConfigureGlobalMappers();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
