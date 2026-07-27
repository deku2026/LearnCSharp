using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage08_StylesThemes.Pages;

namespace LearnAvalonia.Avalonia_Stage08_StylesThemes.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§A1-A2 五个概念必须分清", "第4部分-阶段8-样式主题与ControlTheme.md §A1", "闭卷讲清 A1 七行概念差异", () => new FiveConceptsDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§B1-B2 Style 与 Selector", "第4部分-阶段8-样式主题与ControlTheme.md §B1", "使用类型/类/名/子/伪类选择器", () => new StyleSelectorDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§B3-B4 优先级与 Classes 驱动", "第4部分-阶段8-样式主题与ControlTheme.md §B3", "解释 Selector 优先级与作用域", () => new PriorityClassesDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§C1-C2 FluentTheme 与 ThemeVariant", "第4部分-阶段8-样式主题与ControlTheme.md §C1", "切换 ThemeVariant 即时生效", () => new FluentThemeVariantDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§D1-D4 Static·Dynamic·ThemeResource", "第4部分-阶段8-样式主题与ControlTheme.md §D1", "演示 Static/Dynamic/ThemeResource 差异", () => new ResourceKindsDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§E1-E2 ControlTheme 与 BasedOn", "第4部分-阶段8-样式主题与ControlTheme.md §E1", "使用 Theme 属性指定 ControlTheme", () => new ControlThemeBasedOnDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§E3-E5 ControlTemplate 与无外观控件", "第4部分-阶段8-样式主题与ControlTheme.md §E3", "提供可替换 ControlTheme", () => new ControlTemplateDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§F1-F3 完整浅深色应用示例", "第4部分-阶段8-样式主题与ControlTheme.md §F1", "完整浅色/深色无页面级写死", () => new LightDarkAppDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§F4-F5 VM 切换主题与跨平台抽检", "第4部分-阶段8-样式主题与ControlTheme.md §F4", "VM 切换 ThemeVariant 即时生效", () => new VmThemeSwitchDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§G1-G4 设计系统落地", "第4部分-阶段8-样式主题与ControlTheme.md §G1", "Token 层次与 UI 只绑语义层", () => new DesignSystemDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§H1-H3 12 变化·过时·实验", "第4部分-阶段8-样式主题与ControlTheme.md §H1", "列出 12/12.1 关键变化", () => new ChangesDeprecatedDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§I 平台差", "第4部分-阶段8-样式主题与ControlTheme.md §I", "区分桌面/移动/Browser 主题能力", () => new PlatformDiffDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§J1-J2 最小实验·验收", "第4部分-阶段8-样式主题与ControlTheme.md §J1", "勾选验收清单 8 条", () => new AcceptanceChecklistDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§J3-J4 源码阅读·官方链接", "第4部分-阶段8-样式主题与ControlTheme.md §J3", "列出源码阅读检索点", () => new SourceReadingDemo()) { Order = 14 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
