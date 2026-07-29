using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage27_WindowShellDispatcher;

internal static class StageCatalog
{
    public static LearningStage Create(
        WindowJournal windowJournal,
        NavigationJournal navigationJournal,
        DeepLinkCoordinator deepLinks,
        IUiDispatcher dispatcher,
        SessionGate sessionGate,
        PageLifetime pageLifetime)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "Application、Window、Page",
                "Application 管进程级服务和窗口创建；Window 是生命周期/多窗舞台；Page 是可导航内容，消失不等于销毁。",
                "App.CreateWindow + MainPage lifetime"),
            new(LearningLevel.Beginner, "冷启动链",
                "平台 activation→MauiApp→App→CreateWindow→Shell root→Handler/platform window；启动早期 Shell.Current 可能尚未可用。",
                "DeepLinkCoordinator ready gate"),
            new(LearningLevel.Beginner, "Window 事件",
                "Created/Activated/Deactivated/Stopped/Resumed/Destroying 描述窗口状态；保存轻量状态并暂停工作，不把事件当跨平台完全同序。",
                "WindowJournal"),
            new(LearningLevel.Beginner, "Page 生命周期",
                "Appearing 可多次发生，Disappearing 取消本次页面工作但不 Dispose 缓存页；快速返回时旧任务不得写新 UI。",
                "PageLifetime"),
            new(LearningLevel.Intermediate, "Shell 信息架构",
                "TabBar/Flyout/ShellContent 定义顶层结构，注册路由承载详情/模态；路由名称常量化，避免散落字符串。",
                "AppShell + RouteCatalog"),
            new(LearningLevel.Intermediate, "路由注册",
                "Routing.RegisterRoute 必须在导航前且在组合根集中执行；重复/未注册路由在启动或首次导航失败。",
                "MauiProgram"),
            new(LearningLevel.Intermediate, "IQueryAttributable",
                "页面构造期参数尚未应用；ApplyQueryAttributes 校验白名单 DTO/id 并进入 Loading/Ready/Error 状态。",
                "DetailPage"),
            new(LearningLevel.Intermediate, "Shell 导航管道",
                "GoToAsync 解析 URI、创建页面、应用 query、更新栈和动画；Navigating/Navigated 用于日志和门闸而非业务服务定位。",
                "NavigationJournal"),
            new(LearningLevel.Intermediate, "Dispatcher",
                "后台 HTTP/DB/传感器回调不能直接改 UI 或绑定集合；通过元素 IDispatcher 或 MainThread 编组并 await。",
                "IUiDispatcher"),
            new(LearningLevel.Intermediate, "DispatchRequired",
                "已在 UI 线程可直接执行，否则 Dispatch/InvokeOnMainThreadAsync；封装接口让单测用同步 fake。",
                "MauiUiDispatcher"),
            new(LearningLevel.Advanced, "深链总管道",
                "Intent/URL→平台回调→OnAppLinkRequestReceived→白名单解析→Shell ready→auth gate→统一 GoToAsync。",
                "App + DeepLinkCoordinator"),
            new(LearningLevel.Advanced, "Android 入口",
                "BROWSABLE VIEW intent filter + custom scheme，SingleTop 支持热启动；生产 App Links 还需 HTTPS/assetlinks 验证。",
                "MainActivity IntentFilter"),
            new(LearningLevel.Advanced, "冷/热/温启动",
                "冷启动先排队到 Window.Created；热启动幂等去重；Activity 重建恢复导航状态，不重复压栈。",
                "ready TaskCompletionSource + recent routes"),
            new(LearningLevel.Advanced, "安全白名单",
                "拒绝未知 scheme/host/path，id 限长限字符；token 用一次性交换且不写日志，javascript: 永不进入路由。",
                "DeepLinkRoute"),
            new(LearningLevel.Advanced, "认证门闸",
                "深链、通知点击和 App 内导航走同一 coordinator；未登录先去 Login，成功后恢复经过验证的原目标。",
                "SessionGate"),
            new(LearningLevel.Expert, "导航串行化",
                "并发 push/深链会竞争 Shell 栈；SemaphoreSlim 串行关键区，取消在等待期传播，finally 必须释放。",
                "DeepLinkCoordinator navigation lock"),
            new(LearningLevel.Expert, "多窗口",
                "Singleton 不能抓当前 Page/Shell/Dispatcher；每个 Window 应拥有导航与状态上下文，跨窗共享只留线程安全业务状态。",
                "current single-window attachment is explicit"),
            new(LearningLevel.Expert, "Dispatcher 实现",
                "Android 主 Looper、Apple main queue、Windows DispatcherQueue 最终承载 IDispatcher/MainThread；ConfigureAwait 后需重新编组。",
                "MauiUiDispatcher"),
            new(LearningLevel.Expert, "故障定位",
                "按 Window readiness、route registry、query、gate、thread、Shell stack 分层记录；不要用延迟常数掩盖 readiness 竞态。",
                "journals + readiness task"),
            new(LearningLevel.Expert, "验收",
                "记录 Window 时序、页面缓存次数、合法/非法 query、后台 UI 编组、冷热深链、auth gate 与快速返回取消。",
                "runnable labs"),
        ];

        LearningLab[] labs =
        [
            new("查看 Window 时序", "输出当前窗口已发生的生命周期事件。",
                unusedToken =>
                {
                    IReadOnlyList<string> entries = windowJournal.Snapshot();
                    return Task.FromResult(entries.Count == 0
                        ? "Window events will appear after Created/Activated."
                        : string.Join('\n', entries));
                }),
            new("验证 Dispatcher", "从线程池返回后经可测试 dispatcher 回到 UI 上下文。",
                async token =>
                {
                    int workerThread = await Task.Run(
                        () => Environment.CurrentManagedThreadId,
                        token);
                    int uiThread = -1;
                    await dispatcher.InvokeAsync(() =>
                    {
                        uiThread = Environment.CurrentManagedThreadId;
                        return Task.CompletedTask;
                    });
                    return $"dispatch required now={dispatcher.IsDispatchRequired}; worker={workerThread}; ui={uiThread}";
                }),
            new("拒绝恶意深链", "未知 scheme/host/path 在到达 Shell 前被拒绝。",
                async token =>
                {
                    NavigationOutcome outcome = await deepLinks.NavigateAsync(
                        new Uri("javascript://detail/secret"),
                        token);
                    return $"accepted={outcome.Accepted}; reason={outcome.Reason}";
                }),
            new("运行认证门闸", "合法深链未登录时定向 Login；再次实验可登录后走详情。",
                async token =>
                {
                    sessionGate.SignOut();
                    string id = $"article-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
                    NavigationOutcome outcome = await deepLinks.NavigateAsync(
                        new Uri($"learnmaui://detail/{id}"),
                        token);
                    return $"accepted={outcome.Accepted}; destination={outcome.Destination}; reason={outcome.Reason}";
                }),
            new("查看导航日志", "输出 Navigating/Navigated 与深链协调记录。",
                unusedToken =>
                {
                    IReadOnlyList<string> entries = navigationJournal.Snapshot();
                    return Task.FromResult(entries.Count == 0
                        ? "No navigation recorded yet."
                        : string.Join('\n', entries));
                }),
            new("验证页面取消", "证明 Appearing 可重入，Disappearing 取消本次工作而非销毁页面。",
                unusedToken => Task.FromResult(
                    $"appearing={pageLifetime.AppearingCount}; cancellations={pageLifetime.CancellationCount}; " +
                    $"current canceled={pageLifetime.CurrentToken.IsCancellationRequested}")),
        ];

        return new LearningStage(
            27,
            "Window、Shell 路由与 Dispatcher",
            "第9部分-04-Window-Shell-Dispatcher.md",
            topics,
            labs);
    }
}
