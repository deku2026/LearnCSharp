// Doc:    第9部分-阶段21-应用架构与工程结构.md §7
// Title:  导航与对话框服务
// Stage:  Stage21
// Accept: VM 零 Window 类型引用
// Notes:  VM 只调 INavigationService/IDialogService，禁止 new Window().ShowDialog。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class NavigationDialogDemo : UserControl
{
    public NavigationDialogDemo()
    {
        InitializeComponent();
    }

    private void SimulateConfirm_Click(object? sender, RoutedEventArgs e)
    {
        if (NavLog is not null)
        {
            NavLog.Text = "VM 代码：if (!await _dialogs.ConfirmAsync(\"删除\", \"确定删除？\")) return;\n→ 抽象返回 true，VM 继续执行删除逻辑\n→ VM 全程无 Window 类型引用";
        }
    }

    private void SimulateNavigate_Click(object? sender, RoutedEventArgs e)
    {
        if (NavLog is not null)
        {
            NavLog.Text = "VM 代码：await _nav.NavigateAsync(\"notes/42\", ct);\n→ INavigationService 实现负责 Frame/Page/history\n→ CanGoBack 由历史栈驱动，不暴露具体 Frame 类型";
        }
    }
}
