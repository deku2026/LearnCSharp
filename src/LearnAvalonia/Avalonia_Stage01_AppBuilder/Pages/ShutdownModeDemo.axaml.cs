// Doc:    第2部分-阶段1-AppBuilder与应用启动.md §5
// Title:  Desktop ShutdownMode
// Stage:  Stage01
// Accept: 配置并验证两种 ShutdownMode
// Notes:  交互式：开第二窗口观察 OnLastWindowClose/OnMainWindowClose/OnExplicitShutdown 差异。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder.Pages;

public partial class ShutdownModeDemo : UserControl
{
    public ShutdownModeDemo()
    {
        InitializeComponent();
    }

    private void OpenChildButton_Click(object? sender, RoutedEventArgs e)
    {
        Window child = new()
        {
            Title = "子窗口 — 关我观察 ShutdownMode",
            Width = 360,
            Height = 240,
            Content = new TextBlock
            {
                Text = "子窗口已打开。\n关闭它观察进程是否退出。",
                Margin = new Thickness(16),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            },
        };
        child.Show();
        if (LogText is not null)
        {
            LogText.Text = "日志：已打开子窗口。关闭它观察 lifetime 行为。";
        }
    }

    private void ExplicitShutdownButton_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime controlled)
        {
            if (LogText is not null)
            {
                LogText.Text = "日志：调用 controlled.Shutdown() —— 仅 OnExplicitShutdown 会真正退出。";
            }

            controlled.Shutdown();
        }
        else if (LogText is not null)
        {
            LogText.Text = "日志：当前 lifetime 不可控（非 IControlledApplicationLifetime）。";
        }
    }
}
