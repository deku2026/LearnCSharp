// Doc:    第11部分-阶段29-必读子系统源码精读.md §11
// Title:  Selector / Style Matching 源码精读
// Stage:  Stage29
// Accept: 画出选择器匹配顺序
// Notes:  反射 Selector / IStyle；展示匹配入口与 Combinator。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Styling;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class SelectorSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public SelectorSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type selector = typeof(Selector);
        Type istyle = typeof(IStyle);
        Entries.Add(new ReflectionEntry("Selector (abstract)", selector.FullName ?? selector.Name));
        Entries.Add(new ReflectionEntry("IStyle", istyle.FullName ?? istyle.Name));
        foreach (MethodInfo m in selector.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Take(6))
        {
            Entries.Add(new ReflectionEntry("  方法", m.ReturnType.Name + " " + m.Name + FormatArgs(m)));
        }

        Entries.Add(new ReflectionEntry("Combinators", ".Class / #Name / ^PseudoClass / :is(T) / > child / + next"));
        Entries.Add(new ReflectionEntry("Type Selector", "Button — 类型匹配"));
        Entries.Add(new ReflectionEntry("Class Selector", ".primary — AttachedStyle"));
        Entries.Add(new ReflectionEntry("Name Selector", "#root — x:Name"));
        Entries.Add(new ReflectionEntry("PseudoClass", ":pointerover — 状态选择器"));
        Entries.Add(new ReflectionEntry("匹配序", "Styles 按声明序匹配；后者覆盖前者同优先级"));
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name)) + ")";
    }
}
