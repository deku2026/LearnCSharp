// Doc:    第4部分-阶段8-样式主题与ControlTheme.md §I
// Title:  平台差
// Stage:  Stage08
// Accept: 区分桌面/移动/Browser 主题能力
// Notes:  Linux 各 DE 的系统主题信号可能不同。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage08_StylesThemes.Pages;

public partial class PlatformDiffDemo : UserControl
{
    public PlatformDiffDemo()
    {
        InitializeComponent();
    }
}
