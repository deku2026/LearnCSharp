// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.1
// Title:  Lifetime 与多窗口
// Stage:  Stage10
// Accept: 区分 Show / ShowDialog 与 Owner
// Notes:  Show(modeless)/ShowDialog(modal)/Owner 与 ShutdownMode 三选项。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class LifetimeMultiWindowDemo : UserControl
{
    public LifetimeMultiWindowDemo()
    {
        InitializeComponent();
    }
}
