using Avalonia;

namespace LearnAvalonia.Avalonia_Stage13_GraphicsAnimation;

internal static class Program
{
    // Doc: 第5部分-阶段13-图形动画与效果.md §2.1
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
