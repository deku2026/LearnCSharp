// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §2.4
// Title:  本地资源 vs 远程
// Stage:  Stage17
// Accept: avares:// vs https://
// Notes:  Source 可指向 avares://（包内资源）或 https://（远程）；本地优先 avares。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class LocalVsRemoteDemo : UserControl
{
    public LocalVsRemoteDemo()
    {
        InitializeComponent();
    }
}
