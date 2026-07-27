// Doc:    第9部分-阶段22-数据存储网络与认证.md §8
// Title:  同步与冲突
// Stage:  Stage22
// Accept: 冲突策略可演示
// Notes:  LWW/Server-Wins/Client-Wins/Manual；客户端时间不可靠，优先服务器 updatedAt/ETag。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class SyncConflictDemo : UserControl
{
    public SyncConflictDemo()
    {
        InitializeComponent();
    }

    private void KeepLocalButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SyncLog is not null)
        {
            SyncLog.Text = "策略：Client-Wins（KeepLocal）\n→ 强制推本地版本到服务器\n→ 简单但易丢服务器侧他人修改";
        }
    }

    private void KeepRemoteButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SyncLog is not null)
        {
            SyncLog.Text = "策略：Server-Wins（KeepRemote）\n→ 强制拉服务器版本覆盖本地\n→ 服务器为准，本地修改丢失";
        }
    }

    private void ManualButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SyncLog is not null)
        {
            SyncLog.Text = "策略：Manual（UI 展示两边 diff）\n→ 用户逐字段选择保留版本\n→ 最安全但 UX 复杂\n→ Capstone 要求至少检测冲突 + 用户选一边";
        }
    }

    private void LwwButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SyncLog is not null)
        {
            SyncLog.Text = "策略：Last-Write-Wins（LWW）\n→ 比时间戳；简单易丢数据\n→ 客户端时间不可靠，优先服务器 updatedAt / ETag / 版本向量";
        }
    }
}
