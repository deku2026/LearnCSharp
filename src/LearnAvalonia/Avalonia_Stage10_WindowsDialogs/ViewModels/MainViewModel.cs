using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 为什么桌面窗口是独立阶段", "第5部分-阶段10-桌面窗口对话框与系统集成.md §1", "列出 6 类桌面壳需求", () => new StageOverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 Lifetime 与多窗口", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.1", "区分 Show / ShowDialog 与 Owner", () => new LifetimeMultiWindowDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 位置、尺寸、状态", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.2", "WindowState 是 DirectProperty 不能用 Style", () => new PositionSizeStateDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 WindowDecorations 与自绘标题栏", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.3", "SystemDecorations 已改名 WindowDecorations", () => new WindowDecorationsDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§2.4 透明、亚克力与 Topmost", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.4", "TransparencyLevelHint 可降级", () => new TransparencyTopmostDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§2.5 TrayIcon 必须配 NativeMenu", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.5", "托盘菜单用 NativeMenu 不是 Menu", () => new TrayIconDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§2.6 NativeMenuBar 与 macOS Dock", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.6", "NativeMenu.Menu 用法与跨平台降级", () => new NativeMenuBarDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§2.7 窗口状态持久化", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.7", "Closing 存 Opened 应用 RestoreBounds", () => new WindowStatePersistDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§2.8 多显示器、DPI、拖拽区域", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.8", "BeginMoveDrag 与多屏 Scaling", () => new MultiMonitorDpiDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§2.9 关闭拦截与未保存", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.9", "Closing 异步取消后调用 Close()", () => new CloseInterceptDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§2.10 单实例与 IPC 激活", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.10", "Mutex + 命名管道激活已运行实例", () => new SingleInstanceIpcDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§2.11 文件关联与 URI", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.11", "解析命令行 file path vs URI", () => new FileAssociationUriDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§2.12 StorageProvider 取代旧 FileDialog", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.12", "用 IStorageProvider 打开/保存", () => new StorageProviderDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§2.13 对话框服务抽象", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.13", "IDialogService 让 VM 不引用 Window", () => new DialogServiceAbstractionDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§2.14 任务栏与窗口图标", "第5部分-阶段10-桌面窗口对话框与系统集成.md §2.14", "Icon/ShowInTaskbar/未保存 * 标记", () => new TaskbarIconDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§3 跨平台差异", "第5部分-阶段10-桌面窗口对话框与系统集成.md §3", "Win/mac/Linux 托盘/菜单/透明差异", () => new PlatformDiffDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§4 Avalonia 12 / 12.1 变化", "第5部分-阶段10-桌面窗口对话框与系统集成.md §4", "WindowDecorations / WindowState Direct", () => new Avalonia12ChangesDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§5 过时与禁止", "第5部分-阶段10-桌面窗口对话框与系统集成.md §5", "禁止 SystemDecorations / 旧 FileDialog / Menu 托盘", () => new DeprecatedApisDemo()) { Order = 18 });
        Topics.Add(new TopicItem("§6 最小实验清单", "第5部分-阶段10-桌面窗口对话框与系统集成.md §6", "完成多窗口+模态/托盘/StorageProvider 4 实验", () => new MinExperimentsDemo()) { Order = 19 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
