using Avalonia;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher;

internal static class Program
{
    // Doc: 第2部分-阶段2-Dispatcher线程与UI亲和性.md
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
