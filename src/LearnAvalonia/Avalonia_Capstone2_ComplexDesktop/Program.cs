using Avalonia;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop;

internal static class Program
{
    // Doc: 第9部分-Capstone2-复杂桌面生产应用.md §1
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
