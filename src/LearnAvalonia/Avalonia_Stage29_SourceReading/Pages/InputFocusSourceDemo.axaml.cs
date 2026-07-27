// Doc:    第11部分-阶段29-必读子系统源码精读.md §15
// Title:  Input / Focus 源码精读
// Stage:  Stage29
// Accept: 列出 FocusManager 路径
// Notes:  反射 InputElement / KeyboardDevice / FocusManager / KeyGesture；展示 GotFocus 路径。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class InputFocusSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public InputFocusSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type ie = typeof(InputElement);
        Type kdev = typeof(KeyboardDevice);
        Type fm = typeof(FocusManager);
        Entries.Add(new ReflectionEntry("InputElement", ie.FullName ?? ie.Name));
        Entries.Add(new ReflectionEntry("KeyboardDevice", kdev.FullName ?? kdev.Name));
        Entries.Add(new ReflectionEntry("FocusManager", fm.FullName ?? fm.Name));
        foreach (PropertyInfo p in ie.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Take(6))
        {
            Entries.Add(new ReflectionEntry("  InputElement 静态属性", p.PropertyType.Name + " " + p.Name));
        }

        Entries.Add(new ReflectionEntry("FocusableProperty", "控件可否获焦"));
        Entries.Add(new ReflectionEntry("IsTabStop", "Tab 链是否参与"));
        Entries.Add(new ReflectionEntry("TabIndex", "Tab 序号"));
        Entries.Add(new ReflectionEntry("FocusAdorner", "聚焦视觉边框"));
        Entries.Add(new ReflectionEntry("Focus(scope)", "KeyboardDevice.Focus(control) — 单一全局焦点"));
        Entries.Add(new ReflectionEntry("GotFocus/LostFocus", "routed events — bubble"));
    }
}
