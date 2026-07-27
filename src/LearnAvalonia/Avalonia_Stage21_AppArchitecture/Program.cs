using Avalonia;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture;

internal static class Program
{
    // Doc: 第9部分-阶段21-应用架构与工程结构.md §5
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
