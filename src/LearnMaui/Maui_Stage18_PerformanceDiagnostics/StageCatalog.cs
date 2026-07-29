using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage18_PerformanceDiagnostics;

internal static class StageCatalog
{
    public static LearningStage Create(PerformanceTelemetry telemetry)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "性能闭环",
                "定义关键用户旅程与预算→固定环境测基线→profile 找瓶颈→一次改动→同条件复测→记录收益/副作用，禁止凭感觉优化。",
                "PerformanceSample before/after evidence"),
            new(LearningLevel.Beginner, "CUJ 与指标",
                "冷启动首帧、列表首屏/滚动、详情加载、保存和恢复是 CUJ；关注时延分位、帧卡顿、分配、GC、内存、包体、电量/网络。",
                "named OperationScope per journey"),
            new(LearningLevel.Beginner, "冷启动分段",
                "原生入口、runtime、MauiApp Build、DI、App/Window、XAML/Handler、首帧分别打点；不要把迁移、网络和大图同步塞首帧。",
                "app.initialize and app.create_window operations"),
            new(LearningLevel.Beginner, "冷/暖启动",
                "冷启动控制进程、文件缓存与设备温度；暖启动含 JIT/缓存不可混为一组。多次采样看分布而非单次最好值。",
                "baseline table records launch condition"),
            new(LearningLevel.Intermediate, "Measure / Arrange",
                "布局两阶段可能多次执行；减少深层嵌套、Auto/无限约束与 Measure 副作用，先用 Meter/trace 证实热点。",
                "MAUI Meter plus custom operation metrics"),
            new(LearningLevel.Intermediate, "ActivitySource",
                "Activity 表达跨 UI→网络→DB 的时序与父子关系，tag 低基数且不含 PII；无 Listener 时开销应小。",
                "ActivitySource LearnMaui.Stage18"),
            new(LearningLevel.Intermediate, "Meter",
                "Counter 计次数、Histogram 记录延迟/大小、ObservableGauge 暴露当前值；维度不能用 user/id/url 等高基数数据。",
                "operation.count + operation.duration"),
            new(LearningLevel.Intermediate, "分配与 GC",
                "热路径装箱、字符串、LINQ、闭包和模板创建会分配；看 allocation stack 与 GC pause，再复用/池化，避免过度缓存。",
                "GC.GetTotalAllocatedBytes + CollectionCount"),
            new(LearningLevel.Intermediate, "泄漏",
                "长寿命事件/Timer/Messenger/Handler 引用页面最常见；离页后 WeakReference+GC 只是抽查，最终用 heap dominator/root path 定位。",
                "VerifyCollectibleAsync"),
            new(LearningLevel.Intermediate, "集合通知",
                "ObservableCollection 千次 Add 触发千次通知/布局；分页、批量替换或范围集合减少 UI 抖动，但保留选择/增量语义。",
                "AddIndividuallyAsync counts notifications"),
            new(LearningLevel.Advanced, "图片预算",
                "压缩文件大小不等于解码内存，RGBA 约 width×height×4；按显示尺寸解码、缓存有界、列表取消离屏请求。",
                "EstimateDecodedImageBytes"),
            new(LearningLevel.Advanced, "CollectionView",
                "模板轻量、固定可预测尺寸、复用 DataTemplate、避免嵌套滚动和每项大图/转换器；滚动看 frame timeline。",
                "virtualization is protected by light templates"),
            new(LearningLevel.Advanced, "编译绑定与 SourceGen",
                "x:DataType 减少反射 path，XAML SourceGen 改善膨胀路径；转换器必须纯轻量并避免每次创建。",
                "stages 4/6 compile-time UI paths"),
            new(LearningLevel.Advanced, "UI 线程",
                "主线程只做短 UI work；CPU 计算下沉，I/O 直接 await，批量切回更新。抓现场时看 stack/trace，不先猜锁。",
                "MeasureAsync accepts async operation"),
            new(LearningLevel.Advanced, "网络与 DB",
                "请求去重/批量/分页/缓存减少唤醒，SQLite 事务和索引按 query shape；性能不能牺牲取消、离线正确性。",
                "operation spans can connect network and DB"),
            new(LearningLevel.Advanced, "包体",
                "看 APK/AAB 各 ABI、managed assembly、native lib、资源与符号贡献；trim/AOT 是发布权衡，不用 Debug 包判断。",
                "Stage19/20 handle release measurements"),
            new(LearningLevel.Expert, "工具优先顺序",
                "先计时/日志/Meter 定位，再 dotnet-trace/平台 profiler，最后 heap/CPU 深挖；Android 用 Perfetto/Studio，Windows 用 VS/ETW。",
                "instrumented names make external traces searchable"),
            new(LearningLevel.Expert, "崩溃、Sentry 与 OTel",
                "崩溃/ANR 与性能 trace 采样、脱敏、离线缓存；Aspire 可收遥测但不是后台任务/性能魔法。",
                "standard ActivitySource/Meter integration seam"),
            new(LearningLevel.Expert, "电量与网络 jank",
                "频繁唤醒、重试、GPS、传感器和小请求会耗电；性能报告同时记录 radio/电量/温度和后台约束。",
                "CUJ budget includes resource cost"),
            new(LearningLevel.Expert, "回归门禁",
                "固定设备/数据/版本跑多次，比较 median/p95 和阈值；噪声大先修实验，门禁保存 trace 与基线而非只给红灯。",
                "Snapshot provides serializable samples"),
        ];

        LearningLab[] labs =
        [
            new("测量异步操作", "记录真实 Stopwatch 时延、进程分配和三代 GC 差值，并写 Activity/Meter。",
                async token =>
                {
                    PerformanceSample sample = await telemetry.MeasureAsync(
                        "lab.delay",
                        async cancellationToken =>
                            await Task.Delay(100, cancellationToken),
                        token);
                    return Format(sample);
                }),
            new("比较集合通知", "向 ObservableCollection 单条加入 1000 项并记录通知次数与耗时。",
                async token =>
                {
                    (int notifications, TimeSpan duration) =
                        await PerformanceExperiments.AddIndividuallyAsync(1000, token);
                    return $"items=1000, notifications={notifications}, duration={duration.TotalMilliseconds:F2}ms";
                }),
            new("估算图片解码内存", "比较 4K RGBA 与 400px 缩略图的近似解码内存。",
                _ =>
                {
                    long full = PerformanceExperiments.EstimateDecodedImageBytes(3840, 2160);
                    long thumbnail = PerformanceExperiments.EstimateDecodedImageBytes(400, 225);
                    return Task.FromResult(
                        $"3840×2160 ≈ {full / 1024d / 1024d:F1} MiB\n" +
                        $"400×225 ≈ {thumbnail / 1024d:F1} KiB\n" +
                        $"ratio={full / (double)thumbnail:F1}x");
                }),
            new("泄漏抽查", "创建无根对象并通过 WeakReference/GC 抽查是否可回收。",
                async token =>
                {
                    bool collected =
                        await PerformanceExperiments.VerifyCollectibleAsync(token);
                    return $"collectible={collected}（真实页面泄漏仍需 heap root 分析）";
                }),
            new("查看应用内 trace", "列出最近启动、建窗和实验分段的测量样本。",
                _ => Task.FromResult(telemetry.Snapshot.Count == 0
                    ? "尚无样本。先运行测量实验。"
                    : string.Join(
                        Environment.NewLine,
                        telemetry.Snapshot.TakeLast(10).Select(Format)))),
        ];

        return new LearningStage(
            18,
            "性能、诊断与可观测闭环",
            "第7部分-阶段18-性能与诊断.md",
            topics,
            labs);
    }

    private static string Format(PerformanceSample sample)
    {
        return $"{sample.Name}: {sample.Duration.TotalMilliseconds:F2}ms, " +
               $"alloc={sample.AllocatedBytes / 1024d:F1}KiB, " +
               $"GC={sample.Gen0Collections}/{sample.Gen1Collections}/{sample.Gen2Collections}";
    }
}
