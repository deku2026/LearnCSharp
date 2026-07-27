// Doc:    第7部分-阶段16-统一平台抽象.md §5
// Title:  Screens（Screen abstract）
// Stage:  Stage16
// Accept: Screen 12 起 abstract
// Notes:  Screen 类 12 起抽象；通过 Screens 获取 AllScreens/Primary。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class ScreensDemo : UserControl
{
    public ScreensDemo()
    {
        InitializeComponent();
    }
}
