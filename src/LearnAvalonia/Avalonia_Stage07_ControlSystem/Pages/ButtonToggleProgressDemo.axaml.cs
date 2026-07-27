// Doc:    第4部分-阶段7-控件体系.md §A6
// Title:  Button 族·开关·进度
// Stage:  Stage07
// Accept: 使用 Command 优于业务 Click
// Notes:  业务用 Command（阶段 9）；Click 留给纯 UI。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage07_ControlSystem.Pages;

public partial class ButtonToggleProgressDemo : UserControl
{
    public ButtonToggleProgressDemo()
    {
        InitializeComponent();
    }
}
