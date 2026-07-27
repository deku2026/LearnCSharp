// Doc:    第9部分-阶段22-数据存储网络与认证.md §4
// Title:  弹性、重试、取消
// Stage:  Stage22
// Accept: 取消贯穿列表加载
// Notes:  CTS 模拟取消；导航离开页面 Cancel；超时分层；离线检测。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class ResilienceCancellationDemo : UserControl
{
    private CancellationTokenSource? _loadCts;

    public ResilienceCancellationDemo()
    {
        InitializeComponent();
    }

    private async void StartLoadButton_Click(object? sender, RoutedEventArgs e)
    {
        _loadCts?.Cancel();
        _loadCts = new CancellationTokenSource();
        CancellationToken token = _loadCts.Token;
        if (ResilienceLog is not null)
        {
            ResilienceLog.Text = "开始加载（模拟 3s 任务）...";
        }
        try
        {
            for (int i = 1; i <= 30; i++)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(100, token);
                if (ResilienceLog is not null && i % 10 == 0)
                {
                    ResilienceLog.Text = $"加载进度：{i * 10 / 30 * 10}%（{i}/30）";
                }
            }
            if (ResilienceLog is not null)
            {
                ResilienceLog.Text = "加载完成（未取消）。";
            }
        }
        catch (OperationCanceledException)
        {
            if (ResilienceLog is not null)
            {
                ResilienceLog.Text = "加载已取消（OperationCanceledException 被吞掉，不报错）。";
            }
        }
    }

    private void CancelLoadButton_Click(object? sender, RoutedEventArgs e)
    {
        _loadCts?.Cancel();
        if (ResilienceLog is not null)
        {
            ResilienceLog.Text = "已请求取消（CTS.Cancel()）...";
        }
    }

    private void LeavePageButton_Click(object? sender, RoutedEventArgs e)
    {
        // 导航离开页面 → Cancel（模拟）。
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = null;
        if (ResilienceLog is not null)
        {
            ResilienceLog.Text = "模拟「离开页面」→ CTS.Cancel() + Dispose()，取消令牌跟生命周期绑定。";
        }
    }
}
