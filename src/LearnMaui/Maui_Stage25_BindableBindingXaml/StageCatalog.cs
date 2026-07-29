using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage25_BindableBindingXaml;

internal static class StageCatalog
{
    public static LearningStage Create(BindingLabViewModel viewModel)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "BindableObject",
                "Element/VisualElement/View/Control 都建立在 BindableObject 上；它提供属性存储、绑定、样式和值来源，而非普通 CLR 字段。",
                "SignalBadge : ContentView"),
            new(LearningLevel.Beginner, "BindableProperty 元数据",
                "Create 声明名称、类型、owner、默认值、默认 BindingMode、validate/coerce/propertyChanging/propertyChanged；静态 readonly 字段只注册一次。",
                "TextProperty and SeverityProperty"),
            new(LearningLevel.Beginner, "CLR 包装器",
                "CLR 属性只调用 GetValue/SetValue，不放业务逻辑；所有入口包括样式、绑定和动画都应通过 BindableProperty 回调获得一致行为。",
                "SignalBadge.Text/Severity"),
            new(LearningLevel.Beginner, "BindingContext 继承",
                "Element 从逻辑父级继承 BindingContext，显式设置会截断继承；模板、Popup、资源对象和平台树不应想当然。",
                "MainPage BindingContext -> XAML children"),
            new(LearningLevel.Intermediate, "经典与编译绑定",
                "经典 Binding 用字符串路径和运行时反射；x:DataType 让 XAML 编译器生成强类型访问，错误前移并改善 trim/AOT。",
                "MainPage x:DataType"),
            new(LearningLevel.Intermediate, "绑定模式",
                "OneWay 源到目标、TwoWay 双向、OneTime 只读一次、OneWayToSource 目标回写；按数据流最小化选择，避免无意义 TwoWay。",
                "explicit OneWay bindings"),
            new(LearningLevel.Intermediate, "通知契约",
                "INotifyPropertyChanged 必须在值实际变化后报告准确属性名；集合增删用 ObservableCollection，项属性仍需自身通知。",
                "BindingLabViewModel.SetProperty"),
            new(LearningLevel.Intermediate, "值优先级",
                "本地值/绑定、样式、默认值和动画具有有效值优先级；ClearValue 移除本地来源后才会回落到样式/默认值。",
                "ClearValue lab"),
            new(LearningLevel.Intermediate, "验证与强制",
                "validate 拒绝非法值，coerce 归一化，propertyChanging/Changed 处理边界副作用；回调必须快速且避免重入。",
                "Text length validation"),
            new(LearningLevel.Advanced, "XAML 编译",
                "默认 XamlC 把标记转为构造/设置路径；严格模式把未编译绑定提升为警告/错误，让质量门可自动化。",
                "MauiStrictXamlCompilation"),
            new(LearningLevel.Advanced, "XAML SourceGen",
                "MauiXamlInflator=SourceGen 生成可审查的初始化代码，减少运行时膨胀与反射；仍需真机验证资源和模板。",
                "csproj SourceGen"),
            new(LearningLevel.Advanced, "DataTemplate 类型",
                "模板建立自己的项 BindingContext，必须在 DataTemplate 上声明正确 x:DataType；不要误用父页面 ViewModel 类型。",
                "shared typed topic model is code-created; requirement documented"),
            new(LearningLevel.Advanced, "RelativeSource",
                "引用祖先、Self 或 TemplatedParent 时显式表达来源；复杂跨树引用优先提升为 VM 状态或控件 API。",
                "binding source discipline"),
            new(LearningLevel.Advanced, "MultiBinding",
                "多值转换仍有运行时边界；可预计算 VM 属性时优先强类型属性，让通知依赖和 AOT 行为更明确。",
                "Summary precomputed in view-model"),
            new(LearningLevel.Advanced, "逻辑树与平台树",
                "BindingContext 沿 MAUI 逻辑树流动，Handler 将虚拟属性映射到原生树；看到原生控件不代表它拥有 BindingContext。",
                "BindingTrace"),
            new(LearningLevel.Expert, "属性到像素调用链",
                "PropertyChanged→compiled getter→SetValueCore→BindableProperty callback→Handler.UpdateValue→原生 invalidation；分层断点定位丢失更新。",
                "BindingTrace.PropertyToPixels"),
            new(LearningLevel.Expert, "绑定更新但 UI 不变",
                "先查通知/线程，再查 BindingContext/路径与 Converter，随后查有效值来源、Handler mapper、原生属性和 measure/draw。",
                "trace lab"),
            new(LearningLevel.Expert, "AOT 红线",
                "运行时 LoadFromXaml、动态类型/路径和未标注反射让 Native AOT 不可预测；主路径用静态 XAML、SourceGen 和编译绑定。",
                "strict SourceGen project"),
            new(LearningLevel.Expert, "控件设计",
                "BindableProperty owner/type/default 必须稳定，回调不抓页面，默认对象避免跨实例共享可变状态，事件有对称释放。",
                "SignalBadge"),
            new(LearningLevel.Expert, "验收证据",
                "故意改错绑定应在编译期失败；查看 generated 输出，验证 ClearValue、BindingContext 继承和属性链断点。",
                "compiled XAML + runnable labs"),
        ];

        LearningLab[] labs =
        [
            new("推动编译绑定", "更新 ViewModel，并通过 INotifyPropertyChanged 推动真实 XAML 绑定。",
                unusedToken =>
                {
                    viewModel.Advance();
                    return Task.FromResult($"{viewModel.Summary}; {viewModel.Severity}");
                }),
            new("验证 BindableProperty", "设置合法值并验证超长文本被 validateValue 拒绝。",
                unusedToken =>
                {
                    SignalBadge badge = new() { Text = "valid", Severity = SignalSeverity.Attention };
                    bool rejected;
                    try
                    {
                        badge.Text = new string('x', 161);
                        rejected = false;
                    }
                    catch (ArgumentException)
                    {
                        rejected = true;
                    }

                    return Task.FromResult($"text={badge.Text}; severity={badge.Severity}; invalid rejected={rejected}");
                }),
            new("观察 ClearValue", "移除本地值后确认有效值回落到 BindableProperty 默认值。",
                unusedToken =>
                {
                    SignalBadge badge = new() { Text = "local" };
                    string before = badge.Text;
                    badge.ClearValue(SignalBadge.TextProperty);
                    return Task.FromResult($"before={before}; after='{badge.Text}'; default restored={badge.Text.Length == 0}");
                }),
            new("验证 BindingContext 继承", "把子元素加入逻辑树，确认其继承父级上下文；显式设置可覆盖。",
                unusedToken =>
                {
                    object inherited = new();
                    VerticalStackLayout parent = new() { BindingContext = inherited };
                    Label child = new();
                    parent.Children.Add(child);
                    bool inheritedOk = ReferenceEquals(child.BindingContext, inherited);
                    child.BindingContext = "local";
                    return Task.FromResult($"inherited={inheritedOk}; override={child.BindingContext}");
                }),
            new("打印属性到像素链", "输出六层调用链，供源码断点逐层验证。",
                unusedToken => Task.FromResult(string.Join(
                    '\n',
                    BindingTrace.PropertyToPixels().Select(
                        step => $"{step.Order}. {step.Layer}: {step.Operation}")))),
        ];

        return new LearningStage(
            25,
            "BindableObject、Binding 与 XAML 编译",
            "第9部分-02-BindableObject-Binding-XAML编译.md",
            topics,
            labs);
    }
}
