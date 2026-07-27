// Doc:    第4部分-阶段6-布局系统.md §D3
// Title:  Grid
// Stage:  Stage06
// Accept: 使用 Auto/*/固定 列定义
// Notes:  嵌套多层 * Grid 每次内容变化都可能连锁重测。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class GridPanelDemo : UserControl
{
    public GridPanelDemo()
    {
        InitializeComponent();
    }
}
