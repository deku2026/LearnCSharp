// Doc:    第10部分-阶段25-性能与诊断.md §5
// Title:  GC 与内存
// Stage:  Stage25
// Accept: GC.GetTotalMemory 可演示
// Notes:  GC.GetTotalMemory(false) 当前托管内存；GC.Collect 强制；避免 LOH 大对象。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage25_Performance.Pages;

public partial class GcMemoryDemo : UserControl
{
    public GcMemoryDemo()
    {
        InitializeComponent();
    }

    private void MeasureButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ResultText is null)
        {
            return;
        }

        long before = GC.GetTotalMemory(false);
        List<string> list = new System.Collections.Generic.List<string>(1000);
        for (int i = 0; i < 1000; i++)
        {
            list.Add("item " + i);
        }

        long after = GC.GetTotalMemory(false);
        long beforeCollect = GC.GetTotalMemory(true);
        long delta = after - before;
        ResultText.Text = "分配 1000 个 string\n→ 分配前托管内存=" + before + " bytes\n→ 分配后托管内存=" + after + " bytes\n→ 增量=" + delta + " bytes\n→ GC.Collect 后=" + beforeCollect + " bytes\n→ Gen0=" + GC.CollectionCount(0) + " Gen1=" + GC.CollectionCount(1) + " Gen2=" + GC.CollectionCount(2);
    }
}
