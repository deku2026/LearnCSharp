// Doc:    第10部分-阶段23-无障碍国际化与文本.md §3
// Title:  平台读屏桥接
// Stage:  Stage23
// Accept: 列出三平台桥接点
// Notes:  Windows UIA / macOS AXUIElement / Linux AT-SPI；Avalonia 自动桥接。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

public partial class PlatformBridgeDemo : UserControl
{
    public PlatformBridgeDemo()
    {
        InitializeComponent();
    }
}
