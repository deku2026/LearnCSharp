// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.3
// Title:  路由事件 ValueChanged
// Stage:  Capstone4
// Accept: 订阅 routed event
// Notes:  RoutedEvent.Register + AddHandler；本 demo 在 Loaded 时挂 handler。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class RoutedEventDemo : UserControl
{
    public RoutedEventDemo()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (EvtGauge is not null)
        {
            EvtGauge.AddHandler(GaugeControl.ValueChangedEvent, OnGaugeValueChanged);
        }
    }

    private void OnGaugeValueChanged(object? sender, ValueChangedEventArgs e)
    {
        if (EvtLog is not null)
        {
            EvtLog.Text = $"ValueChanged: OldValue={e.OldValue:0} → NewValue={e.NewValue:0}";
        }
    }

    private void IncButton_Click(object? sender, RoutedEventArgs e)
    {
        if (EvtGauge is not null)
        {
            EvtGauge.Value = Math.Min(EvtGauge.Maximum, EvtGauge.Value + 10);
        }
    }
}
