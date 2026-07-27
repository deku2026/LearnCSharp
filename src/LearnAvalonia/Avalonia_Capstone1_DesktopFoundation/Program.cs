using Avalonia;

namespace LearnAvalonia.Avalonia_Capstone1_DesktopFoundation;

internal static class Program
{
    // Doc: 第4部分-Capstone1-生产级桌面基础应用.md §0
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
