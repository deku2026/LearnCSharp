// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §2.3
// Title:  TableView（12.1 新）
// Stage:  Stage15
// Accept: TableView 列定义/虚拟化
// Notes:  Avalonia 12.1 核心包，无额外 NuGet；行虚拟化、列不虚拟化；只读表首选。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class TableViewDemo : UserControl
{
    public ObservableCollection<LogRow> Logs { get; } = [];

    public TableViewDemo()
    {
        for (int i = 0; i < 1000; i++)
        {
            Logs.Add(new LogRow(i + 1, "INFO", "操作 #" + (i + 1)));
        }

        InitializeComponent();
        DataContext = this;
    }
}

public sealed record LogRow(int Id, string Level, string Message);
