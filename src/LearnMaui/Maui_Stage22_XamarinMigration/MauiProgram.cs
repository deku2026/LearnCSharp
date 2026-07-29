namespace LearnMaui.Maui_Stage22_XamarinMigration;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<MigrationAuditor>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
