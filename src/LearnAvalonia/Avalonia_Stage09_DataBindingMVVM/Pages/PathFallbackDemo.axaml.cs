// Doc:    第4部分-阶段9-数据绑定与MVVM.md §A3
// Title:  路径与 Fallback/TargetNull
// Stage:  Stage09
// Accept: 使用 FallbackValue 与 TargetNullValue
// Notes:  TargetNullValue 源值为 null；FallbackValue 绑定失败/不可用时回退。
using CommunityToolkit.Mvvm.ComponentModel;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class PathFallbackDemo : Avalonia.Controls.UserControl
{
    public PathFallbackDemo()
    {
        InitializeComponent();
        DataContext = new PathFallbackVm();
    }
}

public sealed partial class PathFallbackVm : ObservableObject
{
    [ObservableProperty]
    private string? _nickname;
}
