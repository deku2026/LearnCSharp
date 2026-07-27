using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage11_PagesNavigation.Pages;

namespace LearnAvalonia.Avalonia_Stage11_PagesNavigation.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 为什么需要 Pages", "第5部分-阶段11-Pages与导航架构.md §1", "区分 Window 壳与 Pages 跨平台导航", () => new StageOverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 页面类型一览", "第5部分-阶段11-Pages与导航架构.md §2.1", "默写 Pages 类型职责表", () => new PageTypesDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 ContentPage 最小页", "第5部分-阶段11-Pages与导航架构.md §2.2", "写 ContentPage + Header + CommandBar", () => new ContentPageDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 NavigationPage 与 INavigation", "第5部分-阶段11-Pages与导航架构.md §2.3", "PushAsync/PopAsync/ModalStack 心智", () => new NavigationPageDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§2.4 导航事件", "第5部分-阶段11-Pages与导航架构.md §2.4", "Pushed/Popped 与页级 Appearing", () => new NavigationEventsDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§2.5 TabbedPage", "第5部分-阶段11-Pages与导航架构.md §2.5", "Tab 根 + 每 Tab 内嵌 NavigationPage", () => new TabbedPageDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§2.6 DrawerPage", "第5部分-阶段11-Pages与导航架构.md §2.6", "Drawer+Navigation 汉堡/返回切换", () => new DrawerPageDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§2.7 CarouselPage 与 PipsPager", "第5部分-阶段11-Pages与导航架构.md §2.7", "引导流程用 Carousel 非深层导航", () => new CarouselPageDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§2.8 CommandBar", "第5部分-阶段11-Pages与导航架构.md §2.8", "页级动作条与导航栏分工", () => new CommandBarDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§2.9 Transitions", "第5部分-阶段11-Pages与导航架构.md §2.9", "PageSlide/CrossFade 与焦点保持", () => new TransitionsDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§2.10 Safe area 与 EffectiveBarHeight", "第5部分-阶段11-Pages与导航架构.md §2.10", "刘海/状态栏/主页指示条避让", () => new SafeAreaDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§2.11 完整 NavigationPage 端到端场景", "第5部分-阶段11-Pages与导航架构.md §2.11", "首页→详情→模态登录→返回全链", () => new EndToEndDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§2.12 ReactiveUI 路由（选学）", "第5部分-阶段11-Pages与导航架构.md §2.12", "RoutingState 与官方栈不混用", () => new ReactiveUiRoutingDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§2.13 桌面 document/tab/multi-window", "第5部分-阶段11-Pages与导航架构.md §2.13", "INavigationService 多实现共享契约", () => new DesktopNavigationDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§3 跨平台差异", "第5部分-阶段11-Pages与导航架构.md §3", "移动/Desktop/Browser 主壳差异", () => new PlatformDiffDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§4 Avalonia 12 / 12.1 变化", "第5部分-阶段11-Pages与导航架构.md §4", "Pages 全家入盒与异步 INavigation", () => new Avalonia12ChangesDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§5 过时与禁止", "第5部分-阶段11-Pages与导航架构.md §5", "禁默认反射 ViewLocator 与同步阻塞", () => new DeprecatedApisDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§6 最小实验清单", "第5部分-阶段11-Pages与导航架构.md §6", "完成 NavigationPage/Drawer/Tab 4 实验", () => new MinExperimentsDemo()) { Order = 18 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
