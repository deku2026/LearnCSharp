// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.7
// Title:  窗口状态持久化
// Stage:  Stage10
// Accept: Closing 存 Opened 应用 RestoreBounds
// Notes:  Closing 写、Opened 读；不在 Measure 改 Position。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class WindowStatePersistDemo : UserControl
{
    public WindowStatePersistDemo()
    {
        InitializeComponent();
    }
}
