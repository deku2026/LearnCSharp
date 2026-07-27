// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §2.5
// Title:  Environment 与初始化
// Stage:  Stage17
// Accept: WebViewEnvironment 平台配置
// Notes:  WebViewEnvironment 提供 userDataFolder / browserExecutableFolder 等配置（Win WebView2）。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class EnvironmentDemo : UserControl
{
    public EnvironmentDemo()
    {
        InitializeComponent();
    }
}
