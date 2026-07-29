using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage07_NavigationShell;

internal static class StageCatalog
{
    public static LearningStage Create(
        INavigationService navigation,
        NavigationGate gate,
        NavigationJournal journal)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "Shell 信息架构",
                "Shell 以 FlyoutItem/TabBar、Tab、ShellContent 组织应用顶层信息架构，并统一路由、导航栏、搜索与 URI 导航。",
                "AppShell builds TabBar → ShellContent"),
            new(LearningLevel.Beginner, "DI 装配根 Shell",
                "App 通过构造函数接收 AppShell，CreateWindow 挂载同一实例。禁止在 App 中 new Shell 绕过页面/服务生命周期。",
                "App(AppShell shell) → new Window(_shell)"),
            new(LearningLevel.Beginner, "Flyout 与 Tabs",
                "Flyout 适合较多顶层目的地，底部 Tabs 适合少量高频区域。组合时只保留一套清晰层级，避免重复入口和双重返回栈。",
                "AppShell.FlyoutBehavior + TabBar"),
            new(LearningLevel.Beginner, "路由注册",
                "不在可视 Shell 层级中的详情页先注册类型，再用稳定常量导航。未知/重复路由应在启动或测试中提前暴露。",
                "Routing.RegisterRoute(RouteCatalog.ArticleDetail, typeof(ArticleDetailPage))"),
            new(LearningLevel.Beginner, "相对与绝对导航",
                "相对路由把页面压入当前栈，.. 返回；// 绝对路由重置到 Shell 层级。不要把 NavigationPage 与 Shell 导航随意混用。",
                "GoToAsync(ArticleDetail) and GoToAsync(\"..\")"),
            new(LearningLevel.Intermediate, "IQueryAttributable",
                "页面在 ApplyQueryAttributes 中显式读取并校验参数，可处理重复投递和对象字典；它比把字符串偷偷塞进构造函数更符合 Shell 管线。",
                "ArticleDetailPage : IQueryAttributable"),
            new(LearningLevel.Intermediate, "参数编码与白名单",
                "字符串 query 需要 URI 编码；对象字典仅在进程内传递且会保留引用。任何外部深链参数都按不可信输入校验长度和字符。",
                "ArticleRoute.TryCreate"),
            new(LearningLevel.Intermediate, "返回栈",
                "Shell 维护导航栈，Modal 使用独立呈现语义。页面消失不等于销毁；快速前后导航需取消旧加载并让 ApplyQueryAttributes 幂等。",
                "PresentationMode + relative back"),
            new(LearningLevel.Intermediate, "Modal",
                "登录、编辑等阻断流程可设置 ModalAnimated；Modal 不是新的业务状态源，关闭路径与 Android 系统返回键都必须定义。",
                "LoginPage PresentationMode.ModalAnimated"),
            new(LearningLevel.Intermediate, "返回结果",
                "需要页面结果时用有所有权的 TaskCompletionSource/结果 broker，并在返回、取消、页面销毁时只完成一次，避免静态事件。",
                "NavigationResultBroker<T>"),
            new(LearningLevel.Intermediate, "鉴权门闸",
                "受保护路由先检查 Session；未登录跳登录并保留安全的目标意图，成功后恢复。门闸不能只藏在按钮可见性里。",
                "NavigationGate protected route allow-list"),
            new(LearningLevel.Advanced, "导航横切管道",
                "Shell.Navigating 可做审计、取消与统一门闸，Navigated 记录结果。回调应短小，不能在多个页面重复实现权限/遥测。",
                "AppShell.Navigating / Navigated → NavigationJournal"),
            new(LearningLevel.Advanced, "INavigationService",
                "VM 依赖类型化导航接口而非 Shell.Current；Shell 实现集中构造参数字典、门闸和日志，测试用 fake 断言意图。",
                "INavigationService + ShellNavigationService"),
            new(LearningLevel.Advanced, "类型化路由",
                "RouteCatalog 消除散落字符串，ArticleRoute 封装领域参数验证。复杂应用可用生成器，但仍要理解底层 Shell route。",
                "RouteCatalog + ArticleRoute"),
            new(LearningLevel.Advanced, "SearchHandler",
                "SearchHandler 适合全局可发现搜索，结果模板优先强类型 ItemTemplate；软键盘与结果执行要实测。简单场景也可 Toolbar+搜索页。",
                "documented extension point; no search model in this project"),
            new(LearningLevel.Advanced, "Toolbar 与导航栏",
                "ToolbarItems、TitleView 与 NavBar 配合 Shell；MAUI 10 的 Secondary 工具栏和导航栏可见动画需要按 Android/iOS/桌面实测。",
                "Shell owns navigation chrome"),
            new(LearningLevel.Advanced, "深链统一入口",
                "Custom URI/App Link/通知点击先解析成类型化 RouteIntent，再经过鉴权门闸与同一 INavigationService；冷热启动不分叉业务规则。",
                "ArticleRoute is the shared parsed intent"),
            new(LearningLevel.Expert, "页面创建与 DI",
                "可视 ShellContent 可以接收 DI 已创建页面；注册路由的页面由路由工厂创建。带复杂依赖时应验证当前 MAUI 版本的 DI 路由激活。",
                "AppShell receives MainPage; detail route has safe constructor"),
            new(LearningLevel.Expert, "Shell 选型边界",
                "Shell 适合标准信息架构和 URI 路由；游戏、画布、强自定义转场或嵌入式区域可选自定义 Router。不要为简单应用堆多套导航框架。",
                "one Shell root, no nested NavigationPage"),
            new(LearningLevel.Expert, "可测试导航",
                "纯测试覆盖参数校验、受保护路由重定向、幂等与返回结果；设备测试覆盖系统返回、深链冷热启动、Modal 和快速连点。",
                "ArticleRoute/NavigationGate/NavigationResultBroker are UI-independent"),
        ];

        LearningLab[] labs =
        [
            new("校验类型化路由", "对合法、空值、超长和路径穿越 ID 执行纯函数验证。",
                unusedToken =>
                {
                    bool valid = ArticleRoute.TryCreate("article-42", out ArticleRoute route);
                    bool empty = ArticleRoute.TryCreate("", out _);
                    bool traversal = ArticleRoute.TryCreate("../secret", out _);
                    return Task.FromResult(
                        $"valid={valid}, id={route.Id}\nempty accepted={empty}\ntraversal accepted={traversal}");
                }),
            new("验证鉴权门闸", "检查未登录访问设置页会重定向，而公开详情页可通过。",
                unusedToken =>
                {
                    bool settings = gate.CanNavigate(RouteCatalog.Settings, out string? redirect);
                    bool article = gate.CanNavigate(RouteCatalog.ArticleDetail, out _);
                    return Task.FromResult(
                        $"settings allowed={settings}, redirect={redirect}\narticle allowed={article}");
                }),
            new("导航到详情", "通过类型化导航服务压入详情页并传递已校验的参数。",
                async token =>
                {
                    await navigation.NavigateToArticleAsync("maui-10", token);
                    return "详情路由已交给 Shell；使用系统返回键回到学习页。";
                }),
            new("验证返回结果", "用 TaskCompletionSource broker 完成一次页面结果。",
                async token =>
                {
                    NavigationResultBroker<string> broker = new();
                    Task<string> pending = broker.WaitAsync(token);
                    bool accepted = broker.TrySetResult("saved");
                    string result = await pending;
                    return $"accepted={accepted}, result={result}";
                }),
            new("查看导航日志", "读取 Shell Navigating/Navigated 与服务操作的最新时间线。",
                _ => Task.FromResult(journal.Snapshot.Count == 0
                    ? "尚无导航记录。先运行“导航到详情”。"
                    : string.Join(Environment.NewLine, journal.Snapshot.TakeLast(10)))),
        ];

        return new LearningStage(
            7,
            "导航、Shell 与类型化路由",
            "第3部分-阶段7-导航与Shell.md",
            topics,
            labs);
    }
}
