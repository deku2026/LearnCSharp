// Doc:    第11部分-阶段31-渲染专家档.md §14-15
// Title:  Overdraw / 关键字限制
// Stage:  Stage31
// Accept: 解释 overdraw 红线
// Notes:  Overdraw 检测：每个像素被绘制次数；过度 = 性能红线。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class OverdrawDemo : UserControl
{
    public OverdrawDemo()
    {
        InitializeComponent();
    }
}
