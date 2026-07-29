namespace LearnMaui.Maui_Stage27_WindowShellDispatcher;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        Routing.RegisterRoute(RouteCatalog.Detail, typeof(DetailPage));
        Routing.RegisterRoute(RouteCatalog.Login, typeof(LoginPage));

        builder.Services.AddSingleton<WindowJournal>();
        builder.Services.AddSingleton<NavigationJournal>();
        builder.Services.AddSingleton<SessionGate>();
        builder.Services.AddSingleton<IUiDispatcher, MauiUiDispatcher>();
        builder.Services.AddSingleton<DeepLinkCoordinator>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DetailPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<AppShell>();

        return builder.Build();
    }
}
