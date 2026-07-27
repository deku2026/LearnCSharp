// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §11
// Title:  更新服务
// Stage:  Capstone2
// Accept: IUpdateService 可演示
// Notes:  FakeUpdateService 检查到版本 99.0 提示演示更新；用户可稍后；签名校验真路径必须有设计。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class UpdateServiceDemo : UserControl
{
    public UpdateServiceDemo()
    {
        InitializeComponent();
    }

    private void CheckButton_Click(object? sender, RoutedEventArgs e)
    {
        if (UpdateLog is not null)
        {
            UpdateLog.Text = "FakeUpdateService.CheckAsync(ct) →\n  发现新版本 99.0（演示更新）\n  UpdateInfo { Version = \"99.0\", Notes = \"演示更新通道\" }\n\n或真 Velopack 通道加分";
        }
    }

    private void ApplyButton_Click(object? sender, RoutedEventArgs e)
    {
        if (UpdateLog is not null)
        {
            UpdateLog.Text = "ApplyAsync(info, ct) →\n  检查 → 下载 → 校验签名 → 应用 → 重启\n\n要求：\n  用户可「稍后」\n  不在 UI 线程同步下载无反馈\n  签名校验在真更新路径必须有设计";
        }
    }

    private void LaterButton_Click(object? sender, RoutedEventArgs e)
    {
        if (UpdateLog is not null)
        {
            UpdateLog.Text = "用户选「稍后」→ 本次不更新，下次启动再检查（或下次手动触发）";
        }
    }
}
