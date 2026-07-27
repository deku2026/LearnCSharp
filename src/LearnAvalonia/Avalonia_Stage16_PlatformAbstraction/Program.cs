using Avalonia;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction;

internal static class Program
{
    // Doc: 第7部分-阶段16-统一平台抽象.md §0.3
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
