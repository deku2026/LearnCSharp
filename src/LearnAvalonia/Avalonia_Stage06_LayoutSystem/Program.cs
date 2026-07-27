using Avalonia;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem;

internal static class Program
{
    // Doc: 第4部分-阶段6-布局系统.md §A
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
