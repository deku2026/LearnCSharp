// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §1
// Title:  版本 pin + 包清单
// Stage:  Stage17
// Accept: CPM pin WebView 12.0.1
// Notes:  Avalonia.Controls.WebView 12.0.1 单独 pin（与 Avalonia 12.1 不同步）。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class VersionPinDemo : UserControl
{
    public VersionPinDemo()
    {
        InitializeComponent();
    }
}
