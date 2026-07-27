// Doc:    第11部分-阶段29-必读子系统源码精读.md §10
// Title:  Binding Engine / Compiled 源码精读
// Stage:  Stage29
// Accept: 解释 compiled vs reflection
// Notes:  反射 CompiledBinding / BindingOperations / InstancedBinding 入口。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Data;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class BindingSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public BindingSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type binding = typeof(Binding);
        Type compiled = typeof(CompiledBinding);
        Type ops = typeof(BindingOperations);
        Entries.Add(new ReflectionEntry("Binding (reflection)", binding.FullName ?? binding.Name));
        Entries.Add(new ReflectionEntry("CompiledBinding", compiled.FullName ?? compiled.Name));
        Entries.Add(new ReflectionEntry("BindingOperations", ops.FullName ?? ops.Name));
        foreach (MethodInfo m in ops.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Take(6))
        {
            Entries.Add(new ReflectionEntry("  静态方法", m.ReturnType.Name + " " + m.Name + FormatArgs(m)));
        }

        Entries.Add(new ReflectionEntry("编译绑定", "x:DataType + x:CompileBindings=true → 编译期生成访问器"));
        Entries.Add(new ReflectionEntry("反射绑定", "无 x:DataType → 运行时反射路径"));
        Entries.Add(new ReflectionEntry("12 默认", "CompiledBinding 默认开 — 缺 x:DataType → AVLN2100"));
        Entries.Add(new ReflectionEntry("InstancedBinding", "运行期实例；由 BindingExpression 创建"));
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name)) + ")";
    }
}
