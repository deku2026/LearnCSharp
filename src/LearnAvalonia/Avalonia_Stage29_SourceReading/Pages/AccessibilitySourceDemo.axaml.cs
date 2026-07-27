// Doc:    第11部分-阶段29-必读子系统源码精读.md §18
// Title:  Accessibility Peers 源码精读
// Stage:  Stage29
// Accept: 解释 AutomationPeer 角色
// Notes:  Avalonia.Automation.Peers 下 AutomationPeer / ControlAutomationPeer；Automation.AutomationProperties 描述。
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Automation.Peers;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class AccessibilitySourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public AccessibilitySourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Type peer = typeof(AutomationPeer);
        Type ctrlPeer = typeof(ControlAutomationPeer);
        Entries.Add(new ReflectionEntry("AutomationPeer (abstract)", peer.FullName ?? peer.Name));
        Entries.Add(new ReflectionEntry("ControlAutomationPeer", ctrlPeer.FullName ?? ctrlPeer.Name));
        Entries.Add(new ReflectionEntry("AutomationProperties", "Avalonia.Automation.AutomationProperties — Attached 属性"));
        Entries.Add(new ReflectionEntry("IGridProvider / IInvokeProvider / IRangeValueProvider", "Provider pattern 接口"));
        foreach (MethodInfo m in peer.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(x => x.Name.Contains("Core", StringComparison.Ordinal)).Take(6))
        {
            Entries.Add(new ReflectionEntry("  Peer Core 方法", m.ReturnType.Name + " " + m.Name));
        }

        Entries.Add(new ReflectionEntry("OnCreateAutomationPeer", "控件重写 — 返回 Peer"));
        Entries.Add(new ReflectionEntry("AutomationProperties.Name", "Attached 属性 — 读屏名称"));
        Entries.Add(new ReflectionEntry("Provider 模式", "Invoke / RangeValue / Selection / Toggle …"));
    }
}
