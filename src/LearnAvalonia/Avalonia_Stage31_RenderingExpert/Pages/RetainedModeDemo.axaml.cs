// Doc:    第11部分-阶段31-渲染专家档.md §1
// Title:  Retained-mode 概念
// Stage:  Stage31
// Accept: 解释 retained 与对开发者含义
// Notes:  Avalonia 是 retained-mode：维护可视化树，Render 仅在失效后触发。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class RetainedModeDemo : UserControl
{
    public RetainedModeDemo()
    {
        InitializeComponent();
    }
}
