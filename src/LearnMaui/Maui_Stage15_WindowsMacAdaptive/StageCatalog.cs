using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage15_WindowsMacAdaptive;

internal static class StageCatalog
{
    public static LearningStage Create(
        WindowWorkspaceRegistry workspaces,
        IDesktopPlatformService desktopPlatform)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "窗口宽度而非设备标签",
                "手机可分屏、桌面窗口可变窄，断点基于可用 Width/Height；DeviceIdiom 只做输入能力或默认值提示。",
                "AdaptiveLayoutPolicy.Evaluate"),
            new(LearningLevel.Beginner, "Compact / Medium / Expanded",
                "紧凑单栏+底部导航，中等可用收起 rail，宽屏 master-detail；断点是产品内容阈值，不是机型清单。",
                "AdaptiveMode decisions"),
            new(LearningLevel.Beginner, "尺寸变化",
                "Window.SizeChanged 重新计算视觉状态，避免每像素重建大树；关闭窗口时解除事件订阅。",
                "WindowWorkspaceRegistry.Attach"),
            new(LearningLevel.Beginner, "桌面导航",
                "手机 Shell Tabs/Flyout 与桌面 rail/master-detail共享导航意图，View 决定呈现；详情可同窗 pane 或新 Window。",
                "NavigationPresentation + CreateDetailWindow"),
            new(LearningLevel.Beginner, "输入模态共存",
                "同一操作同时支持触摸、鼠标、触控板和键盘；Hover 只能增强，不能隐藏触屏用户必需功能。",
                "CreatePointerTarget"),
            new(LearningLevel.Intermediate, "Pointer 与 ToolTip",
                "PointerEntered/Exited 提供悬停状态，ToolTip 解释无文字图标；触控目标仍需足够大且命令一致。",
                "PointerGestureRecognizer + ToolTipProperties"),
            new(LearningLevel.Intermediate, "右键与长按",
                "桌面 context menu 与移动长按映射同一上下文操作，危险操作二次确认并保留键盘可达。",
                "platform context surface shares commands"),
            new(LearningLevel.Intermediate, "键盘焦点",
                "Tab 顺序跟视觉/任务顺序，焦点样式清晰；弹窗/新 pane 打开后焦点进入合理目标，关闭后返回触发点。",
                "focus is a first-class desktop state"),
            new(LearningLevel.Intermediate, "KeyboardAccelerator",
                "使用现代 KeyboardAccelerator，把 Ctrl/Cmd 快捷键绑定同一 Command；文本输入、系统快捷键与平台修饰键需避冲突。",
                "central command service rather than per-page business code"),
            new(LearningLevel.Intermediate, "MenuBar",
                "File/Edit/View 等桌面菜单由 MenuBarItem/MenuFlyoutItem 组成，启用状态来自 Command.CanExecute，动态菜单有清晰所有者。",
                "DesktopMenuFactory.Create"),
            new(LearningLevel.Advanced, "多窗口 API",
                "每个 Window 有独立 Page/导航树/尺寸/选中项；Application.OpenWindow/CloseWindow 是平台能力入口，不能复用同一视觉树实例。",
                "WindowWorkspaceRegistry.CreateDetailWindow"),
            new(LearningLevel.Advanced, "每窗状态",
                "应用级缓存可共享，窗口级选择/导航/CTS 必须隔离。Registry 用 WeakReference 与 Destroying 清理，避免静态当前窗口。",
                "WindowWorkspace per Window"),
            new(LearningLevel.Advanced, "Windows 原生窗口",
                "Windows AppWindow/Presenter 控制最小化、最大化、尺寸与标题栏；只在 Platforms/Windows 下实现并用接口暴露产品意图。",
                "IDesktopPlatformService boundary"),
            new(LearningLevel.Advanced, "Mac Catalyst 场景",
                "Mac Catalyst 使用 scene/window 生命周期、菜单和指针惯例；不能把 iPhone 固定宽度 UI 放大冒充桌面。",
                "MACCATALYST platform branch"),
            new(LearningLevel.Advanced, "SafeArea 与桌面 chrome",
                "移动安全区与桌面标题栏/窗口 inset 来源不同；布局消费系统提供的边界，不硬编码刘海或标题栏像素。",
                "window content uses adaptive available size"),
            new(LearningLevel.Advanced, "OnIdiom / OnPlatform",
                "少量值差异可声明式选择；结构级响应由断点/VisualState；业务能力通过接口下沉而不是页面 #if 泥潭。",
                "platform code isolated in DesktopPlatformService"),
            new(LearningLevel.Expert, "平台差下沉",
                "#if 允许在启动注册、Platforms/partial 和平台桥实现；VM/domain 不出现 Windows/Mac/Android 类型。",
                "IDesktopPlatformService keeps native types out of UI model"),
            new(LearningLevel.Expert, "关闭与退出",
                "关闭一个 Window 不等于退出进程；最后窗口、未保存草稿和后台同步要按平台/产品定义，不能依赖强制退出 API。",
                "Destroying removes one workspace only"),
            new(LearningLevel.Expert, "四平台构建矩阵",
                "Android/Windows 本机编译运行，iOS/Mac 需 Mac 工具链；同一共享策略要配平台 smoke test、菜单/返回/窗口行为。",
                "current acceptance target is net10.0-android"),
            new(LearningLevel.Expert, "响应式验收",
                "窄窗、分屏、横竖屏、125–200% 缩放、键盘/鼠标/触控、多窗独立状态都需验证；避免仅模拟全屏桌面。",
                "pure breakpoints plus Window event evidence"),
        ];

        LearningLab[] labs =
        [
            new("验证断点策略", "对手机窄窗、平板中窗和桌面宽窗计算导航与内容结构。",
                _ =>
                {
                    double[] widths = [390, 800, 1440];
                    return Task.FromResult(string.Join(
                        Environment.NewLine,
                        widths.Select(width =>
                        {
                            AdaptiveLayoutDecision decision =
                                AdaptiveLayoutPolicy.Evaluate(width, 900);
                            return $"{width}: {decision.Mode}, {decision.Navigation}, columns={decision.ContentColumns}, detail={decision.ShowDetailPane}";
                        })));
                }),
            new("构造独立详情 Window", "创建拥有独立 Page 和 selected entity 的 Window workspace，不打开系统窗口。",
                _ =>
                {
                    Window window = workspaces.CreateDetailWindow("article-15");
                    WindowWorkspace workspace =
                        workspaces.Snapshot.Single(item => item.SelectedEntityId == "article-15");
                    return Task.FromResult(
                        $"window={window.Title}, workspace={workspace.Id}, entity={workspace.SelectedEntityId}");
                }),
            new("构造桌面菜单", "创建 File MenuBarItem 与两个共享命令入口。",
                _ =>
                {
                    MenuBarItem menu = DesktopMenuFactory.Create();
                    return Task.FromResult($"menu={menu.Text}, items={menu.Count}");
                }),
            new("构造多模态目标", "同一目标同时具备触摸 Tap、桌面 Pointer 和 ToolTip。",
                _ =>
                {
                    View target = DesktopMenuFactory.CreatePointerTarget();
                    return Task.FromResult(
                        $"gestures={target.GestureRecognizers.Count}, tooltip={ToolTipProperties.GetText(target)}");
                }),
            new("查看平台能力", "由平台服务描述当前构建可用的原生窗口能力。",
                _ => Task.FromResult(
                    $"platform={desktopPlatform.Platform}\n{desktopPlatform.DescribeWindowCapabilities()}")),
        ];

        return new LearningStage(
            15,
            "Windows、Mac Catalyst、多窗口与自适应",
            "第6部分-阶段15-Windows-MacCatalyst与自适应.md",
            topics,
            labs);
    }
}
