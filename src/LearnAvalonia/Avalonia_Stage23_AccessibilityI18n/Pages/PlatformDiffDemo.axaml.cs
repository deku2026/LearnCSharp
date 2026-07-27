// Doc:    第10部分-阶段23-无障碍国际化与文本.md §8
// Title:  平台差异汇总
// Stage:  Stage23
// Accept: 差异表覆盖三平台
// Notes:  Windows UIA / macOS AXUIElement / Linux AT-SPI；IME 差异；高对比实现差异。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

public partial class PlatformDiffDemo : UserControl
{
    public PlatformDiffDemo()
    {
        InitializeComponent();
    }
}
