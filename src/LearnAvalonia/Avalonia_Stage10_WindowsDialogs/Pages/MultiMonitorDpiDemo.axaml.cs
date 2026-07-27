// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.8
// Title:  多显示器、DPI、拖拽区域
// Stage:  Stage10
// Accept: BeginMoveDrag 与多屏 Scaling
// Notes:  TopLevel.Screens 枚举 + BeginMoveDrag/BeginResizeDrag。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class MultiMonitorDpiDemo : UserControl
{
    public MultiMonitorDpiDemo()
    {
        InitializeComponent();
    }
}
