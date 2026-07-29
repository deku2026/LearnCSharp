namespace LearnMaui.Maui_Stage24_MauiAppBuilderDI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // Fonts are registered here when the project owns font assets.
            });

        builder.Services
            .AddApplicationCore()
            .AddPresentation();

        return builder.Build();
    }
}
