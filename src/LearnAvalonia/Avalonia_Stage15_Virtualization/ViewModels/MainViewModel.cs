using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 为什么虚拟化是分水岭", "第6部分-阶段15-虚拟化与大数据UI.md §1", "解释虚拟化与非虚拟化的临界点", () => new WhyVirtualizationDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 虚拟化原理与容器", "第6部分-阶段15-虚拟化与大数据UI.md §2.1", "画出虚拟化容器/recycle 原理", () => new VirtualizationPrincipleDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 容器与要满足的条件", "第6部分-阶段15-虚拟化与大数据UI.md §2.2", "列出虚拟化启动 3 条件", () => new ContainerConditionsDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 TableView（12.1 新）", "第6部分-阶段15-虚拟化与大数据UI.md §2.3", "TableView 列定义/虚拟化", () => new TableViewDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§2.4 DataGrid（deprecated）", "第6部分-阶段15-虚拟化与大数据UI.md §2.4", "DataGrid 用法与 deprecated 标注", () => new DataGridDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§2.5 TreeDataGrid（Pro）", "第6部分-阶段15-虚拟化与大数据UI.md §2.5", "TreeDataGrid 为商业 Pro，不实现", () => new TreeDataGridDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§2.6 ItemsRepeater", "第6部分-阶段15-虚拟化与大数据UI.md §2.6", "ItemsRepeater 布局虚拟化", () => new ItemsRepeaterDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§2.7 选择模式", "第6部分-阶段15-虚拟化与大数据UI.md §2.7", "区分 Single/Multiple/Extended", () => new SelectionModesDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§2.8 数据分组", "第6部分-阶段15-虚拟化与大数据UI.md §2.8", "分组虚拟化：Grouping 协调", () => new DataGroupingDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§2.9 滚动 FPS 与内存分析", "第6部分-阶段15-虚拟化与大数据UI.md §2.9", "概念性 FPS 测量方法", () => new ScrollFpsDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§2.10 反模式：虚拟化失败", "第6部分-阶段15-虚拟化与大数据UI.md §2.10", "识别 5 种反模式", () => new AntiPatternsDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§2.11 与 MVVM", "第6部分-阶段15-虚拟化与大数据UI.md §2.11", "虚拟化下 MVVM 注意点", () => new WithMvvmDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§3 跨平台差异", "第6部分-阶段15-虚拟化与大数据UI.md §3", "虚拟化在三平台差异表", () => new CrossPlatformDiffDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§4 Avalonia 12/12.1 变化", "第6部分-阶段15-虚拟化与大数据UI.md §4", "12/12.1 关键变化", () => new Avalonia12ChangesDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§5 过时/禁止", "第6部分-阶段15-虚拟化与大数据UI.md §5", "已删除/废弃 API", () => new DeprecatedApisDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§6 最小实验 A-E", "第6部分-阶段15-虚拟化与大数据UI.md §6", "完成 5 个最小实验", () => new MinExperimentsDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§7 验收清单", "第6部分-阶段15-虚拟化与大数据UI.md §7", "勾选验收清单", () => new AcceptanceChecklistDemo()) { Order = 17 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
