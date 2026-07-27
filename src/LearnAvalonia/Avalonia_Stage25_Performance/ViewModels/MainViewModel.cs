using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage25_Performance.Pages;

namespace LearnAvalonia.Avalonia_Stage25_Performance.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：性能与诊断", "第10部分-阶段25-性能与诊断.md §0", "口述性能三目标", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 启动：冷/热启动", "第10部分-阶段25-性能与诊断.md §1", "Stopwatch 测启动可演示", () => new StartupPerfDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 UI 线程健康", "第10部分-阶段25-性能与诊断.md §2", "UI 线程无长任务", () => new UiThreadHealthDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 布局次数与树规模", "第10部分-阶段25-性能与诊断.md §3", "LayoutUpdated 计数器可演示", () => new LayoutCountDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 绑定与分配", "第10部分-阶段25-性能与诊断.md §4", "CompiledBinding 减少 alloc", () => new BindingAllocDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 GC 与内存", "第10部分-阶段25-性能与诊断.md §5", "GC.GetTotalMemory 可演示", () => new GcMemoryDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 图片、字体、文本", "第10部分-阶段25-性能与诊断.md §6", "图片缓存 + 字体子集", () => new ImageFontTextDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§7 虚拟化", "第10部分-阶段25-性能与诊断.md §7", "ListBox 虚拟化默认", () => new VirtualizationDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§8 Overdraw/Compositor/帧率", "第10部分-阶段25-性能与诊断.md §8", "帧率与 overdraw 说明", () => new OverdrawFpsDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§9 GPU vs 软件渲染", "第10部分-阶段25-性能与诊断.md §9", "Backend 差异说明", () => new GpuSoftwareDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 工具箱", "第10部分-阶段25-性能与诊断.md §11", "dotTrace/dotMem/perfview 列表", () => new ProfilingToolsDemo()) { Order = 11 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
