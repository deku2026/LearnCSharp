using Avalonia;

namespace LearnAvalonia.Avalonia_Stage30_PlatformBackend;

internal static class Program
{
    // Doc: 第11部分-阶段30-平台后端选读.md §0.1
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
