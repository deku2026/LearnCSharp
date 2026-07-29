using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage02_LifecycleWindowState;

internal static class StageCatalog
{
    public static LearningStage Create(
        LifecycleJournal journal,
        PageWorkController pageWork,
        StateStore stateStore)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "Window 生命周期",
                "Created、Activated、Deactivated、Stopped、Resumed、Destroying 描述窗口可见性与交互状态；它们不是严格成对，也不能保证进程被杀前一定收到 Destroying。",
                "LifecycleJournal.Attach(Window)"),
            new(LearningLevel.Beginner, "Page 可见性",
                "OnAppearing 开始仅在页面可见时有意义的加载；OnDisappearing 取消加载和预览。页面消失不代表对象销毁，导航栈可能仍保留实例。",
                "MainPage.OnAppearing / OnDisappearing"),
            new(LearningLevel.Beginner, "四种结束",
                "导航离开、窗口失活/后台、配置变化和进程死亡是四种不同事件。只有持久化状态能够跨进程死亡，内存字段与 Singleton 都不能。",
                "StateStore uses Preferences rather than static memory"),
            new(LearningLevel.Beginner, "状态分层",
                "瞬时控件状态留在 View；页面状态留在 VM；小型非敏感恢复线索放 Preferences；令牌放 SecureStorage；大草稿放文件/数据库；服务端数据可重拉。",
                "StateStore.SaveDraft / LoadDraft"),
            new(LearningLevel.Beginner, "协作取消",
                "CancellationTokenSource 的拥有者负责取消和释放，并把同一个 Token 向下传递。取消是生命周期结果，不应当作业务错误弹窗。",
                "PageWorkController.RestartAsync and Cancel"),
            new(LearningLevel.Intermediate, "Window 事件与业务状态机分离",
                "生命周期只决定启动、暂停、恢复和保存时机；Loading/Content/Error 属于业务状态机。不要用多个生命周期 bool 拼业务流程。",
                "LifecycleJournal only records and forwards events"),
            new(LearningLevel.Intermediate, "ConfigureLifecycleEvents",
                "平台原生生命周期钩子在组合根注册，只做打点或转发。平台回调不直接操作页面字段，也不承载完整业务用例。",
                "MauiProgram.ConfigureLifecycleEvents → AddAndroid"),
            new(LearningLevel.Intermediate, "跨平台与平台事件分工",
                "Window 事件用于共享逻辑；ConfigureLifecycleEvents 用于必须读取原生参数/时序的桥接；Page 回调用于页面级可见性。",
                "LifecycleJournal tags [platform] and [shared]"),
            new(LearningLevel.Intermediate, "配置变化",
                "旋转、分屏、主题或字体缩放可能触发布局和原生配置变化。必须保留的输入先进入 VM，产品要求跨进程时再落盘。",
                "MainActivity.ConfigurationChanges + StateStore"),
            new(LearningLevel.Intermediate, "Preferences 与 SecureStorage",
                "Preferences 只保存非敏感小键值；SecureStorage 保存令牌等敏感小数据且必须处理平台失败。大 JSON 不应塞进 Preferences。",
                "Preferences.Default.Set/Get in StateStore"),
            new(LearningLevel.Intermediate, "冷启动与热启动深链",
                "冷启动先走完整组合根与建窗，再应用路由；热启动把新 URI 投递给同一解析管道，并取消旧页面工作。",
                "DeepLinkParser produces the same article id for both entry modes"),
            new(LearningLevel.Advanced, "深链输入不可信",
                "自定义 URI、HTTPS App Link、通知和分享参数都要校验 scheme、host、path、长度和字符白名单，绝不能直接拼 SQL 或文件路径。",
                "DeepLinkParser.TryParseArticle"),
            new(LearningLevel.Advanced, "链接 Token 与超时",
                "页面离开 Token、用户取消 Token 与超时 Token 可链接；任一来源触发就协作取消下游 I/O。",
                "CancellationTokenSource.CreateLinkedTokenSource"),
            new(LearningLevel.Advanced, "进程死亡恢复",
                "应用可能没有任何最终回调就被系统终止。关键草稿应持续/防抖保存，重启时从本地真相源恢复；进行中的 HTTP 直接重试。",
                "StateStore persists article draft eagerly"),
            new(LearningLevel.Advanced, "多窗口状态",
                "每个 Window 应拥有独立导航树、页面和窗口状态；生命周期日志必须标识窗口。不要把当前 Page 或窗口 CTS 放进全局静态字段。",
                "App.CreateWindow owns its Window and attaches events"),
            new(LearningLevel.Advanced, "后台能力边界",
                "进入后台只适合快速保存线索与停止昂贵资源。需要可靠后台执行时必须使用 Android Job/Work、Apple BGTask 等平台调度，而不是假设普通 Task 会继续。",
                "Window.Stopped records and returns immediately"),
            new(LearningLevel.Expert, "平台语义映射",
                "Android Activity onPause/onStop、Apple scene/app callbacks 与 Windows Activated/Visibility 只能近似映射共享 Window 事件；业务代码依赖语义而非死记 1:1 次数。",
                "ConfigureLifecycleEvents plus Window events"),
            new(LearningLevel.Expert, "事件订阅所有权",
                "订阅者必须有明确的解除时机；长寿命发布者引用短寿命页面会泄漏。Window 自己拥有的事件随窗口销毁，跨窗口 Hub 应使用可释放订阅。",
                "LifecycleJournal.Attach is scoped to the created Window"),
            new(LearningLevel.Expert, "可取消详情页",
                "综合流：Appearing 恢复笔记并创建 CTS 拉正文；Disappearing/Stopped 取消请求并保存笔记；冷启动从路由 id 与本地笔记重建。",
                "MainPage + PageWorkController + StateStore + DeepLinkParser"),
            new(LearningLevel.Expert, "验收时间线",
                "启动、导航、Home、恢复、旋转和强停后应能导出带时间戳的 Window/Page/平台事件，解释哪些状态仍在以及为什么。",
                "LifecycleJournal.Snapshot"),
        ];

        LearningLab[] labs =
        [
            new("查看生命周期时间线", "读取 Window、Page 与 Android 原生回调产生的统一时间线。",
                _ => Task.FromResult(journal.Snapshot.Count == 0
                    ? "尚未捕获事件。切到后台再返回后重试。"
                    : string.Join(Environment.NewLine, journal.Snapshot.TakeLast(12)))),
            new("验证取消", "启动短任务后主动取消，确认 OperationCanceledException 是预期控制流。",
                async token =>
                {
                    using CancellationTokenSource local = CancellationTokenSource.CreateLinkedTokenSource(token);
                    Task work = pageWork.RestartAsync(TimeSpan.FromSeconds(5));
                    await Task.Delay(120, token);
                    pageWork.Cancel();
                    try
                    {
                        await work;
                        return "意外：任务没有取消。";
                    }
                    catch (OperationCanceledException)
                    {
                        return "成功：页面工作收到取消信号，没有继续更新 UI。";
                    }
                }),
            new("保存并恢复草稿", "把非敏感小型恢复线索写入 Preferences，再立刻读取验证。",
                _ =>
                {
                    string draft = $"draft-{DateTimeOffset.Now:HHmmss}";
                    stateStore.SaveDraft(draft);
                    return Task.FromResult($"写入：{draft}\n读回：{stateStore.LoadDraft()}");
                }),
            new("校验深链", "同时检查合法链接、错 host 和路径穿越输入。",
                unusedToken =>
                {
                    Uri valid = new("https://learnmaui.example/articles/a-42");
                    Uri wrongHost = new("https://evil.example/articles/a-42");
                    Uri traversal = new("learnmaui://articles/..");
                    bool accepted = DeepLinkParser.TryParseArticle(valid, out string id);
                    bool rejectedHost = !DeepLinkParser.TryParseArticle(wrongHost, out _);
                    bool rejectedTraversal = !DeepLinkParser.TryParseArticle(traversal, out _);
                    return Task.FromResult(
                        $"合法：{accepted}, id={id}\n拒绝错 host：{rejectedHost}\n拒绝路径穿越：{rejectedTraversal}");
                }),
        ];

        return new LearningStage(
            2,
            "生命周期、窗口与状态恢复",
            "第2部分-阶段2-生命周期窗口与状态.md",
            topics,
            labs);
    }
}
