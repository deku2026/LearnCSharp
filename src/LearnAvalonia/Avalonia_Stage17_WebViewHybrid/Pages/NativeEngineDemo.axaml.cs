// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §3
// Title:  原生引擎细节
// Stage:  Stage17
// Accept: Win WebView2 / macOS WKWebView
// Notes:  Win 用 Edge WebView2（旧机用 WebView1 兜底）；macOS/iOS 用 WKWebView；Linux WPE/WebKitGTK。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class NativeEngineDemo : UserControl
{
    public NativeEngineDemo()
    {
        InitializeComponent();
    }
}
