using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage28_WpfMigration.Pages;

namespace LearnAvalonia.Avalonia_Stage28_WpfMigration.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：WPF/WinUI/WinForms 迁移", "第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §0", "口述迁移三策略", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 概念映射总表", "第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §1", "WPF→Avalonia 概念对照", () => new ConceptMappingDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 XAML/样式/模板/绑定/路由差异", "第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §2", "XAML 差异并排示例", () => new XamlStyleBindingDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 Dispatcher 与线程", "第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §3", "Dispatcher 差异", () => new DispatcherThreadingDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 平台服务差异", "第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §4", "Windows 假设铲除", () => new PlatformServiceDiffDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 增量迁移战术", "第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §5", "增量迁移三战术", () => new IncrementalMigrationDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 Avalonia XPF 边界", "第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §6", "XPF 商业边界", () => new XpfBoundaryDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§7 原生宿主与互操作", "第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §7", "互操作场景", () => new NativeHostInteropDemo()) { Order = 8 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
