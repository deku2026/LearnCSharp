// Doc:    第7部分-阶段16-统一平台抽象.md §2.2
// Title:  最小文件选择器
// Stage:  Stage16
// Accept: StorageProvider.OpenFilePickerAsync
// Notes:  TopLevel.GetTopLevel(this)?.StorageProvider；Options.Title/FileTypeFilter。
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class FilePickerDemo : UserControl
{
    public ObservableCollection<string> Picked { get; } = [];

    public FilePickerDemo()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void OnPickFile(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IStorageProvider? sp = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (sp is null)
        {
            return;
        }

        IReadOnlyList<IStorageFile> files = await sp.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择一个文件",
            AllowMultiple = false,
        });

        foreach (IStorageFile file in files)
        {
            Picked.Add(file.Name + " — " + file.Path);
        }
    }
}
