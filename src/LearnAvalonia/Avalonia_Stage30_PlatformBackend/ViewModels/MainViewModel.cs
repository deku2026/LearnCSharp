using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage30_PlatformBackend.Pages;

namespace LearnAvalonia.Avalonia_Stage30_PlatformBackend.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 概览", "第11部分-阶段30-平台后端选读.md §1", "覆盖桌面 5 后端", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 Win32", "第11部分-阶段30-平台后端选读.md §2", "列出 Win32 入口", () => new Win32Demo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 WinUI Composition", "第11部分-阶段30-平台后端选读.md §3", "解释渲染/输入路径", () => new WinUICompositionDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 macOS (Avalonia.Native)", "第11部分-阶段30-平台后端选读.md §4", "列出 AppKit 桥", () => new MacOSDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 X11 (Linux 默认)", "第11部分-阶段30-平台后端选读.md §5", "列出 X11 入口", () => new X11Demo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 Wayland (实验)", "第11部分-阶段30-平台后端选读.md §6", "标记实验，不实现", () => new WaylandDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 Android (非桌面)", "第11部分-阶段30-平台后端选读.md §7", "见 roadmap，桌面策略不实现", () => new AndroidNotImplDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 iOS (非桌面)", "第11部分-阶段30-平台后端选读.md §8", "见 roadmap，桌面策略不实现", () => new IosNotImplDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 Browser/WASM (非桌面)", "第11部分-阶段30-平台后端选读.md §9", "见 roadmap，桌面策略不实现", () => new BrowserNotImplDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 Headless (桌面/测试)", "第11部分-阶段30-平台后端选读.md §11", "列出 Headless 用途", () => new HeadlessDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 平台总览", "第11部分-阶段30-平台后端选读.md §20", "勾选平台表", () => new PlatformSummaryDemo()) { Order = 11 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
