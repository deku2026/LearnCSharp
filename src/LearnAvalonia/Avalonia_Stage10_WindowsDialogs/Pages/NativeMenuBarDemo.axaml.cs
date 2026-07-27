// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.6
// Title:  NativeMenuBar 与 macOS Dock
// Stage:  Stage10
// Accept: NativeMenu.Menu 用法与跨平台降级
// Notes:  macOS 顶部菜单栏融合；Windows 走降级方案。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class NativeMenuBarDemo : UserControl
{
    public NativeMenuBarDemo()
    {
        InitializeComponent();
    }
}
