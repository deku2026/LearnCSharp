// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §8
// Title:  CSP / XSS / 安全
// Stage:  Stage17
// Accept: CSP/转义/白名单
// Notes:  Content-Security-Policy 头 + JS 字符串转义 + 白名单 Source。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class SecurityDemo : UserControl
{
    public SecurityDemo()
    {
        InitializeComponent();
    }
}
