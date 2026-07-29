namespace LearnMaui.Maui_Stage07_NavigationShell;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        Routing.RegisterRoute(RouteCatalog.ArticleDetail, typeof(ArticleDetailPage));
        Routing.RegisterRoute(RouteCatalog.Login, typeof(LoginPage));

        builder.Services.AddSingleton<SessionState>();
        builder.Services.AddSingleton<NavigationGate>();
        builder.Services.AddSingleton<NavigationJournal>();
        builder.Services.AddSingleton<INavigationService, ShellNavigationService>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AppShell>();

        return builder.Build();
    }
}
