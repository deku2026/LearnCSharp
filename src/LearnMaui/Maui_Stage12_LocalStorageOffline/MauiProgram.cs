namespace LearnMaui.Maui_Stage12_LocalStorageOffline;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        string databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "stage12-offline.db3");
        builder.Services.AddSingleton<IOfflineNoteRepository>(
            new OfflineNoteRepository(databasePath));
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
