// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §6
// Title:  后台 → UI：正确与错误写法
// Stage:  Stage02
// Accept: 给出正确与错误示例各一
// Notes:  交互式：后台计算 + 显式 InvokeAsync 封回 UI；VM 属性路径。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class BackgroundToUiDemo : UserControl
{
    public BackgroundToUiDemo()
    {
        InitializeComponent();
    }

    private async void CorrectButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ResultText is not null)
        {
            ResultText.Text = "后台计算中...";
        }

        int result = await Task.Run(() =>
        {
            int sum = 0;
            for (int i = 0; i < 5_000_000; i++)
            {
                sum += i;
            }

            return sum;
        });

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (ResultText is not null)
            {
                ResultText.Text = $"✅ 后台计算完成：sum={result}，已 InvokeAsync 封回 UI 线程显示";
            }
        });
    }

    private async void VmButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ResultText is not null)
        {
            ResultText.Text = "模拟 VM 属性路径：await 后直接赋值（已在 UI 同步上下文）...";
        }

        string data = await Task.Run(() => "来自后台的标题数据");
        if (ResultText is not null)
        {
            ResultText.Text = $"✅ VM 路径：data=\"{data}\" —— async 事件 await 后通常仍在 UI 线程";
        }
    }
}
