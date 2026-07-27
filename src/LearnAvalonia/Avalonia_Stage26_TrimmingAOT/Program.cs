using Avalonia;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT;

internal static class Program
{
    // Doc: 第10部分-阶段26-Trimming与Native-AOT.md §0
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
