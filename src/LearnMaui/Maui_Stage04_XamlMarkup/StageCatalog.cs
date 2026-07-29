using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage04_XamlMarkup;

internal static class StageCatalog
{
    public static LearningStage Create(ProfileViewModel profile)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "声明式 XAML",
                "XAML 描述对象树、属性与绑定，code-behind 只负责 InitializeComponent、BindingContext 和必须触碰视觉元素的胶水。",
                "XamlDemoView.xaml + XamlDemoView.xaml.cs"),
            new(LearningLevel.Beginner, "x:Class 与 partial 类",
                "x:Class 必须与 code-behind 的命名空间和类型一致；生成的 InitializeComponent 把编译后的 XAML 膨胀进 partial 类。",
                "x:Class=\"LearnMaui...XamlDemoView\""),
            new(LearningLevel.Beginner, "x:Name 与命名范围",
                "x:Name 只用于焦点、动画、测量或平台胶水。DataTemplate 有独立命名范围；展示数据优先绑定而非通过字段赋值。",
                "XamlDemoView uses bindings and needs no named controls"),
            new(LearningLevel.Beginner, "x:DataType 编译绑定",
                "根视图声明 ViewModel 类型，DataTemplate 重新声明 item 类型，使错误路径在编译期暴露并降低反射/AOT 风险。",
                "x:DataType=ProfileViewModel and DataTemplate x:DataType=ProfileItem"),
            new(LearningLevel.Beginner, "ResourceDictionary 作用域",
                "资源从控件、父级、页面到 Application 逐层查找。颜色、间距和样式 token 集中管理，避免页面散落魔法值。",
                "ContentView.Resources defines AccentColor and CaptionStyle"),
            new(LearningLevel.Intermediate, "Static 与 Dynamic Resource",
                "StaticResource 加载时解析一次，适合固定样式；DynamicResource 保留动态引用，适合运行时换肤。不要无差别动态化。",
                "Style={StaticResource CaptionStyle}; TextColor={DynamicResource AccentColor}"),
            new(LearningLevel.Intermediate, "AppThemeBinding",
                "浅色/深色环境值由 AppThemeBinding 自动切换；主题属于环境与 View，不应泄漏成领域模型中的 Color。",
                "CaptionStyle TextColor uses AppThemeBinding"),
            new(LearningLevel.Intermediate, "OnPlatform 与 OnIdiom",
                "少量 Padding、字号和资源差异可声明式分支；安全区与键盘不用硬编码，布局结构完全不同则换模板。",
                "Grid.Padding OnPlatform; Label.FontSize OnIdiom"),
            new(LearningLevel.Intermediate, "Markup Extension",
                "扩展在 XAML 属性赋值时提供值，应纯、快速且无副作用。网络、DI 业务和可变流程不应放进 ProvideValue。",
                "UpperExtension : IMarkupExtension<string>"),
            new(LearningLevel.Intermediate, "默认 XAML 编译",
                "MAUI 10 默认编译 XAML，未知类型/属性提前成为构建错误；生产主线不关闭 XamlC。",
                "MauiStrictXamlCompilation=true"),
            new(LearningLevel.Advanced, "XAML Source Generator",
                "MauiXamlInflator=SourceGen 生成强类型膨胀代码；提交前读取 obj/Generated，并记录第三方控件或 Hot Reload 的兼容边界。",
                "MauiXamlInflator=SourceGen + EmitCompilerGeneratedFiles"),
            new(LearningLevel.Advanced, "C# Markup 对等实现",
                "高度动态 UI 可用 C# Markup，并以强类型表达式 Bind 到同一 ViewModel。它是 View 技术选择，不替代 MVVM。",
                "MarkupDemoView uses CommunityToolkit.Maui.Markup.Bind"),
            new(LearningLevel.Advanced, "XAML 与 Markup 选型",
                "大型静态表单、设计协作优先 XAML；动态视觉树和全 C# 审查可选 Markup；两者共享 VM、服务和设计 token。",
                "XamlDemoView and MarkupDemoView consume ProfileViewModel"),
            new(LearningLevel.Advanced, "MVVM 装配",
                "ViewModel、XAML View 和 Markup View 通过 DI 注册；View 构造时接受 VM，禁止在页面内部 new VM 绕过生命周期管理。",
                "MauiProgram.Services AddTransient registrations"),
            new(LearningLevel.Advanced, "薄 code-behind",
                "允许初始化、焦点、动画和短期视觉事件；HTTP、SQL、文件与领域校验留在服务/VM，导航和弹窗经接口。",
                "XamlDemoView.xaml.cs only initializes and assigns BindingContext"),
            new(LearningLevel.Expert, "BindingContext 流动",
                "BindingContext 沿逻辑树继承；模板和显式变更上下文的子树需重新声明 x:DataType，避免编译期类型与运行时对象错位。",
                "ProfileViewModel root → ProfileItem DataTemplate boundary"),
            new(LearningLevel.Expert, "生成与 AOT 边界",
                "编译绑定和 SourceGen 把更多错误前移并减少运行时反射；动态加载 XAML、字符串路径绑定和复杂 MultiBinding 需要单独审查 trim/AOT。",
                "strong types and generated inflate path"),
            new(LearningLevel.Expert, "Global xmlns 与兼容",
                "MAUI 10 可减少重复 xmlns，但传统显式声明最清楚且第三方控件排障容易；团队应统一风格，重名类型显式前缀消歧。",
                "XamlDemoView keeps explicit xmlns declarations"),
            new(LearningLevel.Expert, "过时 API 清理",
                "新代码使用 Border 代替 Frame、FontImageSource 代替旧 FontImageExtension，并避免 Page.IsBusy、MessagingCenter 与运行时 XAML 主线。",
                "repository scan for obsolete XAML and APIs"),
        ];

        LearningLab[] labs =
        [
            new("构造 XAML View", "实例化 SourceGen/XamlC 编译的视图并检查绑定上下文。",
                _ =>
                {
                    XamlDemoView view = new(profile);
                    return Task.FromResult(
                        $"类型：{view.GetType().Name}\nBindingContext：{view.BindingContext?.GetType().Name}\n技能数：{profile.Skills.Count}");
                }),
            new("构造 C# Markup View", "用同一 ViewModel 构造代码 UI，证明架构层不随 View 技术分叉。",
                _ =>
                {
                    MarkupDemoView view = new(profile);
                    return Task.FromResult(
                        $"类型：{view.GetType().Name}\n共享 VM：{ReferenceEquals(profile, view.BindingContext)}");
                }),
            new("验证属性通知", "修改 ViewModel，并确认 INotifyPropertyChanged 报告精确属性名。",
                _ =>
                {
                    string? changed = null;
                    profile.PropertyChanged += Handler;
                    profile.DisplayName = $"Learner {DateTimeOffset.Now:HHmmss}";
                    profile.PropertyChanged -= Handler;
                    return Task.FromResult($"通知属性：{changed}\n新值：{profile.DisplayName}");

                    void Handler(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
                    {
                        changed = args.PropertyName;
                    }
                }),
            new("验证 Markup Extension", "直接执行纯 UpperExtension，证明无 UI/网络副作用。",
                _ =>
                {
                    UpperExtension extension = new() { Text = "maui sourcegen" };
                    return Task.FromResult(extension.ProvideValue(NullServiceProvider.Instance));
                }),
        ];

        return new LearningStage(
            4,
            "XAML、C# Markup 与架构选型",
            "第3部分-阶段4-XAML-Markup-架构选型.md",
            topics,
            labs);
    }

    private sealed class NullServiceProvider : IServiceProvider
    {
        public static NullServiceProvider Instance { get; } = new();

        public object? GetService(Type serviceType)
        {
            return null;
        }
    }
}
