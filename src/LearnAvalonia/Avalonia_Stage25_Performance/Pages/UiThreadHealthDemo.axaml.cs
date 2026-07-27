// Doc:    第10部分-阶段25-性能与诊断.md §2
// Title:  UI 线程健康
// Stage:  Stage25
// Accept: UI 线程无长任务
// Notes:  UI 线程只做 UI；长任务 Task.Run；Dispatcher.Post 后台到 UI。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace LearnAvalonia.Avalonia_Stage25_Performance.Pages;

public partial class UiThreadHealthDemo : UserControl
{
    public UiThreadHealthDemo()
    {
        InitializeComponent();
    }

    private async void RunBackgroundButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ResultText is null)
        {
            return;
        }

        ResultText.Text = "开始后台任务（Task.Run）...";
        int result = await Task.Run(() =>
        {
            int sum = 0;
            for (int i = 0; i < 10_000_000; i++)
            {
                sum += i;
            }

            return sum;
        });

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            ResultText.Text = "后台完成：sum=" + result + "\n→ UI 线程未阻塞，按钮可点\n→ 长任务必须 Task.Run + Dispatcher 回 UI";
        });
    }
}
