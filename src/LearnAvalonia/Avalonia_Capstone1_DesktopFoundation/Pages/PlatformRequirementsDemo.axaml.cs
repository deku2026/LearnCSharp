// Doc:    第4部分-Capstone1-生产级桌面基础应用.md §3
// Title:  平台要求与差异
// Stage:  Capstone1
// Accept: Win+macOS+Linux 至少各一证据
// Notes:  Windows Tier 允许版本；macOS 注意签名仅开发即可；Linux X11 路径；Wayland 失败不阻通过。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone1_DesktopFoundation.Pages;

public partial class PlatformRequirementsDemo : UserControl
{
    public PlatformRequirementsDemo()
    {
        InitializeComponent();
    }
}
