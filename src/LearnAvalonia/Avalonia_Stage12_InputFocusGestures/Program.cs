using Avalonia;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures;

internal static class Program
{
    // Doc: 第5部分-阶段12-输入焦点手势与IME.md §2.1
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
