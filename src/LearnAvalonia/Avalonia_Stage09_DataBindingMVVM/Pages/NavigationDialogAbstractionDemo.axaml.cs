// Doc:    第4部分-阶段9-数据绑定与MVVM.md §E4
// Title:  导航/对话框抽象
// Stage:  Stage09
// Accept: VM 只依赖接口不引用 Window
// Notes:  桌面实现用 Window.ShowDialog；移动实现用 Pages；VM 只依赖接口。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class NavigationDialogAbstractionDemo : UserControl
{
    public NavigationDialogAbstractionDemo()
    {
        InitializeComponent();
    }
}
