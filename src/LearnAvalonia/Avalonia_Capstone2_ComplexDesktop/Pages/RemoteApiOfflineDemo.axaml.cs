// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §7
// Title:  远程 API 与离线
// Stage:  Capstone2
// Accept: 同步状态 UX + 脏标记
// Notes:  IHttpClientFactory + STJ source gen；mock 服务器；离线读缓存/队列上传。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class RemoteApiOfflineDemo : UserControl
{
    public RemoteApiOfflineDemo()
    {
        InitializeComponent();
    }

    private void OnlineButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ApiLog is not null)
        {
            ApiLog.Text = "在线模式：\n  IHttpClientFactory + STJ source gen\n  模拟服务器（本地 ASP.NET Minimal API 同仓库加分）或公共 mock\n  推送本地 Dirty → 200 → Clean";
        }
    }

    private void OfflineButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ApiLog is not null)
        {
            ApiLog.Text = "离线模式：\n  IConnectivity 抽象检测断网\n  读缓存（SQLite/JSON）\n  本地编辑 → 脏标记 → 队列上传\n  恢复网络 → 批量推送";
        }
    }

    private void SyncStateButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ApiLog is not null)
        {
            ApiLog.Text = "同步状态 UX：\n  图标：已同步 🟢 / 待上传 🟡 / 错误 🔴\n  错误可点看详情与重试\n\n认证（最低）：API Key 进 SecureStore；推荐 PKCE 演示";
        }
    }
}
