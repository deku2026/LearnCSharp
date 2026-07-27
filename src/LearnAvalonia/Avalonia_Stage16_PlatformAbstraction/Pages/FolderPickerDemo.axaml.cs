// Doc:    第7部分-阶段16-统一平台抽象.md §2.4
// Title:  文件夹选择器
// Stage:  Stage16
// Accept: OpenFolderPickerAsync
// Notes:  FolderPickerOpenOptions；返回 IStorageFolder；同样判空。
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class FolderPickerDemo : UserControl
{
    public ObservableCollection<string> Picked { get; } = [];

    public FolderPickerDemo()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void OnPickFolder(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IStorageProvider? sp = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (sp is null)
        {
            return;
        }

        IReadOnlyList<IStorageFolder> folders = await sp.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择一个文件夹",
            AllowMultiple = false,
        });

        foreach (IStorageFolder folder in folders)
        {
            Picked.Add(folder.Name + " — " + folder.Path);
        }
    }
}
