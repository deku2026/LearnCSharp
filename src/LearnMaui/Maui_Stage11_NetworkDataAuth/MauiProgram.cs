namespace LearnMaui.Maui_Stage11_NetworkDataAuth;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<ITokenVault, MemoryTokenVault>();
        builder.Services.AddSingleton<TokenRefreshCoordinator>();
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services
            .AddHttpClient<IArticlesClient, ArticlesClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.learnmaui.example/");
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
