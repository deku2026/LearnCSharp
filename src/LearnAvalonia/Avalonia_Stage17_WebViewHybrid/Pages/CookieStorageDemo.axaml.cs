// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §7
// Title:  Cookie / 存储 / 隐私
// Stage:  Stage17
// Accept: Cookie + localStorage 隔离
// Notes:  WebView2 userDataFolder 隔离 Cookie/localStorage；多账户隔离用多 Environment。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class CookieStorageDemo : UserControl
{
    public CookieStorageDemo()
    {
        InitializeComponent();
    }
}
