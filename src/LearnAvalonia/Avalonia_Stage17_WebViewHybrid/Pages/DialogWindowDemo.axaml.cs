// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §2.2
// Title:  独立对话框窗口
// Stage:  Stage17
// Accept: NativeWebDialog.Show()
// Notes:  NativeWebDialog 弹独立窗口承载 webview；适合「打开 OAuth/帮助」场景。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class DialogWindowDemo : UserControl
{
    public DialogWindowDemo()
    {
        InitializeComponent();
    }
}
