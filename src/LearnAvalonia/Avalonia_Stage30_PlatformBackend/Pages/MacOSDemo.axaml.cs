// Doc:    第11部分-阶段30-平台后端选读.md §4
// Title:  macOS (Avalonia.Native / AppKit)
// Stage:  Stage30
// Accept: 列出 AppKit 桥
// Notes:  macOS 后端通过 libAvaloniaNative.dylib 与 AppKit 桥接；窗口走 NSWindow。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage30_PlatformBackend.Pages;

public partial class MacOSDemo : UserControl
{
    public MacOSDemo()
    {
        InitializeComponent();
    }
}
