using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage04_PropertySystem.Pages;

namespace LearnAvalonia.Avalonia_Stage04_PropertySystem.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 为什么需要属性系统", "第3部分-阶段4-属性系统.md §1", "说明 CLR 属性做不到的事", () => new WhyPropertySystemDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 三种属性 Styled/Direct/Attached", "第3部分-阶段4-属性系统.md §2", "闭卷对比三类属性", () => new ThreePropertyKindsDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 注册与 CLR 包装", "第3部分-阶段4-属性系统.md §3", "手写三类属性注册", () => new RegistrationDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 Get/Set/SetCurrent/Clear/Bind", "第3部分-阶段4-属性系统.md §4", "解释 5 个 API 差异", () => new ValueApiDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 值优先级", "第3部分-阶段4-属性系统.md §5", "实验证明覆盖关系", () => new PrecedenceDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 Metadata 默认/校验/强制/继承", "第3部分-阶段4-属性系统.md §6", "说明 validate/coerce/inherits", () => new MetadataDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 变更通知与响应式", "第3部分-阶段4-属性系统.md §7", "写出 class handler 或 OnPropertyChanged", () => new ChangeNotificationDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 完整自定义控件", "第3部分-阶段4-属性系统.md §8", "三类属性齐活的 MetricTile", () => new CustomControlDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 对比 CLR/WPF/MAUI/Avalonia", "第3部分-阶段4-属性系统.md §9", "对比四种属性系统", () => new PropertySystemCompareDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 WindowState 变 Direct", "第3部分-阶段4-属性系统.md §10", "知道 WindowState 变 Direct 的后果", () => new WindowStateDirectDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 数据验证与错误展示", "第3部分-阶段4-属性系统.md §11", "12 验证样板不再无脑粘贴", () => new DataValidationDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 常见陷阱", "第3部分-阶段4-属性系统.md §12", "列出 7 类陷阱与正确做法", () => new PropertyPitfallsDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 跨平台差异表", "第3部分-阶段4-属性系统.md §13", "覆盖 5 类平台属性差异", () => new PropertyPlatformDiffDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§14 Avalonia 12 最新变化", "第3部分-阶段4-属性系统.md §14", "列出 12/12.1 属性系统变化", () => new Property12ChangesDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§15 过时与禁止", "第3部分-阶段4-属性系统.md §15", "点名 7 项禁令与替代", () => new PropertyDeprecatedDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§16 最小实验 A-F", "第3部分-阶段4-属性系统.md §16", "完成 6 个最小实验", () => new PropertyExperimentsDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§17 源码阅读路径", "第3部分-阶段4-属性系统.md §17", "列出关键类型与关注点", () => new PropertySourceReadingDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§18 验收标准", "第3部分-阶段4-属性系统.md §18", "勾选 9 条验收清单", () => new PropertyAcceptanceDemo()) { Order = 18 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
