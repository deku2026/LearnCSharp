namespace LearnMaui.Maui_Stage28_CollectionViewRuntime;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<ICollectionDispatcher, MauiCollectionDispatcher>();
        builder.Services.AddSingleton<CollectionLabViewModel>();
        builder.Services.AddSingleton<EssentialsProbe>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
