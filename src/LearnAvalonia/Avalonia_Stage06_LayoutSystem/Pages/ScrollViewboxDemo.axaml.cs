// Doc:    第4部分-阶段6-布局系统.md §E1
// Title:  Viewbox 与 ScrollViewer
// Stage:  Stage06
// Accept: 解释 ScrollViewer 与 Viewbox 的 infinity measure
// Notes:  Viewbox 缩放视觉；ScrollViewer 内容在滚动方向常以 infinity measure。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class ScrollViewboxDemo : UserControl
{
    public ScrollViewboxDemo()
    {
        InitializeComponent();
    }
}
