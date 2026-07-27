// Doc:    第10部分-阶段25-性能与诊断.md §1
// Title:  启动：冷启动与热启动
// Stage:  Stage25
// Accept: Stopwatch 测启动可演示
// Notes:  Stopwatch 分段计时；冷启动含 JIT+加载；热启动已驻留。
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage25_Performance.Pages;

public partial class StartupPerfDemo : UserControl
{
    public StartupPerfDemo()
    {
        InitializeComponent();
    }

    private void MeasureButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ResultText is null)
        {
            return;
        }

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            TextBlock block = new() { Text = "item " + i };
            block.Measure(new Avalonia.Size(200, 30));
        }

        sw.Stop();
        ResultText.Text = "构造 + Measure 1000 个 TextBlock\n→ 耗时=" + sw.ElapsedMilliseconds + " ms\n→ 平均=" + (sw.Elapsed.TotalMilliseconds / 1000.0).ToString("F3") + " ms/项\n→ 注意：冷启动含 JIT 首次编译；热启动数据更准";
    }
}
