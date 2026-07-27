using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Capstone3_CrossPlatform.Pages;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 解决方案结构", "第8部分-Capstone3-全平台Avalonia应用.md §1", "桌面三平台 TFM 与项目分层", () => new SolutionStructureDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 共享什么", "第8部分-Capstone3-全平台Avalonia应用.md §2.1", "Views/VM/Service contracts 共享", () => new SharedLayerDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 Host 做什么", "第8部分-Capstone3-全平台Avalonia应用.md §2.2", "桌面平台入口与生命周期", () => new HostRoleDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 Shell 模式", "第8部分-Capstone3-全平台Avalonia应用.md §2.3", "桌面 Shell 选型", () => new ShellPatternDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§3.1 路由策略（桌面）", "第8部分-Capstone3-全平台Avalonia应用.md §3.1", "桌面三平台统一路由", () => new RoutingStrategyDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 存储抽象", "第8部分-Capstone3-全平台Avalonia应用.md §5", "IFileStorage 接口 + 三平台实现", () => new StorageAbstractionDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 平台差异注册（DI）", "第8部分-Capstone3-全平台Avalonia应用.md §6", "DI 容器选型 + 平台注册", () => new PlatformRegistrationDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 日志", "第8部分-Capstone3-全平台Avalonia应用.md §8", "三平台日志状态/提示脚本", () => new LoggingDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§10.2 Headless 测试", "第8部分-Capstone3-全平台Avalonia应用.md §10.2", "Avalonia.Headless 跨平台测试", () => new HeadlessTestDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§11 #if 编译开关", "第8部分-Capstone3-全平台Avalonia应用.md §11", "桌面三平台 #if 例子", () => new ConditionalCompilationDemo()) { Order = 10 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
