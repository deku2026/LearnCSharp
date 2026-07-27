// Doc:    第11部分-阶段29-必读子系统源码精读.md §1
// Title:  AppBuilder 源码精读
// Stage:  Stage29
// Accept: 画出 AppBuilder 注入链
// Notes:  反射列出 AppBuilder 公共方法；可见 UsePlatformDetect/WithInterFont/LogToTrace 等流式配置入口。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class AppBuilderSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public AppBuilderSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type t = typeof(AppBuilder);
        Entries.Add(new ReflectionEntry("类型", t.FullName ?? t.Name));
        Entries.Add(new ReflectionEntry("程序集", t.Assembly.GetName().Name ?? "?"));
        Entries.Add(new ReflectionEntry("基类", t.BaseType?.FullName ?? "(none)"));
        Entries.Add(new ReflectionEntry("接口数", t.GetInterfaces().Length.ToString()));
        Entries.Add(new ReflectionEntry("公共方法（前 12 个）", ""));
        foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Take(12))
        {
            Entries.Add(new ReflectionEntry("  " + m.ReturnType.Name + " " + m.Name, FormatArgs(m)));
        }
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name + " " + x.Name)) + ")";
    }
}
