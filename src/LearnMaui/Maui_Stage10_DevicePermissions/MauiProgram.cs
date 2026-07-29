namespace LearnMaui.Maui_Stage10_DevicePermissions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<IDeviceCapabilities, MauiDeviceCapabilities>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
