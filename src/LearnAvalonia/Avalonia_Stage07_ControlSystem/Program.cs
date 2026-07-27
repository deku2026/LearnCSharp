using Avalonia;

namespace LearnAvalonia.Avalonia_Stage07_ControlSystem;

internal static class Program
{
    // Doc: 第4部分-阶段7-控件体系.md §A
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
