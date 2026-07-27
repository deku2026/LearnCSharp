// Doc:    第8部分-Capstone3-全平台Avalonia应用.md §3.1
// Title:  路由策略（桌面）
// Stage:  Capstone3
// Accept: 桌面三平台统一路由
// Notes:  桌面用 ContentControl 切换 View；移动 ShellContent/NavigationPage 见 roadmap。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform.Pages;

public partial class RoutingStrategyDemo : UserControl
{
    public RoutingStrategyDemo()
    {
        InitializeComponent();
    }
}
