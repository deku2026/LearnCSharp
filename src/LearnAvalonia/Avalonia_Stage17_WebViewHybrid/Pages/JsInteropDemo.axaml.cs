// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §4
// Title:  JS ↔ C# 互操作
// Stage:  Stage17
// Accept: InvokeScript + WebMessageReceived
// Notes:  C# → JS 用 InvokeScript；JS → C# 用 invokeCSharpAction + WebMessageReceived 事件。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class JsInteropDemo : UserControl
{
    public JsInteropDemo()
    {
        InitializeComponent();
    }
}
