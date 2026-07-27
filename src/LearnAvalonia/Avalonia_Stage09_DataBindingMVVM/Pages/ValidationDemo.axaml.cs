// Doc:    第4部分-阶段9-数据绑定与MVVM.md §D1
// Title:  校验与 DataAnnotations
// Stage:  Stage09
// Accept: 使用 ObservableValidator 与 INotifyDataErrorInfo
// Notes:  12 DataAnnotations 绑定插件默认关闭；推荐 Toolkit ObservableValidator + NotifyDataErrorInfo。
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class ValidationDemo : Avalonia.Controls.UserControl
{
    public ValidationDemo()
    {
        InitializeComponent();
        DataContext = new ValidationVm();
    }
}

public sealed partial class ValidationVm : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "名称必填")]
    [MinLength(2, ErrorMessage = "至少 2 个字符")]
    private string _name = "";

    [RelayCommand]
    private void Save()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            return;
        }
    }
}
