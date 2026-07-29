using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage08_StylesThemesAnimation;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "Style 与 Setter",
                "Style 按 TargetType 集中一组属性；显式 key 控制应用点，隐式样式影响同作用域所有控件。Setter 不承载异步业务。",
                "App.xaml BaseActionStyle"),
            new(LearningLevel.Beginner, "BasedOn 与继承",
                "具体样式 BasedOn 基础样式复用间距、圆角和文字色，只覆盖品牌背景。设计 token 与组件样式分层。",
                "PrimaryActionStyle BasedOn BaseActionStyle"),
            new(LearningLevel.Beginner, "资源字典与查找",
                "资源按元素、父树、页面、App 顺序查找；大型应用把颜色、字体、控件样式和主题字典分册合并，并明确键冲突。",
                "Application.Resources ResourceDictionary"),
            new(LearningLevel.Beginner, "语义色与主题",
                "颜色命名表达 Surface/Text/Primary/Error 等语义，而不是 Blue500 用途；AppThemeBinding 随系统主题更新。",
                "Label TextColor uses AppThemeBinding"),
            new(LearningLevel.Beginner, "Static 与 Dynamic",
                "固定样式用 StaticResource；运行时品牌/主题 token 用 DynamicResource。替换 BrandPrimary 后引用控件自动更新。",
                "Button/Border consume DynamicResource BrandPrimary"),
            new(LearningLevel.Intermediate, "应用主题覆盖",
                "UserAppTheme 可跟随系统或由用户选择 Light/Dark/Unspecified；偏好持久化后在启动早期恢复，避免首帧闪烁。",
                "ThemeController.Toggle"),
            new(LearningLevel.Intermediate, "Triggers",
                "PropertyTrigger 看控件属性，DataTrigger 看绑定数据，MultiTrigger 组合条件；进入/退出 Action 只做短视觉效果，不写领域流程。",
                "Visual state is preferred for reusable control states"),
            new(LearningLevel.Intermediate, "VisualStateManager",
                "CommonStates 统一 Normal、Disabled、Focused、PointerOver；自定义 Selected 状态让多个 Setter 原子切换，避免散乱事件改属性。",
                "InteractionDemoView CommonStates"),
            new(LearningLevel.Intermediate, "Behavior",
                "Behavior 封装可复用 View 行为，Attached/Detaching 成对订阅解除；它不应成为隐藏 DI 容器或业务服务定位器。",
                "SelectAllBehavior"),
            new(LearningLevel.Intermediate, "GestureRecognizers",
                "Tap、Swipe、Pinch、Pan、Pointer、Drag/Drop 统一表达交互；同一区域手势冲突需产品优先级，且提供键盘/读屏等效入口。",
                "TapGestureRecognizer + PointerGestureRecognizer"),
            new(LearningLevel.Intermediate, "KeyboardAccelerator",
                "桌面快捷键属于同一 Command 的另一输入入口，不复制业务逻辑；避免抢占系统/可访问性组合键，并在菜单显示。",
                "gesture and accelerator should share a command"),
            new(LearningLevel.Advanced, "DataTemplate 与 Selector",
                "模板描述模型外观，Selector 只按稳定类型/状态选择并复用 DataTemplate，不能每项返回新模板导致虚拟化缓存失效。",
                "CardTemplateSelector has reusable templates"),
            new(LearningLevel.Advanced, "ControlTemplate",
                "ControlTemplate 重塑控件视觉结构而保留公共 API；TemplateBinding 连接宿主属性，业务数据仍经 BindingContext。",
                "control templating is separate from item DataTemplate"),
            new(LearningLevel.Advanced, "Border、Shadow 与 Clip",
                "Border 是现代容器；Shadow 需克制半径和透明度；Clip 可裁剪复杂形状但增加渲染成本，列表中尤其需要测量。",
                "InteractionDemoView Border.Shadow"),
            new(LearningLevel.Advanced, "覆盖层选型",
                "Alert 用异步 DisplayAlertAsync；页内 Overlay 适合加载/轻提示；Popup 适合轻模态；完整流程用 Shell Modal。返回键与焦点必须定义。",
                "choose overlay by modality and ownership"),
            new(LearningLevel.Advanced, "现代异步动画",
                "MAUI 10 使用 FadeToAsync/ScaleToAsync 等可等待 API；串行直接 await，并行 Task.WhenAll，页面离开取消。",
                "AnimationCoordinator.PlayEnterAsync"),
            new(LearningLevel.Advanced, "动画取消",
                "当 API 没有 Token 重载时对 Task 使用 WaitAsync(token)，取消后调用 CancelAnimations 清理视觉状态；CTS 所有者负责 Dispose。",
                "WaitAsync + View.CancelAnimations"),
            new(LearningLevel.Expert, "减少动态效果",
                "动画是增强而非状态来源。产品应尊重系统减少动态设置，跳过或缩短位移/缩放，并保证无动画时信息仍完整。",
                "AnimationCoordinator can be bypassed by policy"),
            new(LearningLevel.Expert, "AutomationId 与可测试 UI",
                "给稳定交互目标设置语义化 AutomationId，不按布局位置命名；E2E 用它定位，单元测试仍覆盖 VM/状态机。",
                "Stage08Card and Stage08PrimaryAction"),
            new(LearningLevel.Expert, "平台差异与废弃清理",
                "Pointer/快捷键在桌面更重要，手势在移动更常见；Alert、工具栏和阴影外观不同。禁止旧同步动画、Frame、MessagingCenter 和泄漏 Behavior。",
                "platform-aware enhancement with shared commands"),
        ];

        LearningLab[] labs =
        [
            new("构造交互视图", "编译并实例化包含 Style、VSM、Behavior、手势、Shadow 与 AutomationId 的 XAML。",
                _ =>
                {
                    InteractionDemoView view = new();
                    return Task.FromResult($"构造成功：{view.GetType().Name}");
                }),
            new("切换应用主题", "切换 UserAppTheme，验证应用级覆盖不进入领域模型。",
                _ =>
                {
                    AppTheme theme = ThemeController.Toggle();
                    return Task.FromResult($"UserAppTheme={theme}");
                }),
            new("替换动态品牌色", "修改 App ResourceDictionary 中的 BrandPrimary，DynamicResource 消费者会更新。",
                _ =>
                {
                    Color color = Color.FromArgb("#7C3AED");
                    ThemeController.SetBrandColor(color);
                    return Task.FromResult($"BrandPrimary={color.ToHex()}");
                }),
            new("运行可取消动画", "并发执行 FadeToAsync 与 ScaleToAsync，并通过 WaitAsync 接收取消。",
                async token =>
                {
                    using AnimationCoordinator coordinator = new();
                    Label target = new();
                    await coordinator.PlayEnterAsync(target, token);
                    return $"Opacity={target.Opacity:F1}, Scale={target.Scale:F1}";
                }),
            new("验证模板复用", "让 Selector 对相同模型类型返回同一 DataTemplate 实例。",
                _ =>
                {
                    CardTemplateSelector selector = new();
                    Label container = new();
                    DataTemplate first = selector.SelectTemplate(
                        new ArticleCard("A", "a"),
                        container);
                    DataTemplate second = selector.SelectTemplate(
                        new ArticleCard("B", "b"),
                        container);
                    return Task.FromResult($"Template reused={ReferenceEquals(first, second)}");
                }),
        ];

        return new LearningStage(
            8,
            "样式、主题、动画与手势",
            "第3部分-阶段8-样式主题动画手势.md",
            topics,
            labs);
    }
}
