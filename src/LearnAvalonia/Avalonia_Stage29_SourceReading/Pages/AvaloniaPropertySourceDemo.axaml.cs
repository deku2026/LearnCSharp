// Doc:    第11部分-阶段29-必读子系统源码精读.md §4
// Title:  AvaloniaProperty 源码精读
// Stage:  Stage29
// Accept: 区分 Styled/Direct/Attached
// Notes:  反射 AvaloniaProperty 抽象基类与 StyledProperty/DirectProperty 注册入口。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class AvaloniaPropertySourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public AvaloniaPropertySourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type baseT = typeof(AvaloniaProperty);
        Type styled = typeof(StyledProperty<>);
        Type direct = typeof(DirectPropertyBase<>);
        Type attached = typeof(AttachedProperty<>);
        Entries.Add(new ReflectionEntry("AvaloniaProperty", baseT.FullName ?? baseT.Name));
        Entries.Add(new ReflectionEntry("StyledProperty<T>", styled.Name + " — 注册主路径 Register<TOwner,T>"));
        Entries.Add(new ReflectionEntry("DirectPropertyBase<T>", direct.Name + " — Direct 走 getter/setter 委托"));
        Entries.Add(new ReflectionEntry("AttachedProperty<T>", attached.Name + " — Attached 寄主到别的类型"));
        Entries.Add(new ReflectionEntry("Register", "静态入口；返回类型化 property"));
        foreach (MethodInfo m in baseT.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Take(6))
        {
            Entries.Add(new ReflectionEntry("  静态方法", m.ReturnType.Name + " " + m.Name + FormatArgs(m)));
        }

        Entries.Add(new ReflectionEntry("注册参数", "name / defaultValue / inherits / defaultBindingMode / validate / coerce"));
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name)) + ")";
    }
}
