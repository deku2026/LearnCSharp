using Avalonia;

namespace LearnAvalonia.Avalonia_Stage25_Performance;

internal static class Program
{
    // Doc: 第10部分-阶段25-性能与诊断.md §0
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
