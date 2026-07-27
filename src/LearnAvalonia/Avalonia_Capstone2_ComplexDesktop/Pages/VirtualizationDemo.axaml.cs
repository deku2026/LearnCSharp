// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §2
// Title:  大数据虚拟化
// Stage:  Capstone2
// Accept: 万级条目可滚动
// Notes:  数据源≥10000；Avalonia 虚拟化；过滤搜索防抖；勿在 GetHashCode/Equals 做重 IO。
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class VirtualizationDemo : UserControl
{
    public ObservableCollection<ItemVm> Items { get; } = new();
    private Stopwatch? _sw;

    public VirtualizationDemo()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void GenerateButton_Click(object? sender, RoutedEventArgs e)
    {
        _sw = Stopwatch.StartNew();
        Items.Clear();
        // 生成 10,000 行假数据（附录 A 模式）。
        for (int i = 0; i < 10_000; i++)
        {
            Items.Add(new ItemVm(i, $"条目 {i:D5}", i % 7 == 0 ? "重要" : "普通"));
        }
        _sw.Stop();
        if (VirtLog is not null)
        {
            VirtLog.Text = $"已生成 {Items.Count:N0} 行（耗时 {_sw.ElapsedMilliseconds} ms）。\nListBox 虚拟化自动只渲染可见项；滚动应流畅不阻塞。\n禁止：一次性生成 1 万复杂可视化控件无虚拟化。";
        }
    }

    private void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        Items.Clear();
        if (VirtLog is not null)
        {
            VirtLog.Text = "已清空。";
        }
    }

    public sealed record ItemVm(int Id, string Title, string Tag);
}
