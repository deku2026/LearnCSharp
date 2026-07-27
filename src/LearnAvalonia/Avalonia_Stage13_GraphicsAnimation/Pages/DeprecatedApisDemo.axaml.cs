// Doc:    第5部分-阶段13-图形动画与效果.md §5
// Title:  过时与禁止
// Stage:  Stage13
// Accept: 禁 Render 每帧 new/Infinite 不治理
// Notes:  Render 内缓存 Pen/Brush；Infinite 动画需治理。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage13_GraphicsAnimation.Pages;

public partial class DeprecatedApisDemo : UserControl
{
    public DeprecatedApisDemo()
    {
        InitializeComponent();
    }
}
