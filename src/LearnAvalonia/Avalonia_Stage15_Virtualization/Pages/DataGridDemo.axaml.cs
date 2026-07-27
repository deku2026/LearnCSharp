// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §2.4
// Title:  DataGrid（deprecated）
// Stage:  Stage15
// Accept: DataGrid 用法与 deprecated 标注
// Notes:  DataGrid 自 12.1 起标 deprecated；只读用 TableView，编辑考虑 TreeDataGrid（Pro）或自研。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class DataGridDemo : UserControl
{
    public ObservableCollection<PersonRow> People { get; } =
    [
        new("Neil", "Armstrong"),
        new("Buzz", "Lightyear"),
        new("James", "Kirk"),
    ];

    public DataGridDemo()
    {
        InitializeComponent();
        DataContext = this;
    }
}

public sealed record PersonRow(string FirstName, string LastName);
