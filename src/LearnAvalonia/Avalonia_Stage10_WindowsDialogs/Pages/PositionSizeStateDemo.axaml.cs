// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.2
// Title:  位置、尺寸、状态
// Stage:  Stage10
// Accept: WindowState 是 DirectProperty 不能用 Style
// Notes:  WindowState 是 DirectProperty；不能靠 Style Setter。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class PositionSizeStateDemo : UserControl
{
    public PositionSizeStateDemo()
    {
        InitializeComponent();
    }
}
