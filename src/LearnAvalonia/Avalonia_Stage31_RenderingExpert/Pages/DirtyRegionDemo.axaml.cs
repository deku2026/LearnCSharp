// Doc:    第11部分-阶段31-渲染专家档.md §3
// Title:  Dirty Region / 失效区域
// Stage:  Stage31
// Accept: 解释失效与重绘边界
// Notes:  12.1 后 dirty region 改名 dirty rect；InvalidateVisual 加入 dirty。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class DirtyRegionDemo : UserControl
{
    public DirtyRegionDemo()
    {
        InitializeComponent();
    }
}
