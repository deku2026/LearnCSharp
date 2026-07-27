// Doc:    第11部分-阶段30-平台后端选读.md §11
// Title:  Headless (桌面/测试)
// Stage:  Stage30
// Accept: 列出 Headless 用途
// Notes:  Avalonia.Headless 用于单测/CI；不走真实窗口/GPU，渲染到内存位图。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage30_PlatformBackend.Pages;

public partial class HeadlessDemo : UserControl
{
    public HeadlessDemo()
    {
        InitializeComponent();
    }
}
