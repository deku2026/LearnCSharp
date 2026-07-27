// Doc:    第11部分-阶段29-必读子系统源码精读.md §7
// Title:  Visual Tree 源码精读
// Stage:  Stage29
// Accept: 画出 Visual 与 VisualRoot
// Notes:  Avalonia.VisualTree 命名空间下 IVisual（internal）/ Visual / IVisualRoot；展示 VisualRoot 入口。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class VisualTreeSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public VisualTreeSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type visual = typeof(Avalonia.Visual);
        Entries.Add(new ReflectionEntry("Visual (abstract)", visual.FullName ?? visual.Name));
        Entries.Add(new ReflectionEntry("IVisual", "Avalonia.VisualTree.IVisual — 接口"));
        Entries.Add(new ReflectionEntry("IVisualRoot", "Avalonia.VisualTree.IVisualRoot — 根接口"));
        foreach (PropertyInfo p in visual.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Take(8))
        {
            Entries.Add(new ReflectionEntry("  Visual 属性", p.PropertyType.Name + " " + p.Name));
        }

        Entries.Add(new ReflectionEntry("VisualRoot", "IVisual? — 渲染根"));
        Entries.Add(new ReflectionEntry("GetTopLevel", "static TopLevel.GetTopLevel(visual) — 正确取 TopLevel"));
        Entries.Add(new ReflectionEntry("禁止", "visual.GetVisualRoot() as TopLevel — 强转失败"));
        Entries.Add(new ReflectionEntry("VisualTreeExtensions", "GetVisualAncestors / GetVisualDescendants / GetVisualParent"));
    }
}
