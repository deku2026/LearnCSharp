// Doc:    第10部分-阶段25-性能与诊断.md §3
// Title:  布局次数与树规模
// Stage:  Stage25
// Accept: LayoutUpdated 计数器可演示
// Notes:  LayoutUpdated 事件触发次数 = 布局轮次；多次 Invalidate = 性能问题。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage25_Performance.Pages;

public partial class LayoutCountDemo : UserControl
{
    private int _layoutCount;

    public LayoutCountDemo()
    {
        InitializeComponent();
        if (LayoutProbe is not null)
        {
            LayoutProbe.LayoutUpdated += (_, _) => _layoutCount++;
        }
    }

    private void TriggerLayoutButton_Click(object? sender, RoutedEventArgs e)
    {
        if (LayoutProbe is null || CountText is null)
        {
            return;
        }

        _layoutCount = 0;
        LayoutProbe.Width = LayoutProbe.Width == 200 ? 220 : 200;
        LayoutProbe.InvalidateMeasure();
        CountText.Text = "Width 切换 + InvalidateMeasure 后\n→ LayoutUpdated 触发次数=" + _layoutCount + "\n→ 一次视觉变更应只触发 1-2 轮；多次表示级联 Invalidate";
    }
}
