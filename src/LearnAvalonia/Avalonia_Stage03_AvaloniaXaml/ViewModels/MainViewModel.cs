using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage03_AvaloniaXaml.Pages;

namespace LearnAvalonia.Avalonia_Stage03_AvaloniaXaml.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 文件/命名空间/根约定", "第3部分-阶段3-Avalonia-XAML.md §1", "解释 axaml 命名空间与 x:Class/Name/Key", () => new FileNamespaceDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 内容属性/属性元素/附加属性", "第3部分-阶段3-Avalonia-XAML.md §2", "说明 content property 与附加属性", () => new ContentPropertyDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 标记扩展", "第3部分-阶段3-Avalonia-XAML.md §3", "默写 12 Binding 语义与 BindingBase", () => new MarkupExtensionDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 x:DataType 与 x:CompileBindings", "第3部分-阶段3-Avalonia-XAML.md §4", "说明 DataType 作用域与编译绑定开关", () => new DataTypeCompileDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 资源 Static/Dynamic/Theme", "第3部分-阶段3-Avalonia-XAML.md §5", "区分 StaticResource/DynamicResource/ThemeResource", () => new ResourceKindsDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 样式与 ControlTheme", "第3部分-阶段3-Avalonia-XAML.md §6", "不把 WPF Trigger 硬套 Avalonia", () => new StyleControlThemeDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 design-time 与 Hot Reload", "第3部分-阶段3-Avalonia-XAML.md §7", "说明设计时数据与 Hot Reload 预期", () => new DesignTimeDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 XAML 编译/IL/动态加载", "第3部分-阶段3-Avalonia-XAML.md §8", "说明编译进 IL 与 Loader 的 AOT 边界", () => new CompileIlDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 axaml 与纯 C# 对照", "第3部分-阶段3-Avalonia-XAML.md §9", "完成 PersonCard 双实现", () => new AxamlVsCsDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 与其他 UI 技术对比", "第3部分-阶段3-Avalonia-XAML.md §10", "概念对比 WPF/WinUI/MAUI/纯 C#", () => new TechCompareDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 MVVM 页面骨架", "第3部分-阶段3-Avalonia-XAML.md §11", "写完整 MVVM 页面骨架", () => new MvvmSkeletonDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 跨平台差异表", "第3部分-阶段3-Avalonia-XAML.md §12", "覆盖 5 类平台 XAML 差异", () => new XamlPlatformDiffDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 Avalonia 12 最新变化", "第3部分-阶段3-Avalonia-XAML.md §13", "列出 12/12.1 XAML 关键变化", () => new Xaml12ChangesDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§14 过时与禁止", "第3部分-阶段3-Avalonia-XAML.md §14", "点名 8 项 XAML 禁令与替代", () => new XamlDeprecatedDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§15 最小实验 A-E", "第3部分-阶段3-Avalonia-XAML.md §15", "完成 5 个最小实验", () => new XamlExperimentsDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§16 验收标准", "第3部分-阶段3-Avalonia-XAML.md §16", "勾选 9 条验收清单", () => new XamlAcceptanceDemo()) { Order = 16 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
