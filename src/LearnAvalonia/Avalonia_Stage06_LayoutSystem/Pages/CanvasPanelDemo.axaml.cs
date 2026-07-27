// Doc:    第4部分-阶段6-布局系统.md §D6
// Title:  Canvas
// Stage:  Stage06
// Accept: 使用 Canvas.Left/Top 绝对定位
// Notes:  Canvas 子项 Measure 时常收到 infinity；自身尺寸需显式或由父约束。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class CanvasPanelDemo : UserControl
{
    public CanvasPanelDemo()
    {
        InitializeComponent();
    }
}
