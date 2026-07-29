namespace LearnMaui.Maui_Stage26_HandlerMapperLayout;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<ExpertEntry, TracingEntryHandler>());

        HandlerLabMappings.Configure();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
