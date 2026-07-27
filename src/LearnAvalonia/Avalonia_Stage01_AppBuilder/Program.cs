using Avalonia;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder;

internal static class Program
{
    // Doc: 第2部分-阶段1-AppBuilder与应用启动.md §3.1
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
