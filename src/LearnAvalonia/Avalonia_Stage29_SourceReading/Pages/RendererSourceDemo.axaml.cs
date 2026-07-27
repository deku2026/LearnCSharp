// Doc:    第11部分-阶段29-必读子系统源码精读.md §16
// Title:  Renderer / Compositor / Skia 源码精读
// Stage:  Stage29
// Accept: 画出 Render 调用链
// Notes:  IRenderRoot 在 Avalonia.Base；IRenderer/Compositor 内部类型；用字符串描述调用链。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class RendererSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public RendererSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type comp = typeof(Compositor);
        Entries.Add(new ReflectionEntry("Compositor", comp.FullName ?? comp.Name));
        Entries.Add(new ReflectionEntry("IRenderRoot", "Avalonia.Rendering.IRenderRoot — 根接口"));
        Entries.Add(new ReflectionEntry("IRenderer", "Avalonia.Rendering.IRenderer — internal 渲染器"));
        foreach (PropertyInfo p in comp.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Take(6))
        {
            Entries.Add(new ReflectionEntry("  Compositor 属性", p.PropertyType.Name + " " + p.Name));
        }

        Entries.Add(new ReflectionEntry("Render(DrawingContext)", "Control.Render — 自绘入口"));
        Entries.Add(new ReflectionEntry("InvalidateVisual", "标记 dirty → 加入 dirty region"));
        Entries.Add(new ReflectionEntry("Compositor.RequestCompositionUpdate", "提交到合成器"));
        Entries.Add(new ReflectionEntry("Skia", "DrawRect/DrawText → Skia API"));
        Entries.Add(new ReflectionEntry("Render Root", "TopLevel.Renderer / IRenderRoot"));
        Entries.Add(new ReflectionEntry("线程", "Render 在 UI 线程或独立 Render 线程（platform）"));
    }
}
