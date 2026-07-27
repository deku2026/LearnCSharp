using Avalonia;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT;

internal static class Program
{
    // Doc: 第10部分-Capstone4-自定义控件渲染与Native-AOT.md §5
    // Notes: IsAotCompatible=true 演示；BuildAvaloniaApp doubles as design-time preview.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    [STAThread]
    public static int Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
}
