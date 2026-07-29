using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage03_UIThreadAsync;

internal static class StageCatalog
{
    public static LearningStage Create(AsyncWorkCoordinator coordinator)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "主线程心智模型",
                "Android Looper 等平台消息循环负责 UI；MAUI 控件只能在所属 UI 线程读写。await 默认恢复捕获的 SynchronizationContext，但库代码不能盲目依赖它。",
                "MainThread.IsMainThread"),
            new(LearningLevel.Beginner, "MainThread 与 Dispatcher",
                "MainThread 适合从平台/服务边界切回应用主线程；元素的 Dispatcher 适合调度到该视觉树所属线程，多窗口时优先使用元素 Dispatcher。",
                "MainThread.InvokeOnMainThreadAsync"),
            new(LearningLevel.Beginner, "async Task 默认",
                "所有可等待业务方法返回 Task/Task<T>，让调用者等待、取消并观察异常。async void 只用于框架规定的 UI 事件入口。",
                "AsyncWorkCoordinator.SearchAsync"),
            new(LearningLevel.Beginner, "禁止同步阻塞",
                ".Result、.Wait() 和 GetAwaiter().GetResult() 会冻结 UI，并可能因上下文互相等待造成死锁。异步链应一路 await 到事件边界。",
                "all labs await Tasks end-to-end"),
            new(LearningLevel.Beginner, "取消所有权",
                "页面或命令拥有 CTS，向服务传递 Token，在离页、新请求、用户取消或超时时触发，并把 OperationCanceledException 当作预期控制流。",
                "MainPage.OnDisappearing → AsyncWorkCoordinator.Cancel"),
            new(LearningLevel.Intermediate, "Task.Run 的正确边界",
                "CPU 密集循环可送到线程池；HTTP、数据库等天然异步 I/O 直接 await，包进 Task.Run 只浪费线程且破坏取消/异常语义。",
                "CountOnWorkerAsync wraps only CPU work"),
            new(LearningLevel.Intermediate, "异常传播",
                "Task 中异常沿 await 传播并在可负责的边界处理。fire-and-forget 必须由明确观察器捕获；业务层不得用 async void 吞掉崩溃。",
                "LearningStageView.RunLabAsync catches and displays exceptions"),
            new(LearningLevel.Intermediate, "防重入与 single-flight",
                "按钮连点可能启动多次请求。命令 CanExecute、SemaphoreSlim 或取消前次请求可实现禁重入/最后一次为准策略。",
                "SemaphoreSlim _singleFlight + AsyncCommand.IsRunning"),
            new(LearningLevel.Intermediate, "超时与链接 Token",
                "用户取消、页面取消和超时应组合成链接 Token；服务只接收一个 Token，并在所有 await 与循环中协作检查。",
                "CreateLinkedTokenSource + CancelAfter"),
            new(LearningLevel.Intermediate, "动画是异步操作",
                "FadeTo/ScaleTo 等动画返回 Task，应 await 并在离页时取消；并行动画用 Task.WhenAll，避免 Dispatcher.StartTimer 式失控回调。",
                "await Task.WhenAll(view.FadeTo(...), view.ScaleTo(...))"),
            new(LearningLevel.Intermediate, "可取消定时器",
                "PeriodicTimer 用 await 和 Token 表达所有权，页面消失即可终止循环。UI DispatcherTimer 只适合轻量 UI 节拍。",
                "CountTimerTicksAsync uses PeriodicTimer"),
            new(LearningLevel.Advanced, "异步命令",
                "ICommand.Execute 必须是 void，所以 async void 仅停留在这一 UI 边界；命令内部捕获异常、暴露运行状态、禁重入并提供 Cancel。",
                "AsyncCommand.Execute"),
            new(LearningLevel.Advanced, "绑定更新线程",
                "后台回调或异步流产生的集合变更应批量切回 Dispatcher；不要逐项高频跨线程通知，也不要在后台线程改 ObservableCollection。",
                "MainThread.InvokeOnMainThreadAsync around UI-bound state"),
            new(LearningLevel.Advanced, "生命周期联动",
                "Appearing 创建页面 Token，Disappearing 取消。新搜索取消旧搜索；窗口停止可取消应用级前台任务。每个 CTS 都有单一拥有者。",
                "MainPage owns coordinator lifetime"),
            new(LearningLevel.Advanced, "并发而非串行",
                "互不依赖的异步 I/O 先启动再 WhenAll；有依赖或限流时保持顺序。WhenAll 的异常也必须被 await 和观察。",
                "Task.WhenAll in concurrent lab"),
            new(LearningLevel.Expert, "SynchronizationContext 边界",
                "UI 入口 await 通常回到 UI 上下文；ConfigureAwait(false) 适合不触 UI 的可复用库内部。回到 View/VM 更新点时显式使用 Dispatcher。",
                "service code has no UI reference; UI marshaling is explicit"),
            new(LearningLevel.Expert, "多窗口 Dispatcher",
                "MainThread 只有平台主线程概念，而元素 Dispatcher 还表达窗口/视觉树归属。高级多窗代码不能缓存某个已销毁页面的 Dispatcher。",
                "BindableObject.Dispatcher belongs to an element"),
            new(LearningLevel.Expert, "审查反模式",
                "扫描 async void（仅事件/命令边界允许）、Result/Wait、无 Token 的长任务、未观察 Task、后台线程改 UI、无限定时器与连点重入。",
                "rg \"async void|\\.Result|\\.Wait\\(\""),
        ];

        LearningLab[] labs =
        [
            new("验证 UI 调度", "从线程池发起并切回主线程，输出两个执行位置。",
                async token =>
                {
                    bool workerWasMain = await Task.Run(() => MainThread.IsMainThread, token);
                    bool dispatchedWasMain = await MainThread.InvokeOnMainThreadAsync(
                        () => MainThread.IsMainThread);
                    return $"线程池 IsMainThread={workerWasMain}\n调度后 IsMainThread={dispatchedWasMain}";
                }),
            new("搜索与防重入", "运行可取消的异步搜索，并经 SemaphoreSlim 保证单次执行。",
                async token => await coordinator.SearchAsync("maui async", token)),
            new("CPU 工作与取消", "把纯 CPU 累加放到 Task.Run，并在循环内观察 Token。",
                async token =>
                {
                    long result = await AsyncWorkCoordinator.CountOnWorkerAsync(100_000, token);
                    return $"1..100000 的和：{result}";
                }),
            new("超时演示", "用 CancelAfter 为慢任务建立明确超时边界。",
                async token =>
                {
                    using CancellationTokenSource timeout = new(TimeSpan.FromMilliseconds(120));
                    using CancellationTokenSource linked =
                        CancellationTokenSource.CreateLinkedTokenSource(token, timeout.Token);
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2), linked.Token);
                        return "意外：慢任务未超时。";
                    }
                    catch (OperationCanceledException) when (timeout.IsCancellationRequested)
                    {
                        return "成功：120ms 超时已协作取消慢任务。";
                    }
                }),
            new("PeriodicTimer", "运行三个可取消节拍，证明定时循环有清晰所有权。",
                async token =>
                {
                    int ticks = await AsyncWorkCoordinator.CountTimerTicksAsync(
                        3,
                        TimeSpan.FromMilliseconds(80),
                        token);
                    return $"已观察 {ticks} 个节拍，Timer 已释放。";
                }),
        ];

        return new LearningStage(
            3,
            "UI 线程与异步纪律",
            "第2部分-阶段3-UI线程与异步纪律.md",
            topics,
            labs);
    }
}
