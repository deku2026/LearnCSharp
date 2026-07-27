using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage05_LogicalVisualTree.Pages;

namespace LearnAvalonia.Avalonia_Stage05_LogicalVisualTree.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 Logical Tree", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §1", "说明逻辑树影响什么", () => new LogicalTreeDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 Visual Tree", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §2", "打印同一控件两棵树", () => new VisualTreeDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 Templated Parent", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §3", "说明 TemplatedParent 与 TemplateBinding", () => new TemplatedParentDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 NameScope", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §4", "解释模板内外 x:Name 互不可见", () => new NameScopeDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 资源查找路径", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §5", "说明资源沿逻辑父链查找", () => new ResourceLookupDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 路由事件/焦点/HitTest", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §6", "描述路由事件与 hit test 路径", () => new RoutedEventDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 Popup 独立视觉根", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §7", "说明 Popup 独立 visual root 后果", () => new PopupVisualRootDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 AdornerLayer", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §8", "知道 AdornerLayer 获取方式", () => new AdornerLayerDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 TopLevel/IPresentationSource", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §9", "正确调用 GetTopLevel，不盲转", () => new TopLevelDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 从启动到树", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §10", "串起阶段 1-5 的树形成过程", () => new StartupToTreeDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 DevTools 对照法", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §11", "用 DevTools 对照两棵树", () => new DevToolsDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 综合示例 LabeledField", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §12", "自定义带模板控件的树", () => new LabeledFieldDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 跨平台差异表", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §13", "覆盖 8 类平台树注意", () => new TreePlatformDiffDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§14 Avalonia 12 最新变化", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §14", "列出 12/12.1 树相关变化", () => new Tree12ChangesDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§15 过时与禁止", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §15", "点名 7 项禁令与替代", () => new TreeDeprecatedDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§16 最小实验 A-F", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §16", "完成 6 个最小实验", () => new TreeExperimentsDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§17 源码阅读路径", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §17", "列出关键类型与关注点", () => new TreeSourceReadingDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§18 验收标准", "第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §18", "勾选 9 条验收清单", () => new TreeAcceptanceDemo()) { Order = 18 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
