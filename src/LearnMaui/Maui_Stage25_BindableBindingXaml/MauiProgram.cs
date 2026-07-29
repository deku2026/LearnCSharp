namespace LearnMaui.Maui_Stage25_BindableBindingXaml;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<BindingLabViewModel>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
