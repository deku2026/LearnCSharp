using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage26_TrimmingAOT.Pages;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：Trimming 与 AOT", "第10部分-阶段26-Trimming与Native-AOT.md §0", "口述 trim/AOT 收益与代价", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 项目配置基础", "第10部分-阶段26-Trimming与Native-AOT.md §1", "PublishTrimmed/PublishAot 配置", () => new ProjectConfigDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 Avalonia 特有友好点", "第10部分-阶段26-Trimming与Native-AOT.md §2", "CompiledBinding 友好 AOT", () => new AvaloniaFriendlyDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 动态 XAML 禁区", "第10部分-阶段26-Trimming与Native-AOT.md §3", "运行时 XAML 不兼容 AOT", () => new DynamicXamlForbiddenDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 反射/DI/序列化/插件", "第10部分-阶段26-Trimming与Native-AOT.md §4", "反射替代方案", () => new ReflectionDiSerializerDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 ViewLocator 不友好", "第10部分-阶段26-Trimming与Native-AOT.md §5", "ViewLocator 反射 vs CompiledBinding", () => new ViewLocatorDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 警告处理流程", "第10部分-阶段26-Trimming与Native-AOT.md §7", "Trim warnings 修复路径", () => new WarningWorkflowDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 符号与崩溃诊断", "第10部分-阶段26-Trimming与Native-AOT.md §8", "Native AOT 崩溃 dump", () => new SymbolsCrashDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 对比实验设计", "第10部分-阶段26-Trimming与Native-AOT.md §9", "trim/AOT 体积对比表", () => new ComparisonExperimentDemo()) { Order = 9 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
