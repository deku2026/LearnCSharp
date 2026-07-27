// Doc:    第11部分-阶段31-渲染专家档.md §7
// Title:  Metal
// Stage:  Stage31
// Accept: macOS Metal 角色
// Notes:  macOS 后端通过 Metal 直接 GPU；不通过 OpenGL。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class MetalDemo : UserControl
{
    public MetalDemo()
    {
        InitializeComponent();
    }
}
