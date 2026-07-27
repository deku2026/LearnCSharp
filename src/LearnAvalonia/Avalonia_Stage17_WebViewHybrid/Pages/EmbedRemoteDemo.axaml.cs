// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §2.1
// Title:  嵌入远程页（XAML）
// Stage:  Stage17
// Accept: NativeWebView Source=URL
// Notes:  NativeWebView 是 Control；Source 是 Uri；NavigationCompleted 事件。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class EmbedRemoteDemo : UserControl
{
    public EmbedRemoteDemo()
    {
        InitializeComponent();
    }
}
