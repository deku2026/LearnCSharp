using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：从会写到可演进", "第9部分-阶段21-应用架构与工程结构.md §0", "口述三大目标与一页总图", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 UI 架构选型 MVVM/Rx/MVU", "第9部分-阶段21-应用架构与工程结构.md §1", "书面选型并给理由", () => new UiArchitectureChoiceDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 分层 vs 垂直切片", "第9部分-阶段21-应用架构与工程结构.md §2", "画清依赖方向", () => new LayeringSlicingDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 模块化与贡献点", "第9部分-阶段21-应用架构与工程结构.md §3", "IModule + 贡献点契约", () => new ModularizationDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 插件与 AssemblyLoadContext", "第9部分-阶段21-应用架构与工程结构.md §4", "ALC 隔离与卸载限制", () => new PluginAlcDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 DI 与 Hosting 组合根", "第9部分-阶段21-应用架构与工程结构.md §5", "BuildServiceProvider 仅在 host", () => new DiHostingDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 配置与日志", "第9部分-阶段21-应用架构与工程结构.md §6", "IOptions + 结构化日志", () => new ConfigLoggingDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§7 导航与对话框服务", "第9部分-阶段21-应用架构与工程结构.md §7", "VM 零 Window 类型引用", () => new NavigationDialogDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§8 状态/Undo/Redo/文档模型", "第9部分-阶段21-应用架构与工程结构.md §8", "Undo 最小演示", () => new UndoDocumentDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§9 Autosave 与崩溃恢复", "第9部分-阶段21-应用架构与工程结构.md §9", "杀进程后可恢复", () => new AutosaveRecoveryDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§10 多窗口状态", "第9部分-阶段21-应用架构与工程结构.md §10", "持久化窗口 bounds 与文档列表", () => new MultiWindowStateDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§11 遥测边界", "第9部分-阶段21-应用架构与工程结构.md §11", "接口存在且默认安全", () => new TelemetryDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§12 桌面生产补充", "第9部分-阶段21-应用架构与工程结构.md §12", "单实例/IPC/CLI/文件关联/更新/迁移", () => new DesktopProductionDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§13 授权许可边界", "第9部分-阶段21-应用架构与工程结构.md §13", "功能开关 + 校验服务接口", () => new LicensingDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§14 平台差异（架构层）", "第9部分-阶段21-应用架构与工程结构.md §14", "桌面/移动/Browser 架构差异", () => new PlatformDiffDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§15 Avalonia 12 相关", "第9部分-阶段21-应用架构与工程结构.md §15", "模板/导航/trim/WebView 架构影响", () => new Avalonia12Demo()) { Order = 16 });
        Topics.Add(new TopicItem("§16 过时内容红线", "第9部分-阶段21-应用架构与工程结构.md §16", "点名过时与替代", () => new DeprecatedDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§17 实验边界", "第9部分-阶段21-应用架构与工程结构.md §17", "区分必做与加分", () => new ExperimentBoundaryDemo()) { Order = 18 });
        Topics.Add(new TopicItem("§18 最小实验 A-H", "第9部分-阶段21-应用架构与工程结构.md §18", "完成 8 个最小实验", () => new MinExperimentsDemo()) { Order = 19 });
        Topics.Add(new TopicItem("§19 验收标准", "第9部分-阶段21-应用架构与工程结构.md §19", "勾选 11 条验收清单", () => new AcceptanceChecklistDemo()) { Order = 20 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
