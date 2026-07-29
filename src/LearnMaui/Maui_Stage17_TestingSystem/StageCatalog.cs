using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage17_TestingSystem;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "测试目标",
                "测试降低发布风险、缩短反馈并保护契约，不追逐数字。按失败代价把逻辑放到最便宜且足够真实的层。",
                "ExecutableTestSuite contracts"),
            new(LearningLevel.Beginner, "可测核心分层",
                "Domain/Application/Core 保持普通 net10.0 可测代码，MAUI Host 只做 UI/平台组合；不要给 MAUI app 强改 TFM 冒充单元测试。",
                "LoadViewModel has no MAUI type"),
            new(LearningLevel.Beginner, "测公开契约",
                "断言 State、Value、Error、通知与依赖调用，不反射私有字段；重构内部实现时测试不应无故碎裂。",
                "TestSuccessAsync/TestErrorAsync"),
            new(LearningLevel.Beginner, "AAA 与单一原因",
                "Arrange 建替身、Act 执行一个行为、Assert 检查结果；失败信息说明哪个业务契约破坏。",
                "Executable test case methods"),
            new(LearningLevel.Beginner, "异步测试",
                "测试方法返回 Task 并 await，禁止 Thread.Sleep、async void 和同步 Wait；用可控 gate/clock/Token 驱动。",
                "TestCase Func<CancellationToken, Task>"),
            new(LearningLevel.Intermediate, "INPC 契约",
                "属性只在值改变时通知正确名称，派生属性也通知；测试最终状态与关键通知，不锁死无意义顺序。",
                "TestPropertyChangedAsync"),
            new(LearningLevel.Intermediate, "失败路径",
                "权限 denied/restricted、网络 401/500/坏 JSON、存储失败、取消与超时都是一等测试，不只写 happy path。",
                "FakeDataGateway.Exception + cancellation"),
            new(LearningLevel.Intermediate, "手写 Fake",
                "简单接口用可配置 Fake 比脆弱 mock 表达更清楚；Fake 返回值、异常、延迟并可记录调用。",
                "FakeDataGateway"),
            new(LearningLevel.Intermediate, "时间与调度",
                "注入 TimeProvider/IClock，测试推进虚拟时间，不依赖 DateTime.Now 和真实延迟；超时测试应确定。",
                "FakeClock"),
            new(LearningLevel.Intermediate, "仓储测试",
                "纯映射/冲突用 fake，SQLite Repository 用临时真实 DB 验证 schema/事务/迁移；每个测试隔离路径并清理。",
                "Stage12 repository seam"),
            new(LearningLevel.Advanced, "DeviceRunners",
                "需要 Handler、Essentials 或原生 API 的测试打包进设备 runner，在 emulator/device 内执行；不与主机单测职责重复。",
                "device-specific tests are a separate project concern"),
            new(LearningLevel.Advanced, "XHarness",
                "XHarness 安装、启动设备测试 app、收集结果与崩溃日志，适合 CI 编排；环境失败与产品失败要区分。",
                "host orchestration belongs in CI scripts"),
            new(LearningLevel.Advanced, "Appium UI smoke",
                "只覆盖高价值主流程：启动、登录/导航、创建、错误恢复。页面对象封装稳定 AutomationId 与等待条件。",
                "LearningPageObject"),
            new(LearningLevel.Advanced, "稳定定位",
                "AutomationId 是测试契约；禁止按坐标、文本翻译或视觉树深度定位。语义/可见/可交互条件后再点击。",
                "TestingDemoView AutomationIds"),
            new(LearningLevel.Advanced, "Flaky 治理",
                "显式等待状态而非固定 sleep，隔离账户/数据/端口，失败保留截图、日志、page source；重试只作诊断不能掩盖。",
                "deterministic core suite has no sleep"),
            new(LearningLevel.Advanced, "快照测试边界",
                "Verify 适合稳定序列化/DTO/生成输出；像素 UI 易受平台字体/渲染影响，不作为跨平台主门禁。",
                "behavior contracts over pixel snapshots"),
            new(LearningLevel.Expert, "CI 分层门禁",
                "PR 跑格式/主机单测/核心构建，主干跑平台构建，计划任务跑 emulator/device/UI smoke；失败产物可追溯。",
                "LearnCSharp.CI.slnx excludes Android by repository policy"),
            new(LearningLevel.Expert, "覆盖率",
                "覆盖率找未测区域，不代表正确性。风险表优先支付、认证、迁移、冲突、权限与失败恢复，再看行覆盖。",
                "five high-signal contracts in executable suite"),
            new(LearningLevel.Expert, "平台权限自动化",
                "Appium 前置用 adb/simctl 预授权或重置权限；应用仍测试拒绝后的产品 UX，测试代码不绕过真实门闸。",
                "permission setup belongs to platform harness"),
            new(LearningLevel.Expert, "测试金字塔与奖杯",
                "大量 Core 单测，中量真实存储/HTTP 组件测试，少量设备和 UI smoke；每层回答不同问题，不重复堆数量。",
                "core suite + XAML AutomationId seam"),
        ];

        LearningLab[] labs =
        [
            new("运行可执行测试套件", "按顺序运行成功、错误、取消、INPC 和虚拟时间五个契约。",
                async token =>
                {
                    ExecutableTestSuite suite = new();
                    IReadOnlyList<TestResult> results = await suite.RunAsync(token);
                    return string.Join(
                        Environment.NewLine,
                        results.Select(result =>
                            $"{(result.Passed ? "PASS" : "FAIL")} {result.Name} ({result.Duration.TotalMilliseconds:F0}ms) {result.Failure}"));
                }),
            new("验证 Fake 错误", "让 FakeDataGateway 抛出离线错误，VM 应进入 Error 且不崩溃。",
                async token =>
                {
                    LoadViewModel viewModel = new(
                        new FakeDataGateway { Exception = new IOException("offline") },
                        new FakeClock(DateTimeOffset.UnixEpoch));
                    await viewModel.LoadAsync(token);
                    return $"State={viewModel.State}, Error={viewModel.Error}";
                }),
            new("验证取消", "使用已取消 Token，断言 VM 回到 Idle 并把取消传播给调用者。",
                async token =>
                {
                    using CancellationTokenSource canceled =
                        CancellationTokenSource.CreateLinkedTokenSource(token);
                    canceled.Cancel();
                    LoadViewModel viewModel = new(
                        new FakeDataGateway { Delay = TimeSpan.FromSeconds(1) },
                        new FakeClock(DateTimeOffset.UnixEpoch));
                    try
                    {
                        await viewModel.LoadAsync(canceled.Token);
                        return "FAIL: cancellation was not observed";
                    }
                    catch (OperationCanceledException)
                    {
                        return $"PASS: State={viewModel.State}";
                    }
                }),
            new("验证 UI 定位契约", "实例化 XAML 并输出 Page Object 的稳定 AutomationId。",
                _ =>
                {
                    TestingDemoView view = new();
                    return Task.FromResult(
                        $"view={view.GetType().Name}\nquery={LearningPageObject.Query}\nload={LearningPageObject.Load}\nresult={LearningPageObject.Result}");
                }),
            new("验证虚拟时间", "固定 clock 后加载，结果时间必须完全等于预设值。",
                async token =>
                {
                    DateTimeOffset expected =
                        new(2040, 2, 3, 4, 5, 6, TimeSpan.Zero);
                    LoadViewModel viewModel = new(
                        new FakeDataGateway(),
                        new FakeClock(expected));
                    await viewModel.LoadAsync(token);
                    return $"expected={expected:O}\nactual={viewModel.LastLoadedAt:O}";
                }),
        ];

        return new LearningStage(
            17,
            "测试体系与自动化边界",
            "第7部分-阶段17-测试体系.md",
            topics,
            labs);
    }
}
