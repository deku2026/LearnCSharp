// Doc:    第11部分-阶段29-必读子系统源码精读.md §12
// Title:  ControlTheme / TemplatedControl 源码精读
// Stage:  Stage29
// Accept: 说出 TemplatedControl 契约
// Notes:  反射 TemplatedControl / ControlTheme / TemplateApi；展示 OnApplyTemplate 入口。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class ControlThemeSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public ControlThemeSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type tc = typeof(TemplatedControl);
        Type theme = typeof(ControlTheme);
        Entries.Add(new ReflectionEntry("TemplatedControl", tc.FullName ?? tc.Name));
        Entries.Add(new ReflectionEntry("ControlTheme", theme.FullName ?? theme.Name));
        foreach (MethodInfo m in tc.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(x => x.Name is "OnApplyTemplate" or "OnTemplateChanged").Take(4))
        {
            Entries.Add(new ReflectionEntry("  方法", m.ReturnType.Name + " " + m.Name + FormatArgs(m)));
        }

        Entries.Add(new ReflectionEntry("TemplateBinding", "在模板里 {TemplateBinding Property}"));
        Entries.Add(new ReflectionEntry("OnApplyTemplate", "e.NameScope.Find(\"PART_xxx\") — 命名部分"));
        Entries.Add(new ReflectionEntry("TemplatedParent", "模板内控件的 TemplatedParent = 控件本身"));
        Entries.Add(new ReflectionEntry("ControlTheme 注册", "App.Styles 内 <ControlTheme x:Key=...>"));
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name)) + ")";
    }
}
