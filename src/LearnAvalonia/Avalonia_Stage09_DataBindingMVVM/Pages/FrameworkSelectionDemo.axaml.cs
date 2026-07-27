// Doc:    第4部分-阶段9-数据绑定与MVVM.md §G1
// Title:  MVVM 框架选型表
// Stage:  Stage09
// Accept: 默认方案口述：Toolkit+compiled+DI
// Notes:  CommunityToolkit.Mvvm 8.4.2 默认；ReactiveUI.Avalonia 12.0.3 可选；Prism pre；ViewLocator AOT 不友好。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class FrameworkSelectionDemo : UserControl
{
    public FrameworkSelectionDemo()
    {
        InitializeComponent();
    }
}
