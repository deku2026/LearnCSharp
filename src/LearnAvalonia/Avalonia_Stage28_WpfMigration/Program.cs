using Avalonia;

namespace LearnAvalonia.Avalonia_Stage28_WpfMigration;

internal static class Program
{
    // Doc: 第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §0
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
