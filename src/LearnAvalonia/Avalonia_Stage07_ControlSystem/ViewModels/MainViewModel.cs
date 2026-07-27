using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage07_ControlSystem.Pages;

namespace LearnAvalonia.Avalonia_Stage07_ControlSystem.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§A1-A5 内容与基础控件", "第4部分-阶段7-控件体系.md §A1", "区分单内容/多项目/纯文本/装饰", () => new ContentBasicDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§A6-A8 Button 族·开关·进度", "第4部分-阶段7-控件体系.md §A6", "使用 Command 优于业务 Click", () => new ButtonToggleProgressDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§B1-B7 输入控件速览", "第4部分-阶段7-控件体系.md §B1", "使用 PlaceholderText 与各输入控件", () => new InputControlsDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§C1 集合决策总表", "第4部分-阶段7-控件体系.md §C1", "背决策表不背 API", () => new CollectionDecisionDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§C2-C5 ItemsControl·ListBox·TreeView·Carousel", "第4部分-阶段7-控件体系.md §C2", "使用 ListBox 选中项与 ItemTemplate", () => new ListTreeCarouselDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§C6 TableView 12.1 只读表", "第4部分-阶段7-控件体系.md §C6", "准确说出 TableView 包与状态", () => new TableViewDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§C7-C10 ItemsRepeater·DataGrid·TreeDataGrid·虚拟化", "第4部分-阶段7-控件体系.md §C7", "区分各表控件包/许可/维护边界", () => new RepeaterDataGridVirtualDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§D1-D6 导航与 Pages", "第4部分-阶段7-控件体系.md §D1", "区分桌面壳与移动 Pages 导航", () => new NavigationPagesDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§E1-E2 弹层 Popup·Flyout·ContextMenu", "第4部分-阶段7-控件体系.md §E1", "使用 MenuFlyout 与 ContextMenu", () => new FlyoutPopupDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§E3-E4 ToolTip·Expander·SplitView·Window", "第4部分-阶段7-控件体系.md §E3", "使用 SplitView+ContentControl 桌面壳", () => new ToolTipSplitWindowDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§F1-F3 控件与模板·伪类·AutomationId", "第4部分-阶段7-控件体系.md §F1", "为关键控件补 AutomationId", () => new TemplatePseudoClassDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§G1 完整选型决策表", "第4部分-阶段7-控件体系.md §G1", "按 G1 表做选型口述（抽查 8 个）", () => new SelectionDecisionDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§H1-H3 12 输入选择行为", "第4部分-阶段7-控件体系.md §H1", "解释触摸/笔选择在 release 的意义", () => new PointerReleaseDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§I 平台差异摘要", "第4部分-阶段7-控件体系.md §I", "区分桌面/移动/Browser 能力", () => new PlatformDiffDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§J1-J3 过时·实验·反模式", "第4部分-阶段7-控件体系.md §J1", "点名已删除 API 与替代", () => new DeprecatedAntiPatternDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§K1-K4 最小实验·验收·链接", "第4部分-阶段7-控件体系.md §K1", "勾选验收清单 8 条", () => new AcceptanceChecklistDemo()) { Order = 16 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
