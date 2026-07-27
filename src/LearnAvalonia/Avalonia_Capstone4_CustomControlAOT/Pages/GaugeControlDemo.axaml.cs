// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.1
// Title:  GaugeControl 控件运行
// Stage:  Capstone4
// Accept: 运行 GaugeControl + Value 变更
// Notes:  滑块改 Value，GaugeControl 重绘；ValueChanged 事件记日志。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class GaugeControlDemo : UserControl
{
    public GaugeControlDemo()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void ValueSlider_ValueChanged(object? sender, RoutedEventArgs e)
    {
        if (Gauge is not null && ValueSlider is not null && GaugeLog is not null)
        {
            Gauge.Value = ValueSlider.Value;
            GaugeLog.Text = $"Value = {Gauge.Value:0} / {Gauge.Maximum:0}";
        }
    }
}
