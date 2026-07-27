// Doc:    第4部分-阶段9-数据绑定与MVVM.md §I
// Title:  串联清单
// Stage:  Stage09
// Accept: 拼装 MainVM+服务+单测心智
// Notes:  MainViewModel(IDialogService, IDataService) + ObservableCollection + async + 单测 Fake。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class IntegrationChecklistDemo : UserControl
{
    public IntegrationChecklistDemo()
    {
        InitializeComponent();
    }
}
