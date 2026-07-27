// Doc:    第11部分-阶段31-渲染专家档.md §8
// Title:  OpenGL / EGL
// Stage:  Stage31
// Accept: 列出 GL 入口
// Notes:  Linux X11 走 GLX/EGL；Windows 走 Wgl/Vulkan。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class OpenGlDemo : UserControl
{
    public OpenGlDemo()
    {
        InitializeComponent();
    }
}
