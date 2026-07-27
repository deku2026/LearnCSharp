// Doc:    第10部分-阶段23-无障碍国际化与文本.md §1
// Title:  AutomationPeer 与 a11y 树
// Stage:  Stage23
// Accept: 自定义 Peer 暴露名称/角色
// Notes:  自定义 Control 重写 OnCreateAutomationPeer 返回自定义 Peer；GetNameValue/GetClassName。
using Avalonia.Automation.Peers;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

public partial class AutomationPeerDemo : UserControl
{
    public AutomationPeerDemo()
    {
        InitializeComponent();
    }
}

internal sealed class AccessibleCard : ContentControl
{
    protected override AutomationPeer OnCreateAutomationPeer()
        => new AccessibleCardPeer(this);

    public string? AccessibleName { get; set; }
}

internal sealed class AccessibleCardPeer : ContentControlAutomationPeer
{
    private readonly AccessibleCard _owner;

    public AccessibleCardPeer(AccessibleCard owner) : base(owner)
    {
        _owner = owner;
    }

    protected override string? GetNameCore() => _owner.AccessibleName ?? base.GetNameCore();

    protected override string GetClassNameCore() => "AccessibleCard";
}
