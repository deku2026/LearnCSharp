// Doc:    第11部分-阶段29-必读子系统源码精读.md §17
// Title:  Platform Storage 源码精读
// Stage:  Stage29
// Accept: 列出 IPlatform 抽象边界
// Notes:  Avalonia.Platform 下 IPlatform / IWindowingPlatform / IStorageProvider；用 AvaloniaLocator + 字符串描述。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class PlatformStorageSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public PlatformStorageSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type loc = typeof(AvaloniaLocator);
        Entries.Add(new ReflectionEntry("IPlatform", "Avalonia.Platform.IPlatform — 平台根"));
        Entries.Add(new ReflectionEntry("IWindowingPlatform", "Avalonia.Platform.IWindowingPlatform — 窗口/控件工厂"));
        Entries.Add(new ReflectionEntry("IPlatformThreading", "Avalonia.Platform.IPlatformThreading — UI 线程信号"));
        Entries.Add(new ReflectionEntry("IPlatformGraphics", "Avalonia.Platform.IPlatformGraphics — GPU 上下文工厂"));
        Entries.Add(new ReflectionEntry("IStorageProvider", "Avalonia.Platform.Storage.IStorageProvider"));
        Entries.Add(new ReflectionEntry("IClipboard", "Avalonia.Platform.IClipboard"));
        Entries.Add(new ReflectionEntry("AvaloniaLocator", loc.FullName ?? loc.Name));
        foreach (MethodInfo m in loc.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Take(4))
        {
            Entries.Add(new ReflectionEntry("  Locator 方法", m.ReturnType.Name + " " + m.Name));
        }

        Entries.Add(new ReflectionEntry("注册方式", "AvaloniaLocator.CurrentMutable.BindTo<T>(impl)"));
    }
}
