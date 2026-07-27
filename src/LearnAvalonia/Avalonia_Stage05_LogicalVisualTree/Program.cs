using Avalonia;

namespace LearnAvalonia.Avalonia_Stage05_LogicalVisualTree;

internal static class Program
{
    // Doc: 第3部分-阶段5-LogicalTree-VisualTree-NameScope.md
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
