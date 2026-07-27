using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 AppBuilder", "第11部分-阶段29-必读子系统源码精读.md §1", "画出 AppBuilder 注入链", () => new AppBuilderSourceDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 Application/Lifetime", "第11部分-阶段29-必读子系统源码精读.md §2", "列出 Lifetime 三分支与序", () => new ApplicationLifetimeSourceDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 AvaloniaObject", "第11部分-阶段29-必读子系统源码精读.md §3", "说出 AO 的属性优先级入口", () => new AvaloniaObjectSourceDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 AvaloniaProperty", "第11部分-阶段29-必读子系统源码精读.md §4", "区分 Styled/Direct/Attached", () => new AvaloniaPropertySourceDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 Property Store", "第11部分-阶段29-必读子系统源码精读.md §5", "解释优先级与 effective 值", () => new PropertyStoreSourceDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 Logical Tree", "第11部分-阶段29-必读子系统源码精读.md §6", "列出 ILogical 关系接口", () => new LogicalTreeSourceDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 Visual Tree", "第11部分-阶段29-必读子系统源码精读.md §7", "画出 Visual 与 VisualRoot", () => new VisualTreeSourceDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 Routed Events", "第11部分-阶段29-必读子系统源码精读.md §8", "区分 tunnel/bubble/直接", () => new RoutedEventSourceDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 Layout Manager", "第11部分-阶段29-必读子系统源码精读.md §9", "描述 Measure/Arrange 队列", () => new LayoutSourceDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 Binding Engine", "第11部分-阶段29-必读子系统源码精读.md §10", "解释 compiled vs reflection", () => new BindingSourceDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 Selector/Style", "第11部分-阶段29-必读子系统源码精读.md §11", "画出选择器匹配顺序", () => new SelectorSourceDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 ControlTheme", "第11部分-阶段29-必读子系统源码精读.md §12", "说出 TemplatedControl 契约", () => new ControlThemeSourceDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 TopLevel/Window", "第11部分-阶段29-必读子系统源码精读.md §13", "列出 TopLevel 平台入口", () => new TopLevelSourceDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§14 Dispatcher", "第11部分-阶段29-必读子系统源码精读.md §14", "解释优先级与 Send/Post", () => new DispatcherSourceDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§15 Input/Focus", "第11部分-阶段29-必读子系统源码精读.md §15", "列出 FocusManager 路径", () => new InputFocusSourceDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§16 Renderer/Compositor", "第11部分-阶段29-必读子系统源码精读.md §16", "画出 Render 调用链", () => new RendererSourceDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§17 Platform Storage", "第11部分-阶段29-必读子系统源码精读.md §17", "列出 IPlatform 抽象边界", () => new PlatformStorageSourceDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§18 Accessibility Peers", "第11部分-阶段29-必读子系统源码精读.md §18", "解释 AutomationPeer 角色", () => new AccessibilitySourceDemo()) { Order = 18 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
