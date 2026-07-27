using Avalonia;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI;

internal static class Program
{
    // Doc: 第10部分-阶段27-打包部署更新与CI.md §0
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
