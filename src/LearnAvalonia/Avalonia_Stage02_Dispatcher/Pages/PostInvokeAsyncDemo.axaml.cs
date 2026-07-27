// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §3
// Title:  Post、InvokeAsync 与优先级
// Stage:  Stage02
// Accept: 说明 Post 与 InvokeAsync 差异及 ExecutionContext 捕获
// Notes:  交互式：Post 火后不管 vs InvokeAsync 可等待。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class PostInvokeAsyncDemo : UserControl
{
    public PostInvokeAsyncDemo()
    {
        InitializeComponent();
    }

    private void PostButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ResultText is not null)
        {
            ResultText.Text = "Post：已排队，主线程继续...";
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (ResultText is not null)
            {
                ResultText.Text = $"Post 回调执行：tid={Environment.CurrentManagedThreadId}，回调稍后运行";
            }
        });
    }

    private async void InvokeAsyncButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ResultText is not null)
        {
            ResultText.Text = "InvokeAsync：调用方等待...";
        }

        int uiTid = await Dispatcher.UIThread.InvokeAsync(() => Environment.CurrentManagedThreadId);
        if (ResultText is not null)
        {
            ResultText.Text = $"InvokeAsync 完成：回调在 tid={uiTid} 执行，调用方已 await";
        }

        await Task.Delay(50);
    }
}
