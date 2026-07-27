using Avalonia;

namespace LearnAvalonia.Avalonia_Stage11_PagesNavigation;

internal static class Program
{
    // Doc: 第5部分-阶段11-Pages与导航架构.md §2.1
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
