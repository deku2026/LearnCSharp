// Doc:    第4部分-阶段7-控件体系.md §I
// Title:  平台差异摘要
// Stage:  Stage07
// Accept: 区分桌面/移动/Browser 能力
// Notes:  多 Window 桌面有；移动主单视图；Browser 无。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage07_ControlSystem.Pages;

public partial class PlatformDiffDemo : UserControl
{
    public PlatformDiffDemo()
    {
        InitializeComponent();
    }
}
