using Avalonia;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform;

internal static class Program
{
    // Doc: 第8部分-Capstone3-全平台Avalonia应用.md §1
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
