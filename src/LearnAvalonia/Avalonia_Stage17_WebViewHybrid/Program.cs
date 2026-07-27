using Avalonia;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid;

internal static class Program
{
    // Doc: 第7部分-阶段17-WebView与Hybrid-UI.md §0.3
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
