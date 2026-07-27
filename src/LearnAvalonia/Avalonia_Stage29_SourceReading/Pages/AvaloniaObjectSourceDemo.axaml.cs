// Doc:    第11部分-阶段29-必读子系统源码精读.md §3
// Title:  AvaloniaObject 源码精读
// Stage:  Stage29
// Accept: 说出 AO 的属性优先级入口
// Notes:  反射 AvaloniaObject 的 GetValue/SetValue/SetCurrentValue/ClrProperty 注册入口。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class AvaloniaObjectSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public AvaloniaObjectSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type t = typeof(AvaloniaObject);
        Entries.Add(new ReflectionEntry("类型", t.FullName ?? t.Name));
        Entries.Add(new ReflectionEntry("基类", t.BaseType?.FullName ?? "(none)"));
        Entries.Add(new ReflectionEntry("接口数", t.GetInterfaces().Length.ToString()));
        Entries.Add(new ReflectionEntry("属性优先级入口", "SetValue / SetCurrentValue / ClearValue / SetValue<T>"));
        foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(x => x.Name is "GetValue" or "SetValue" or "SetCurrentValue" or "ClearValue" or "IsSet")
                     .Take(8))
        {
            Entries.Add(new ReflectionEntry("  方法", m.ReturnType.Name + " " + m.Name + FormatArgs(m)));
        }

        Entries.Add(new ReflectionEntry("OnPropertyChanged", "virtual — 子类/属性系统钩子"));
        Entries.Add(new ReflectionEntry("绑定入口", "this[!Property] / BindingOperations"));
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name)) + ")";
    }
}
