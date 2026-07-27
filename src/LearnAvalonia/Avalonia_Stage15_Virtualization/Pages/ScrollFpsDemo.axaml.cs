// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §2.9
// Title:  滚动 FPS 与内存分析
// Stage:  Stage15
// Accept: 概念性 FPS 测量方法
// Notes:  概念性方法：Composition.Rendering 事件计数；内存用 dotnet-counters。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class ScrollFpsDemo : UserControl
{
    public ScrollFpsDemo()
    {
        InitializeComponent();
    }
}
