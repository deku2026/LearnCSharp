using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage13_BackgroundPush;

internal static class StageCatalog
{
    public static LearningStage Create(
        IBackgroundScheduler scheduler,
        BackgroundSyncCoordinator coordinator,
        INotificationPlatformBridge notifications,
        PushRegistrationStore pushRegistration)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "共享层建模意图",
                "共享代码描述同步原因、最早时间、网络/充电约束和唯一名称，不直接引用 WorkManager/BGTask/Windows API。",
                "BackgroundWorkIntent"),
            new(LearningLevel.Beginner, "调度端口",
                "IBackgroundScheduler 是产品意图到平台调度器的端口；Android、iOS、Windows 实现不同，调用方不假设执行时刻。",
                "IBackgroundScheduler"),
            new(LearningLevel.Beginner, "后台不等于常驻",
                "移动系统可延迟、合并、限额或取消工作；普通 Task 在进程死亡后不可靠。产品承诺应是最终同步而非精确秒级。",
                "EarliestStart expresses a lower bound, not a guarantee"),
            new(LearningLevel.Beginner, "本地与远程通知",
                "本地通知由设备安排，远程推送由 APNs/FCM 传输；可见通知、静默推送和后台同步是三条不同通道。",
                "NotificationEnvelope is independent of scheduling"),
            new(LearningLevel.Beginner, "通知权限",
                "Android 13+ POST_NOTIFICATIONS 与 Apple authorization 按需请求，并在拒绝时保留应用内消息中心；不要首启强迫授权。",
                "AndroidManifest POST_NOTIFICATIONS"),
            new(LearningLevel.Intermediate, "Android WorkManager 映射",
                "唯一工作名、ExistingWorkPolicy、网络/充电约束和退避映射 BackgroundWorkIntent；Worker 只解析 DI 并调用共享协调器。",
                "UniqueName + constraints in BackgroundWorkIntent"),
            new(LearningLevel.Intermediate, "前台服务边界",
                "用户可感知的长上传/导航等才使用前台服务并持续通知；普通刷新不滥用，Android 版本限制和服务类型需清单声明。",
                "this sample does not claim a foreground service"),
            new(LearningLevel.Intermediate, "通知渠道",
                "Android 8+ 先创建稳定 channel id；用户控制渠道级声音/重要性，应用升级不能悄悄改变已创建渠道行为。",
                "EnsureDefaultChannelAsync"),
            new(LearningLevel.Intermediate, "FCM/APNs token 轮换",
                "推送 token 不是用户 ID，会轮换；更新后幂等上传并与账户/设备注册关联，登出撤销后清本地。",
                "PushRegistrationStore.Update/Clear"),
            new(LearningLevel.Intermediate, "iOS BackgroundTasks",
                "BGAppRefreshTask/BGProcessingTask 需 identifier、entitlement 和最早时间，系统决定执行；expiration handler 必须取消共享工作并完成任务。",
                "shared coordinator accepts CancellationToken"),
            new(LearningLevel.Intermediate, "Silent push 限额",
                "content-available 不是可靠队列，会受用户设置、能耗和系统策略限制；服务端保留真相，客户端下一次机会补拉。",
                "sync is idempotent and retryable"),
            new(LearningLevel.Advanced, "单飞与幂等同步",
                "前台恢复、后台任务、推送可能同时触发；协调器 single-flight，操作以稳定 ID/版本幂等，重复执行不破坏数据。",
                "BackgroundSyncCoordinator SemaphoreSlim"),
            new(LearningLevel.Advanced, "重试语义",
                "只有瞬时失败返回 retry，永久鉴权/校验失败不重试；退避由平台调度器控制，内部不自建无限循环。",
                "SyncRunResult.ShouldRetry"),
            new(LearningLevel.Advanced, "通知点击路由",
                "payload 只包含稳定 route/entity id，不信任任意 URL；点击后进入与普通深链相同的解析、鉴权和导航管道。",
                "NotificationRouter allow-list"),
            new(LearningLevel.Advanced, "冷/热启动",
                "冷启动先完成组合根和建窗再消费 pending intent；热启动直接投递到现有窗口。消费需幂等，避免同一点击导航两次。",
                "NotificationEnvelope.Id is a deduplication key"),
            new(LearningLevel.Advanced, "隐私文案",
                "通知说明用户价值和类别，不泄漏锁屏敏感正文；营销与交易通知分渠道/授权，提供设置和退订。",
                "Envelope separates title/body from route metadata"),
            new(LearningLevel.Advanced, "电量与网络约束",
                "批量、增量、压缩和服务端 cursor 减少唤醒；大同步选择 unmetered/charging，紧急业务也要定义数据预算。",
                "NetworkConstraint + RequiresCharging"),
            new(LearningLevel.Expert, "精确闹钟与 Boot",
                "精确闹钟需要特殊权限和强产品理由；设备重启后调度是否保留取决于 API，BootReceiver 只重建必要计划。",
                "no exact alarm permission in this sample"),
            new(LearningLevel.Expert, "社区库版本矩阵",
                "推送/通知库必须记录 MAUI/.NET/Android/iOS 版本、维护状态、权限、trim/AOT 和原生依赖；仓库未安装 FCM 库所以不伪造实现。",
                "platform bridge stays provider-neutral"),
            new(LearningLevel.Expert, "测试与可靠性定义",
                "纯测 intent/router/single-flight，平台测约束/channel/token，真机测强停/重启/离线/省电/权限；可靠=服务端真相+客户端最终补偿。",
                "InMemoryBackgroundScheduler enables deterministic tests"),
        ];

        LearningLab[] labs =
        [
            new("安排唯一后台意图", "以唯一名和约束保存调度意图；重复同名会更新而非堆积。",
                async token =>
                {
                    BackgroundWorkIntent intent = new(
                        "notes-sync",
                        "user-edit",
                        DateTimeOffset.UtcNow.AddMinutes(5),
                        NetworkConstraint.Connected,
                        false);
                    await scheduler.ScheduleAsync(intent, token);
                    await scheduler.ScheduleAsync(intent with { Reason = "push-hint" }, token);
                    return $"scheduled={scheduler.Snapshot.Count}, reason={scheduler.Snapshot.Single().Reason}";
                }),
            new("执行共享同步", "在网络可用时运行一次可取消、single-flight 的共享协调器。",
                async token =>
                {
                    SyncRunResult result =
                        await coordinator.RunAsync("manual-lab", true, token);
                    return $"didRun={result.DidRun}, retry={result.ShouldRetry}, runs={coordinator.RunCount}";
                }),
            new("验证离线重试", "网络不可用时不做忙循环，只返回可由平台退避调度的 retry 语义。",
                async token =>
                {
                    SyncRunResult result =
                        await coordinator.RunAsync("offline-lab", false, token);
                    return $"didRun={result.DidRun}, retry={result.ShouldRetry}, message={result.Message}";
                }),
            new("创建通知渠道", "在 Android 8+ 幂等创建默认渠道；旧版本安全跳过。",
                async token =>
                {
                    string channel = await notifications.EnsureDefaultChannelAsync(token);
                    return $"platform={notifications.Platform}, channel={channel}";
                }),
            new("校验通知点击", "把受信任 payload 转成类型化路由，并拒绝路径穿越。",
                unusedToken =>
                {
                    bool valid = NotificationRouter.TryParse(
                        new NotificationEnvelope("n-1", "Article", "Ready", "article", "a-42"),
                        out NotificationRoute route);
                    bool invalid = NotificationRouter.TryParse(
                        new NotificationEnvelope("n-2", "Bad", "Bad", "article", "../secret"),
                        out _);
                    return Task.FromResult($"valid={valid}, route={route}\ninvalid accepted={invalid}");
                }),
            new("模拟 token 轮换", "幂等更新同一 token，再接受新 token 并增加 revision。",
                _ =>
                {
                    bool first = pushRegistration.Update("token-a");
                    bool duplicate = pushRegistration.Update("token-a");
                    bool rotated = pushRegistration.Update("token-b");
                    return Task.FromResult(
                        $"first={first}, duplicate={duplicate}, rotated={rotated}, revision={pushRegistration.Revision}");
                }),
        ];

        return new LearningStage(
            13,
            "后台任务、推送与通知",
            "第5部分-阶段13-后台推送通知.md",
            topics,
            labs);
    }
}
