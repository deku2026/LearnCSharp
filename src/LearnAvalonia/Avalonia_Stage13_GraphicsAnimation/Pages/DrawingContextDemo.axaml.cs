// Doc:    第5部分-阶段13-图形动画与效果.md §2.3
// Title:  DrawingContext（即时模式）
// Stage:  Stage13
// Accept: AffectsRender + Render 纪律
// Notes:  Control.Render + AffectsRender；Render 内少分配。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage13_GraphicsAnimation.Pages;

public partial class DrawingContextDemo : UserControl
{
    public DrawingContextDemo()
    {
        InitializeComponent();
    }
}
