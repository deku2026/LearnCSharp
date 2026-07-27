// Doc:    第11部分-阶段29-必读子系统源码精读.md §13
// Title:  TopLevel / Window / PresentationSource 源码精读
// Stage:  Stage29
// Accept: 列出 TopLevel 平台入口
// Notes:  反射 TopLevel / Window / PresentationSource；展示平台桥接点。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class TopLevelSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public TopLevelSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type tl = typeof(TopLevel);
        Type window = typeof(Window);
        Entries.Add(new ReflectionEntry("TopLevel", tl.FullName ?? tl.Name));
        Entries.Add(new ReflectionEntry("Window", window.FullName ?? window.Name));
        Entries.Add(new ReflectionEntry("基类", window.BaseType?.FullName ?? "(none)"));
        Entries.Add(new ReflectionEntry("GetTopLevel", "static TopLevel.GetTopLevel(visual) — 正确入口"));
        foreach (PropertyInfo p in tl.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Take(8))
        {
            Entries.Add(new ReflectionEntry("  属性", p.PropertyType.Name + " " + p.Name));
        }

        Entries.Add(new ReflectionEntry("Renderer", "TopLevel.Renderer — 触发 Render"));
        Entries.Add(new ReflectionEntry("PlatformImpl", "ITopLevelImpl — 平台后端实现"));
        Entries.Add(new ReflectionEntry("ClientSize / FrameSize", "TopLevel.ClientSize — 不含 chrome"));
    }
}
