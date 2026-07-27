using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：无障碍与文本", "第10部分-阶段23-无障碍国际化与文本.md §0", "口述 a11y 四目标与文本三难", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 AutomationPeer 与 a11y 树", "第10部分-阶段23-无障碍国际化与文本.md §1", "自定义 Peer 暴露名称/角色", () => new AutomationPeerDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 键盘导航与焦点", "第10部分-阶段23-无障碍国际化与文本.md §2", "Tab/Arrow/Drag 验收可演示", () => new KeyboardNavDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 平台读屏桥接", "第10部分-阶段23-无障碍国际化与文本.md §3", "列出三平台桥接点", () => new PlatformBridgeDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 语义/高对比/减弱动效/动态字号", "第10部分-阶段23-无障碍国际化与文本.md §4", "切换 ThemeVariant 与动效开关", () => new HighContrastDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 RTL、BiDi 与布局镜像", "第10部分-阶段23-无障碍国际化与文本.md §5", "FlowDirection 切换可演示", () => new RtlBidiDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 Unicode/Grapheme/IME", "第10部分-阶段23-无障碍国际化与文本.md §6", "StringInfo 字素拆解可演示", () => new UnicodeGraphemeDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§7 国际化：RESX/Culture/复数", "第10部分-阶段23-无障碍国际化与文本.md §7", "运行时切语言可演示", () => new I18nResxDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§8 平台差异汇总", "第10部分-阶段23-无障碍国际化与文本.md §8", "差异表覆盖三平台", () => new PlatformDiffDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§9 Avalonia 12/12.1 相关", "第10部分-阶段23-无障碍国际化与文本.md §9", "列出 12/12.1 关键变化", () => new Avalonia12Demo()) { Order = 10 });
        Topics.Add(new TopicItem("§10 过时与禁止", "第10部分-阶段23-无障碍国际化与文本.md §10", "点名已删除 API 与替代", () => new DeprecatedDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 验收清单", "第10部分-阶段23-无障碍国际化与文本.md §12", "勾选 12 条验收清单", () => new AcceptanceChecklistDemo()) { Order = 12 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
