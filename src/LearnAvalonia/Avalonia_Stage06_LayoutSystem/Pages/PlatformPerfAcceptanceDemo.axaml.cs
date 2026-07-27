// Doc:    第4部分-阶段6-布局系统.md §G
// Title:  平台差 · 12 变化 · 专家 · 验收
// Stage:  Stage06
// Accept: 统计 resize 布局次数与平台差异
// Notes:  嵌套昂贵；虚拟化把节点数从 N 降到视口大小 k。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class PlatformPerfAcceptanceDemo : UserControl
{
    public PlatformPerfAcceptanceDemo()
    {
        InitializeComponent();
    }
}
