using Avalonia;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT;

public static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .UseHarfBuzz()
            .LogToTrace();
}
