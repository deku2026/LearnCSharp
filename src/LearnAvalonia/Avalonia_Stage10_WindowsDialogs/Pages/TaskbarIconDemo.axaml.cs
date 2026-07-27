// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.14
// Title:  任务栏与窗口图标
// Stage:  Stage10
// Accept: Icon/ShowInTaskbar/未保存 * 标记
// Notes:  WindowIcon 资源 + ShowInTaskbar + 未保存标题前缀。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class TaskbarIconDemo : UserControl
{
    public TaskbarIconDemo()
    {
        InitializeComponent();
    }
}
