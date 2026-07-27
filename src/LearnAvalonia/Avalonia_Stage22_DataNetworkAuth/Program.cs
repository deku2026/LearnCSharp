using Avalonia;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth;

internal static class Program
{
    // Doc: 第9部分-阶段22-数据存储网络与认证.md §1
    // Notes: BuildAvaloniaApp doubles as design-time preview entrypoint; keep signature stable.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    [STAThread]
    public static int Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
}
