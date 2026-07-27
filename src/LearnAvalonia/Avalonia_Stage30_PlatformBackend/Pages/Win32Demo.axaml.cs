// Doc:    第11部分-阶段30-平台后端选读.md §2
// Title:  Win32 后端
// Stage:  Stage30
// Accept: 列出 Win32 入口
// Notes:  Win32 走 Hwnd + IMM 输入；UseVulkanDelayDrawing / UseWgl 等 Win32 专属选项。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage30_PlatformBackend.Pages;

public partial class Win32Demo : UserControl
{
    public Win32Demo()
    {
        InitializeComponent();
    }
}
