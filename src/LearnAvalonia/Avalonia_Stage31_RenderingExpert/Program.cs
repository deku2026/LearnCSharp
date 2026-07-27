using Avalonia;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert;

internal static class Program
{
    // Doc: 第11部分-阶段31-渲染专家档.md §0.1
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
