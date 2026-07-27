// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.12
// Title:  StorageProvider 取代旧 FileDialog
// Stage:  Stage10
// Accept: 用 IStorageProvider 打开/保存
// Notes:  TopLevel.GetTopLevel(this)?.StorageProvider 是唯一入口。
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class StorageProviderDemo : UserControl
{
    public StorageProviderDemo()
    {
        InitializeComponent();
    }

    private static IStorageProvider? Provider(Control c)
        => TopLevel.GetTopLevel(c)?.StorageProvider;

    private async void OpenFilesBtn_Clicked(object? sender, RoutedEventArgs e)
    {
        IStorageProvider? sp = Provider(this);
        if (sp is null) { Result.Text = "StorageProvider 不可用"; return; }
        IReadOnlyList<IStorageFile> files = await sp.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "打开",
            AllowMultiple = true,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("文本") { Patterns = new[] { "*.txt", "*.md" } },
                FilePickerFileTypes.All,
            },
        });
        StringBuilder sb = new StringBuilder();
        sb.Append("选中 ").Append(files.Count).AppendLine(" 个文件：");
        foreach (IStorageFile f in files)
            sb.AppendLine(f.Path.LocalPath);
        Result.Text = sb.ToString();
    }

    private async void SaveBtn_Clicked(object? sender, RoutedEventArgs e)
    {
        IStorageProvider? sp = Provider(this);
        if (sp is null) { Result.Text = "StorageProvider 不可用"; return; }
        IStorageFile? file = await sp.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "另存为",
            SuggestedFileName = "demo",
            DefaultExtension = "txt",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("文本") { Patterns = new[] { "*.txt" } },
            },
        });
        Result.Text = file is null ? "已取消" : file.Path.LocalPath;
    }

    private async void OpenFolderBtn_Clicked(object? sender, RoutedEventArgs e)
    {
        IStorageProvider? sp = Provider(this);
        if (sp is null) { Result.Text = "StorageProvider 不可用"; return; }
        IReadOnlyList<IStorageFolder> folders = await sp.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择文件夹",
            AllowMultiple = false,
        });
        if (folders.Count == 0) { Result.Text = "已取消"; return; }
        Result.Text = folders[0].Path.LocalPath + " (folder selected)";
    }
}
