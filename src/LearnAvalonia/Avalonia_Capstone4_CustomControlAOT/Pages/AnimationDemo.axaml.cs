// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.7
// Title:  动画（值变更过渡）
// Stage:  Capstone4
// Accept: 用 Avalonia.Animation 做过渡
// Notes:  Value 变化时用 DoubleTransition 让数值平滑过渡。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class AnimationDemo : UserControl
{
    public AnimationDemo()
    {
        InitializeComponent();
    }
}
