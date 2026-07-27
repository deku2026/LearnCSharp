// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §11
// Title:  平台差异细节
// Stage:  Stage17
// Accept: 三平台 webview 后端
// Notes:  Win WebView2 / macOS WKWebView / Linux WPE WebKit（NativeWebView）或 WebKitGTK（Dialog）。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class PlatformDiffDemo : UserControl
{
    public PlatformDiffDemo()
    {
        InitializeComponent();
    }
}
