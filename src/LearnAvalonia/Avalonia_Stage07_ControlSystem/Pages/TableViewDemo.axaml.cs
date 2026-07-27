// Doc:    第4部分-阶段7-控件体系.md §C6
// Title:  TableView 12.1 只读表首选
// Stage:  Stage07
// Accept: 准确说出 TableView 包与状态
// Notes:  位于 Avalonia.Controls 核心包；自 12.1 起作为轻量只读表；派生自 ListBox 一脉；行虚拟化、列不虚拟化。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage07_ControlSystem.Pages;

public partial class TableViewDemo : UserControl
{
    public ObservableCollection<TableRow> Rows { get; } =
    [
        new("任务 A", "进行中"),
        new("任务 B", "已完成"),
        new("任务 C", "待开始"),
    ];

    public TableViewDemo()
    {
        InitializeComponent();
        DataContext = this;
    }
}

public sealed record TableRow(string Name, string Status);
