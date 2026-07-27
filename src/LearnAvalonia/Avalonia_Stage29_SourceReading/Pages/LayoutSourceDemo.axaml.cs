// Doc:    第11部分-阶段29-必读子系统源码精读.md §9
// Title:  Layout Manager 源码精读
// Stage:  Stage29
// Accept: 描述 Measure/Arrange 队列
// Notes:  Avalonia.Layout 命名空间 Layoutable / ILayoutable（internal 接口）；反射 Layoutable 即可。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Layout;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class LayoutSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public LayoutSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type layoutable = typeof(Layoutable);
        Entries.Add(new ReflectionEntry("Layoutable", layoutable.FullName ?? layoutable.Name));
        Entries.Add(new ReflectionEntry("ILayoutable", "Avalonia.Layout.ILayoutable — 接口"));
        Entries.Add(new ReflectionEntry("LayoutManager", "Avalonia.Layout.LayoutManager — internal 队列"));
        foreach (MethodInfo m in layoutable.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(x => x.Name is "Measure" or "Arrange" or "InvalidateMeasure" or "InvalidateArrange").Take(6))
        {
            Entries.Add(new ReflectionEntry("  方法", m.ReturnType.Name + " " + m.Name + FormatArgs(m)));
        }

        Entries.Add(new ReflectionEntry("MeasureOverride", "virtual Size — 子类重写计算"));
        Entries.Add(new ReflectionEntry("ArrangeOverride", "virtual Size — 子类重写布局"));
        Entries.Add(new ReflectionEntry("队列", "LayoutManager 批量 dispatch，合并多个 Invalidate"));
        Entries.Add(new ReflectionEntry("触发表", "AffectsMeasure / AffectsArrange — 静态构造注册"));
        Entries.Add(new ReflectionEntry("DesiredSize", "Measure 后只读 — Arrange 的输入"));
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name)) + ")";
    }
}
