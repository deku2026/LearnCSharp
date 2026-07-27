// Doc:    第11部分-阶段29-必读子系统源码精读.md §2
// Title:  Application / Lifetime 源码精读
// Stage:  Stage29
// Accept: 列出 Lifetime 三分支与序
// Notes:  反射 Application 基类与 IClassicDesktopStyleApplicationLifetime；可见 OnFrameworkInitializationCompleted 入口。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class ApplicationLifetimeSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public ApplicationLifetimeSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type app = typeof(Application);
        Entries.Add(new ReflectionEntry("Application 基类", app.FullName ?? app.Name));
        Entries.Add(new ReflectionEntry("程序集", app.Assembly.GetName().Name ?? "?"));
        Entries.Add(new ReflectionEntry("OnFrameworkInitializationCompleted", "virtual void — 子类必 override"));
        Entries.Add(new ReflectionEntry("Initialize", "virtual void — 加载 App.axaml"));
        foreach (MethodInfo m in app.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Take(8))
        {
            Entries.Add(new ReflectionEntry("  方法", m.ReturnType.Name + " " + m.Name + FormatArgs(m)));
        }

        Type desktop = typeof(IClassicDesktopStyleApplicationLifetime);
        Type single = typeof(ISingleViewApplicationLifetime);
        Entries.Add(new ReflectionEntry("IClassicDesktop", "MainWindow / ShutdownMode / Windows"));
        Entries.Add(new ReflectionEntry("ISingleView", "MainView"));
        Entries.Add(new ReflectionEntry("三分支序", "Activity → SingleView → Desktop（铁律）"));
    }

    private static string FormatArgs(MethodInfo m)
    {
        ParameterInfo[] p = m.GetParameters();
        return "(" + string.Join(", ", p.Select(x => x.ParameterType.Name)) + ")";
    }
}
