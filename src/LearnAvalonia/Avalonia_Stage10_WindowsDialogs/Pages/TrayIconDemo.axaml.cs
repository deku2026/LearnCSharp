// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.5
// Title:  TrayIcon 必须配 NativeMenu
// Stage:  Stage10
// Accept: 托盘菜单用 NativeMenu 不是 Menu
// Notes:  TrayIcon.Menu 必须为 NativeMenu；OnExplicitShutdown 托盘常驻。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class TrayIconDemo : UserControl
{
    public TrayIconDemo()
    {
        InitializeComponent();
    }
}
