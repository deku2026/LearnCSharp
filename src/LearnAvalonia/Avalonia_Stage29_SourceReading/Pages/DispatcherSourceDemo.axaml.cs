// Doc:    第11部分-阶段29-必读子系统源码精读.md §14
// Title:  Dispatcher 源码精读
// Stage:  Stage29
// Accept: 解释优先级与 Send/Post
// Notes:  反射 Dispatcher / DispatcherPriority；展示 Post/Invoke/InvokeAsync 入口。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Threading;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class DispatcherSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public DispatcherSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type disp = typeof(Dispatcher);
        Type prio = typeof(DispatcherPriority);
        Entries.Add(new ReflectionEntry("Dispatcher", disp.FullName ?? disp.Name));
        foreach (FieldInfo f in prio.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            Entries.Add(new ReflectionEntry("  Priority", f.Name));
        }

        foreach (MethodInfo m in disp.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(x => x.Name is "Post" or "Invoke" or "InvokeAsync" or "CheckAccess").Take(8))
        {
            Entries.Add(new ReflectionEntry("  方法", m.ReturnType.Name + " " + m.Name + FormatArgs(m)));
        }

        Entries.Add(new ReflectionEntry("UIThread", "Dispatcher.UIThread — 单例"));
        Entries.Add(new ReflectionEntry("Send", "Invoke — 同步阻塞"));
        Entries.Add(new ReflectionEntry("Post", "Post — 异步队列"));
        Entries.Add(new ReflectionEntry("CheckAccess", "当前线程是否 UI 线程"));
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name)) + ")";
    }
}
