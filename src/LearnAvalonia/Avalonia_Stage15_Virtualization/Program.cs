using Avalonia;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization;

internal static class Program
{
    // Doc: 第6部分-阶段15-虚拟化与大数据UI.md §0.3
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
