// Doc:    第4部分-阶段9-数据绑定与MVVM.md §C1
// Title:  INPC·集合·命令
// Stage:  Stage09
// Accept: 使用 Toolkit ObservableProperty 与 RelayCommand
// Notes:  生产用 Toolkit 源生成；ObservableCollection 跨线程必须 UI 封送；async Task 命令禁 async void。
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class InpcCollectionCommandDemo : Avalonia.Controls.UserControl
{
    public InpcCollectionCommandDemo()
    {
        InitializeComponent();
        DataContext = new InpcCollectionCommandVm();
    }
}

public sealed partial class InpcCollectionCommandVm : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _name = "";

    public ObservableCollection<string> People { get; } = ["Alice", "Bob"];

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        People.Add(Name);
        Name = "";
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

    [RelayCommand]
    private async Task LoadAsync(CancellationToken ct)
    {
        await Task.Delay(500, ct);
        People.Add("Loaded Item");
    }
}
