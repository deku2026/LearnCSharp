// Doc:    第11部分-阶段29-必读子系统源码精读.md §8
// Title:  Routed Events 源码精读
// Stage:  Stage29
// Accept: 区分 tunnel/bubble/直接
// Notes:  RoutedEventArgs + Routing 枚举（Avalonia.Interactivity）；展示注册入口。
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class RoutedEventSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public RoutedEventSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type args = typeof(RoutedEventArgs);
        Type re = typeof(RoutedEvent);
        Entries.Add(new ReflectionEntry("RoutedEventArgs", args.FullName ?? args.Name));
        Entries.Add(new ReflectionEntry("RoutedEvent", re.FullName ?? re.Name));
        Entries.Add(new ReflectionEntry("  Routing.Bubble", "由 child 向 root 上升"));
        Entries.Add(new ReflectionEntry("  Routing.Tunnel", "由 root 向 child 下沉（Preview）"));
        Entries.Add(new ReflectionEntry("  Routing.Direct", "只触发自己"));
        Entries.Add(new ReflectionEntry("注册入口", "RoutedEvent.Register<TOwner>(name, routing, type)"));
        Entries.Add(new ReflectionEntry("订阅", "interactive.AddHandler(RoutedEvent, handler)"));
        Entries.Add(new ReflectionEntry("e.Handled", "true 截断后续链"));
        Entries.Add(new ReflectionEntry("RoutingStrategy", "12 中类型为 Routing 枚举 — 字段同上"));
    }
}
