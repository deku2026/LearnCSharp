using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage01_AppBuilder.Pages;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 启动链总览", "第2部分-阶段1-AppBuilder与应用启动.md §1", "闭卷画出完整启动链", () => new StartupChainDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 AppBuilder", "第2部分-阶段1-AppBuilder与应用启动.md §2", "解释 Configure/UsePlatformDetect/Skia+HarfBuzz", () => new AppBuilderDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 完整桌面示例", "第2部分-阶段1-AppBuilder与应用启动.md §3", "手写 Program + App 三分支", () => new DesktopProgramDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 IApplicationLifetime", "第2部分-阶段1-AppBuilder与应用启动.md §4", "区分 Desktop/SingleView/Activity", () => new ApplicationLifetimeDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 ShutdownMode", "第2部分-阶段1-AppBuilder与应用启动.md §5", "配置并验证两种 ShutdownMode", () => new ShutdownModeDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 手动 lifetime", "第2部分-阶段1-AppBuilder与应用启动.md §6", "写通过 Start+Run 的桌面最小程序", () => new ManualLifetimeDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 Android 入口", "第2部分-阶段1-AppBuilder与应用启动.md §7", "列出 Android 12 正确类型与已删除 API", () => new AndroidEntryDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 iOS scene", "第2部分-阶段1-AppBuilder与应用启动.md §8", "说明 AppDelegate.Window 可能 null", () => new IosEntryDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 其他平台入口", "第2部分-阶段1-AppBuilder与应用启动.md §9", "覆盖 Browser/X11/Wayland/framebuffer", () => new OtherPlatformsDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 TopLevel/VisualTree", "第2部分-阶段1-AppBuilder与应用启动.md §10", "说明 TopLevel 与启动成功标志", () => new TopLevelVisualTreeDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 跨平台差异表", "第2部分-阶段1-AppBuilder与应用启动.md §11", "平台表覆盖 8 类宿主", () => new PlatformDiffDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 Avalonia 12 最新变化", "第2部分-阶段1-AppBuilder与应用启动.md §12", "列出 12/12.1 关键变化", () => new Avalonia12ChangesDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 过时与禁止", "第2部分-阶段1-AppBuilder与应用启动.md §13", "点名已删除 API 与替代", () => new DeprecatedApisDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§14 最小实验 A-E", "第2部分-阶段1-AppBuilder与应用启动.md §14", "完成 5 个最小实验", () => new MinExperimentsDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§15 验收标准", "第2部分-阶段1-AppBuilder与应用启动.md §15", "勾选 11 条验收清单", () => new AcceptanceChecklistDemo()) { Order = 15 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
