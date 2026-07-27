// Doc:    第4部分-阶段6-布局系统.md §A1.2
// Title:  无限可用空间（Infinity）
// Stage:  Stage06
// Accept: 说明 infinity measure 的陷阱
// Notes:  StackPanel 主轴 / ScrollViewer 内容 / Canvas 子项 measure 时常收到无限维度。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class InfinityMeasureDemo : UserControl
{
    public InfinityMeasureDemo()
    {
        InitializeComponent();
    }
}
