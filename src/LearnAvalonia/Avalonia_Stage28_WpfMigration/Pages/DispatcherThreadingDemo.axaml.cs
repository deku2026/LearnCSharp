// Doc:    第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §3
// Title:  Dispatcher 与线程
// Stage:  Stage28
// Accept: Dispatcher 差异
// Notes:  WPF Dispatcher.CurrentDispatcher；Avalonia Dispatcher.UIThread；线程模型不同。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage28_WpfMigration.Pages;

public partial class DispatcherThreadingDemo : UserControl
{
    public DispatcherThreadingDemo()
    {
        InitializeComponent();
    }
}
