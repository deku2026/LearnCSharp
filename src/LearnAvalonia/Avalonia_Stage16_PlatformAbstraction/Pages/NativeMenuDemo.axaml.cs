// Doc:    第7部分-阶段16-统一平台抽象.md §9
// Title:  原生菜单/通知
// Stage:  Stage16
// Accept: NativeMenu vs Menu + 通知
// Notes:  NativeMenu 跨平台（macOS 顶部菜单/Win/Linux Tray）；TrayIcon 必须 NativeMenu。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class NativeMenuDemo : UserControl
{
    public NativeMenuDemo()
    {
        InitializeComponent();
    }
}
