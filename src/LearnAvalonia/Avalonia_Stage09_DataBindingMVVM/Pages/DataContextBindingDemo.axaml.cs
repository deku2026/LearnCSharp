// Doc:    第4部分-阶段9-数据绑定与MVVM.md §A1
// Title:  DataContext 与绑定模式
// Stage:  Stage09
// Accept: 解释 DataContext 继承与绑定模式
// Notes:  View 尽量只知 VM 类型；VM 不引用具体 Window 类型。
using CommunityToolkit.Mvvm.ComponentModel;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class DataContextBindingDemo : Avalonia.Controls.UserControl
{
    public DataContextBindingDemo()
    {
        InitializeComponent();
        DataContext = new DataContextBindingVm();
    }
}

public sealed partial class DataContextBindingVm : ObservableObject
{
    [ObservableProperty]
    private string _name = "张三";

    [ObservableProperty]
    private int _progress = 60;
}
