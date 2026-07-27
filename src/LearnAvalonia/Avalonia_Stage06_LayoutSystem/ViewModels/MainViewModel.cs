using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§A1 两阶段管线", "第4部分-阶段6-布局系统.md §A1", "闭卷画出 Measure→Arrange 时序", () => new LayoutPipelineDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§A1.1 尺寸三元组", "第4部分-阶段6-布局系统.md §A1.1", "解释 Desired vs Available vs Final", () => new SizeSemanticsDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§A1.2 无限可用空间", "第4部分-阶段6-布局系统.md §A1.2", "说明 infinity measure 的陷阱", () => new InfinityMeasureDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§B1 Invalidation 规则", "第4部分-阶段6-布局系统.md §B1", "区分 InvalidateMeasure/Arrange/Visual", () => new InvalidationRulesDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§B2-B3 Layout Rounding 与 DPI", "第4部分-阶段6-布局系统.md §B2", "在分数缩放下验证无 1px 错位", () => new LayoutRoundingDpiDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§C1-C2 Alignment 与 Margin/Padding", "第4部分-阶段6-布局系统.md §C1", "正确说明 Alignment 与 Margin/Padding", () => new AlignmentMarginDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§C3-C4 Width/Min/Max 与 Clip", "第4部分-阶段6-布局系统.md §C3", "说明 Width/Min/Max 优先级与 Clip", () => new WidthMinMaxClipDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§D1 Panel 选型总表", "第4部分-阶段6-布局系统.md §D1", "按场景选型 Panel，无全用 Canvas", () => new PanelSelectionDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§D2 StackPanel", "第4部分-阶段6-布局系统.md §D2", "使用 Spacing 与 Orientation", () => new StackPanelDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§D3 Grid", "第4部分-阶段6-布局系统.md §D3", "使用 Auto/*/固定 列定义", () => new GridPanelDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§D4 WrapPanel", "第4部分-阶段6-布局系统.md §D4", "使用 ItemWidth/Height 换行", () => new WrapPanelDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§D5 DockPanel", "第4部分-阶段6-布局系统.md §D5", "使用 Dock 与 LastChildFill", () => new DockPanelDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§D6 Canvas", "第4部分-阶段6-布局系统.md §D6", "使用 Canvas.Left/Top 绝对定位", () => new CanvasPanelDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§D7-D8 RelativePanel 与 UniformGrid", "第4部分-阶段6-布局系统.md §D7", "使用 RelativePanel 约束与 UniformGrid 等分", () => new RelativeUniformPanelDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§E1-E2 Viewbox 与 ScrollViewer", "第4部分-阶段6-布局系统.md §E1", "解释 ScrollViewer 与 Viewbox 的 infinity measure", () => new ScrollViewboxDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§F 自定义 Panel", "第4部分-阶段6-布局系统.md §F1", "提交一个自定义 Panel + 布局断言", () => new CustomPanelDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§G-J 平台差·12变化·专家·验收", "第4部分-阶段6-布局系统.md §G", "统计 resize 布局次数与平台差异", () => new PlatformPerfAcceptanceDemo()) { Order = 17 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
