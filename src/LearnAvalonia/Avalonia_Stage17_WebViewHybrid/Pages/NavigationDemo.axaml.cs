// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §2.3
// Title:  前进/后退/刷新
// Stage:  Stage17
// Accept: GoBack/GoForward/Reload
// Notes:  NativeWebView 提供 GoBack()/GoForward()/Reload()/Stop()；CanGoBack/CanGoForward 通知。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class NavigationDemo : UserControl
{
    public NavigationDemo()
    {
        InitializeComponent();
    }
}
