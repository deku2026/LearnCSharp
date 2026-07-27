using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 TopLevel 统一抽象", "第7部分-阶段16-统一平台抽象.md §1", "解释 TopLevel 三模式", () => new TopLevelAbstractionDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 StorageProvider 职责", "第7部分-阶段16-统一平台抽象.md §2.1", "StorageProvider 三件套", () => new StorageProviderOverviewDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 最小文件选择器", "第7部分-阶段16-统一平台抽象.md §2.2", "StorageProvider.OpenFilePickerAsync", () => new FilePickerDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 沙盒/书签/安全", "第7部分-阶段16-统一平台抽象.md §2.3", "Starts/Bookmark 安全模型", () => new StorageSecurityDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§2.4 文件夹选择器", "第7部分-阶段16-统一平台抽象.md §2.4", "OpenFolderPickerAsync", () => new FolderPickerDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§2.5 平台差异（Storage）", "第7部分-阶段16-统一平台抽象.md §2.5", "Storage 三平台差异", () => new StoragePlatformDiffDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§3 Clipboard (IAsyncDataTransfer)", "第7部分-阶段16-统一平台抽象.md §3", "Clipboard.SetTextAsync/TryGetTextAsync", () => new ClipboardDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§4 Launcher（URI/外部）", "第7部分-阶段16-统一平台抽象.md §4", "Launcher.LaunchUriAsync", () => new LauncherDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§5 Screens（Screen abstract）", "第7部分-阶段16-统一平台抽象.md §5", "Screen 12 起 abstract", () => new ScreensDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§6 PlatformSettings / Hotkey", "第7部分-阶段16-统一平台抽象.md §6", "HotkeyConfiguration 读取", () => new PlatformSettingsDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§7 PlatformFeedback（触觉）", "第7部分-阶段16-统一平台抽象.md §7", "PlatformFeedback 触觉反馈", () => new PlatformFeedbackDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§8 拖放（DnD）", "第7部分-阶段16-统一平台抽象.md §8", "应用内 vs 应用外 DnD", () => new DragDropDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§9 原生菜单/通知", "第7部分-阶段16-统一平台抽象.md §9", "NativeMenu vs Menu + 通知", () => new NativeMenuDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§10 WebView 在本阶段位置", "第7部分-阶段16-统一平台抽象.md §10", "WebView 放阶段17", () => new WebViewPositionDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§11 接口能力清单", "第7部分-阶段16-统一平台抽象.md §11", "TopLevel 能力访问器", () => new CapabilityMatrixDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§12 平台差异总表", "第7部分-阶段16-统一平台抽象.md §12", "三平台能力总表", () => new PlatformMatrixDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§13 Avalonia 12/12.1 变化", "第7部分-阶段16-统一平台抽象.md §13", "12/12.1 平台抽象变化", () => new Avalonia12ChangesDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§16 最小实验 A-E", "第7部分-阶段16-统一平台抽象.md §16", "完成 5 个最小实验", () => new MinExperimentsDemo()) { Order = 18 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
