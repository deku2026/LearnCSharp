using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 架构与示例题", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1", "列出 GaugeControl + RadialPanel + 仪表表", () => new ArchitectureDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 GaugeControl 控件", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.1", "运行 GaugeControl + Value 变更", () => new GaugeControlDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 StyledProperty + DirectProperty", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.2", "区分两种 property 注册", () => new PropertyTypesDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 路由事件 ValueChanged", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.3", "订阅 routed event", () => new RoutedEventDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 ControlTheme", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.4", "ControlTheme 注册并应用", () => new ControlThemeDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 RadialPanel 自定义布局", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.5", "MeasureOverride + ArrangeOverride", () => new RadialPanelDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 自定义 Render (DrawingContext)", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.6", "Render 自绘圆弧刻度", () => new CustomRenderDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 动画 (值变更过渡)", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.7", "用 Avalonia.Animation 做过渡", () => new AnimationDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 无障碍 AutomationPeer", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.8", "OnCreateAutomationPeer 返回", () => new AccessibilityDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 本地化", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.9", "ResourceProvider + 多语言", () => new LocalizationDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 测试结构", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §3", "Unit/Headless/集成/a11y", () => new TestStructureDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 Benchmark 概念", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §4", "BenchmarkDotNet stub", () => new BenchmarkDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 Trim / Native AOT", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §5", "IsAotCompatible + 警告", () => new TrimAotDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§14 源码精读一面", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §6", "一页模板", () => new SourceReadingDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§15 平台表 + 12.1 变更", "第10部分-Capstone4-自定义控件渲染与Native-AOT.md §7-8", "勾选平台表与 12.1 变更", () => new PlatformDiffDemo()) { Order = 15 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
