// Doc:    第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §7
// Title:  原生宿主与互操作场景
// Stage:  Stage28
// Accept: 互操作场景
// Notes:  WinForms 嵌 Avalonia；HwndHost 等价；NativeControlHost。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage28_WpfMigration.Pages;

public partial class NativeHostInteropDemo : UserControl
{
    public NativeHostInteropDemo()
    {
        InitializeComponent();
    }
}
