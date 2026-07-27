using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage24_TestingSystem.Pages;

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：测试体系", "第10部分-阶段24-测试体系.md §0", "口述三层测试金字塔", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 分层职责与命名", "第10部分-阶段24-测试体系.md §1", "Unit/Headless/Visual/Appium 职责", () => new LayeringDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 Unit：ViewModel 与服务", "第10部分-阶段24-测试体系.md §2", "xUnit 单测 ViewModel", () => new UnitViewModelDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 Headless 测试完整示例", "第10部分-阶段24-测试体系.md §3", "AvaloniaFact 构造控件 + 断言", () => new HeadlessDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 Visual regression", "第10部分-阶段24-测试体系.md §4", "Skia 像素 diff 阈值", () => new VisualRegressionDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 Appium 与真窗口", "第10部分-阶段24-测试体系.md §5", "Appium WinAppDriver 注释示例", () => new AppiumDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 专题测试清单", "第10部分-阶段24-测试体系.md §6", "数据绑定/焦点/主题清单", () => new TopicChecklistDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§7 CI 集成", "第10部分-阶段24-测试体系.md §7", "dotnet test + Headless 跑通", () => new CiIntegrationDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§8 平台差异", "第10部分-阶段24-测试体系.md §8", "三平台测试差异表", () => new PlatformDiffDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§9 Avalonia 12/12.1 变化", "第10部分-阶段24-测试体系.md §9", "Headless.XUnit 稳定", () => new Avalonia12Demo()) { Order = 10 });
        Topics.Add(new TopicItem("§10 过时", "第10部分-阶段24-测试体系.md §10", "点名过时 API", () => new DeprecatedDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 完整样例仓库清单", "第10部分-阶段24-测试体系.md §12", "从零最小仓库结构", () => new RepoSkeletonDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 验收", "第10部分-阶段24-测试体系.md §13", "勾选验收清单", () => new AcceptanceDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§17 一页总结", "第10部分-阶段24-测试体系.md §17", "测试体系一页纸", () => new OnePagerDemo()) { Order = 14 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
