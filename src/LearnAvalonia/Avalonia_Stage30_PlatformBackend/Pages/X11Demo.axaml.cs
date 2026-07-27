// Doc:    第11部分-阶段30-平台后端选读.md §5
// Title:  X11 (Linux 默认)
// Stage:  Stage30
// Accept: 列出 X11 入口
// Notes:  X11 后端通过 Xlib + EGL/GLX；窗口走 WindowImpl 与 XIM 输入。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage30_PlatformBackend.Pages;

public partial class X11Demo : UserControl
{
    public X11Demo()
    {
        InitializeComponent();
    }
}
