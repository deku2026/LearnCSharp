// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §1
// Title:  架构落位（强制）
// Stage:  Capstone2
// Accept: 插件只引 Abstractions + 组合根在 Desktop host
// Notes:  解决方案目录 + 依赖规则 + UI 架构默认 CommunityToolkit。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class ArchitectureDemo : UserControl
{
    public ArchitectureDemo()
    {
        InitializeComponent();
    }
}
