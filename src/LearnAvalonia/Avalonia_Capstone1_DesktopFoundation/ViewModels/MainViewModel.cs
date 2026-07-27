using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Capstone1_DesktopFoundation.Pages;

namespace LearnAvalonia.Avalonia_Capstone1_DesktopFoundation.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 规格导读与目标", "第4部分-Capstone1-生产级桌面基础应用.md §0", "口述 Capstone 目标与非目标", () => new SpecOverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 功能清单 P0/P1/P2", "第4部分-Capstone1-生产级桌面基础应用.md §1", "功能清单 P0 全绿", () => new FeatureChecklistDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 架构建议与组合根", "第4部分-Capstone1-生产级桌面基础应用.md §2", "DI 组合根清晰", () => new ArchitectureCompositionDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 平台要求与差异", "第4部分-Capstone1-生产级桌面基础应用.md §3", "Win+macOS+Linux 至少各一证据", () => new PlatformRequirementsDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 必须项详述（XAML/DI/Theme/导航/对话框/Storage）", "第4部分-Capstone1-生产级桌面基础应用.md §4", "VM 零 Window 类型引用", () => new MustImplementDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 UI 信息架构与线框", "第4部分-Capstone1-生产级桌面基础应用.md §5", "主壳自适应 resize 无崩布局", () => new UiInformationArchitectureDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 分周实施计划", "第4部分-Capstone1-生产级桌面基础应用.md §6", "按周计划执行并过门禁", () => new WeeklyPlanDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§7 验收勾选清单", "第4部分-Capstone1-生产级桌面基础应用.md §7", "验收勾选全绿", () => new AcceptanceChecklistDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§8 反模式清单", "第4部分-Capstone1-生产级桌面基础应用.md §8", "无反模式出现", () => new AntiPatternDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§9-18 目录骨架·测试·交付·接口", "第4部分-Capstone1-生产级桌面基础应用.md §9", "交付物清单齐全", () => new DeliveryInterfaceDemo()) { Order = 10 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
