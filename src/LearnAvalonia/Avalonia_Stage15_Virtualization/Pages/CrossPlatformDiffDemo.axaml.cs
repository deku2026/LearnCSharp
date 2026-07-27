// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §3
// Title:  跨平台差异
// Stage:  Stage15
// Accept: 虚拟化在三平台差异表
// Notes:  Win/macOS/Linux 均走 Skia；触屏滚动行为差异；触屏 DPI 影响 container count。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class CrossPlatformDiffDemo : UserControl
{
    public CrossPlatformDiffDemo()
    {
        InitializeComponent();
    }
}
