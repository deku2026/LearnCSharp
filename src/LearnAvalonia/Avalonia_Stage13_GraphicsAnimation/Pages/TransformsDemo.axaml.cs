// Doc:    第5部分-阶段13-图形动画与效果.md §2.6
// Title:  Transforms
// Stage:  Stage13
// Accept: RenderTransform 不影响布局
// Notes:  RenderTransform 绘制时变换；动画用它避免 relayout。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage13_GraphicsAnimation.Pages;

public partial class TransformsDemo : UserControl
{
    public TransformsDemo()
    {
        InitializeComponent();
    }
}
