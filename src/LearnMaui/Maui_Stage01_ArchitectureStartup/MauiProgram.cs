using Microsoft.Extensions.Logging;

namespace LearnMaui.Maui_Stage01_ArchitectureStartup;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(_ =>
            {
                // Font registration belongs at the composition root. The sample uses
                // system fonts, so no external font asset is required.
            });

        builder.Logging.AddProvider(new StartupLoggerProvider());
        builder.Services.AddArchitectureStartup();

        return builder.Build();
    }

    private static IServiceCollection AddArchitectureStartup(this IServiceCollection services)
    {
        services.AddSingleton<IStartupClock, SystemStartupClock>();
        services.AddSingleton<AppSession>();
        services.AddTransient<StartupProbe>();
        services.AddTransient<MainPage>();
        return services;
    }
}
