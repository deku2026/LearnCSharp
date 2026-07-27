using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：复杂桌面生产应用", "第9部分-Capstone2-复杂桌面生产应用.md §0", "口述必做模块与三桌面铁律", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 架构落位（强制）", "第9部分-Capstone2-复杂桌面生产应用.md §1", "插件只引 Abstractions + 组合根在 Desktop host", () => new ArchitectureDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 大数据虚拟化", "第9部分-Capstone2-复杂桌面生产应用.md §2", "万级条目可滚动", () => new VirtualizationDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 Document / Tab 模型", "第9部分-Capstone2-复杂桌面生产应用.md §3", "多 Tab 文档 + 脏文档确认", () => new DocumentTabDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 Undo/Redo", "第9部分-Capstone2-复杂桌面生产应用.md §4", "10 步编辑可逐步撤销", () => new UndoRedoDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 Autosave 与崩溃恢复", "第9部分-Capstone2-复杂桌面生产应用.md §5", "杀进程演练可恢复", () => new AutosaveRecoveryDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 SQLite 本地索引", "第9部分-Capstone2-复杂桌面生产应用.md §6", "元数据 + 启动迁移 + 备份", () => new SqliteDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§7 远程 API 与离线", "第9部分-Capstone2-复杂桌面生产应用.md §7", "同步状态 UX + 脏标记", () => new RemoteApiOfflineDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§8 插件边界", "第9部分-Capstone2-复杂桌面生产应用.md §8", "无插件可跑 + 有插件菜单出现", () => new PluginDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§9 Tray · NativeMenu · DnD", "第9部分-Capstone2-复杂桌面生产应用.md §9", "TrayIcon+NativeMenu + 文件拖入", () => new TrayMenuDndDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§10 全局激活 · 单实例 · CLI", "第9部分-Capstone2-复杂桌面生产应用.md §10", "再双击不出现双实例", () => new SingleInstanceCliDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§11 更新服务", "第9部分-Capstone2-复杂桌面生产应用.md §11", "IUpdateService 可演示", () => new UpdateServiceDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§12 UI 自动化与测试", "第9部分-Capstone2-复杂桌面生产应用.md §12", "单元 + Headless 冒烟绿", () => new UiAutomationTestDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§13 性能剖析", "第9部分-Capstone2-复杂桌面生产应用.md §13", "冷启动/加载/滚动/Tab 内存数字", () => new PerfProfilingDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§14 三桌面平台能力矩阵", "第9部分-Capstone2-复杂桌面生产应用.md §14", "每行 README 写已测/降级", () => new PlatformMatrixDemo()) { Order = 15 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
