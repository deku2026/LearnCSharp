// Doc:    第2部分-阶段1-AppBuilder与应用启动.md §11
// Title:  跨平台差异表
// Stage:  Stage01
// Accept: 平台表覆盖 8 类宿主
// Notes:  展示 Win32/macOS/X11/Wayland/Android/iOS/Browser/framebuffer/设计时 9 行对照。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder.Pages;

public partial class PlatformDiffDemo : UserControl
{
    public PlatformDiffDemo()
    {
        InitializeComponent();
    }
}
