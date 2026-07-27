using Avalonia;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls;

internal static class Program
{
    // Doc: 第6部分-阶段14-自定义控件.md §2.1
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
