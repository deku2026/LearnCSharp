using Avalonia;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading;

internal static class Program
{
    // Doc: 第11部分-阶段29-必读子系统源码精读.md §0.5
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
