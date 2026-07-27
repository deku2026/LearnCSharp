using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 三类（+Panel/绘制）对照", "第6部分-阶段14-自定义控件.md §1", "UserControl/Templated/Content 对照", () => new ControlTypesDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 UserControl 快路径", "第6部分-阶段14-自定义控件.md §2.1", "App 内复用块；难换肤", () => new UserControlDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 自定义 Panel", "第6部分-阶段14-自定义控件.md §2.2", "MeasureOverride/ArrangeOverride", () => new CustomPanelDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 自定义绘制 Control", "第6部分-阶段14-自定义控件.md §2.3", "StyledProperty+AffectsRender+AutomationPeer", () => new CustomDrawControlDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§2.4 TemplatedControl 结构", "第6部分-阶段14-自定义控件.md §2.4", "OnApplyTemplate + ControlTheme", () => new TemplatedControlDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§2.5 TemplateBinding vs Binding", "第6部分-阶段14-自定义控件.md §2.5", "模板内必须 TemplateBinding 控件属性", () => new TemplateBindingDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§2.6 Named parts 约定", "第6部分-阶段14-自定义控件.md §2.6", "PART_ 前缀；卸载旧事件", () => new NamedPartsDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§2.7 伪类（Pseudoclasses）", "第6部分-阶段14-自定义控件.md §2.7", ":pointerover/:pressed 自定义", () => new PseudoclassesDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§2.8 属性：Styled/Direct/Attached", "第6部分-阶段14-自定义控件.md §2.8", "三类属性 + Affects* 注册", () => new PropertyTypesDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§2.9 路由事件", "第6部分-阶段14-自定义控件.md §2.9", "RoutedEvent.Register + AddHandler", () => new RoutedEventDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§2.10 命令", "第6部分-阶段14-自定义控件.md §2.10", "Command 属性 + CanExecute", () => new CommandDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§2.11 AutomationPeer", "第6部分-阶段14-自定义控件.md §2.11", "OnCreateAutomationPeer 名称/类型", () => new AutomationPeerDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§2.12 默认主题与组件库打包", "第6部分-阶段14-自定义控件.md §2.12", "Themes/Fluent/Index.axaml + StyleInclude", () => new LibraryPackagingDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§2.13 Design-time", "第6部分-阶段14-自定义控件.md §2.13", "Design.DataContext + Design.IsDesignMode", () => new DesignTimeDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§2.14 AOT 友好", "第6部分-阶段14-自定义控件.md §2.14", "避免反射；显式 axaml 编译", () => new AotFriendlyDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§3 完整端到端：RatingControl", "第6部分-阶段14-自定义控件.md §3", "逻辑+ControlTheme+应用+单测", () => new RatingControlEndToEndDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§4 跨平台差异", "第6部分-阶段14-自定义控件.md §4", "模板/无障碍/AOT 各平台抽检", () => new PlatformDiffDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§5 Avalonia 12 / 12.1 变化", "第6部分-阶段14-自定义控件.md §5", "ControlTheme/伪类/AOT 改进", () => new Avalonia12ChangesDemo()) { Order = 18 });
        Topics.Add(new TopicItem("§6 过时与禁止", "第6部分-阶段14-自定义控件.md §6", "禁默认反射 ViewLocator/旧 Trigger", () => new DeprecatedApisDemo()) { Order = 19 });
        Topics.Add(new TopicItem("§11 组件库工程化", "第6部分-阶段14-自定义控件.md §11", "版本/API 稳定性/测试金字塔/文档", () => new LibraryEngineeringDemo()) { Order = 20 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
