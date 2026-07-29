using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage23_Tizen;

internal static class StageCatalog
{
    public static LearningStage Create(IOptionalPlatformCapabilities capabilities)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "可选支线",
                "Tizen 由 Samsung 生态维护，不是 MAUI 10 Android/iOS/Windows/Mac Catalyst 主线，也不占 M1–M4 验收。",
                "Android-only target remains unchanged"),
            new(LearningLevel.Beginner, "默认 N/A",
                "没有 Samsung Tizen TV/设备合同、商店或 KPI，就书面标记阶段 N/A，不安装 workload 来凑平台数。",
                "TizenBusinessCase"),
            new(LearningLevel.Beginner, "启用条件",
                "明确目标设备、可复现工具链、签名 owner、真机/模拟器、TV UX owner 和稳定共享 Core 齐备后才进入 spike。",
                "business-case decision"),
            new(LearningLevel.Beginner, "TV 不是大号手机",
                "十英尺距离、横屏、安全区、大字号、D-pad 焦点、遥控返回和弱输入要求 UI 信息架构重做。",
                "TvFocusNavigator"),
            new(LearningLevel.Intermediate, "共享与重做",
                "网络、账户、领域模型和缓存可共享；页面结构、焦点路径、输入和媒体控制通常为 TV 单独实现。",
                "capability interface boundary"),
            new(LearningLevel.Intermediate, "工具链复核",
                "maui-tizen/workload 名称、Tizen SDK、模板、IDE 和支持设备会变化，实施前以 Samsung 当前一手文档重核并钉版本。",
                "OptionalWorkloadManifest"),
            new(LearningLevel.Intermediate, "目标框架隔离",
                "可选 TFM、独立 solution 配置和平台实现应隔离 TV 特化；不让可选 workload 破坏手机主线 restore/build。",
                "current Android csproj has no Tizen TFM"),
            new(LearningLevel.Intermediate, "平台服务",
                "平台能力封装在接口后，#if TIZEN 只留头项目/平台边缘；共享业务不直接引用 Samsung API。",
                "IOptionalPlatformCapabilities"),
            new(LearningLevel.Intermediate, "能力不对等",
                "后台、推送、生物识别、传感器和商店能力不得按 Android/iOS 推断；逐 API 记录 supported/fallback/evidence。",
                "CapabilityStatus matrix"),
            new(LearningLevel.Advanced, "第三方包",
                "逐个核对 NuGet 的 Tizen TFM、原生依赖和维护状态；不支持的 feature 通过能力矩阵关闭，不用运行时碰运气。",
                "capability fallback"),
            new(LearningLevel.Advanced, "焦点图",
                "D-pad 导航是有方向的焦点图，不是触摸手势；每个可交互元素要有稳定位置、禁用态和确定的下一焦点。",
                "TvFocusNavigator.Move"),
            new(LearningLevel.Advanced, "CI 非阻断",
                "无受控 Tizen runner 时，不把可选 TV job 设为主线 required；可用夜间/手动验证并单独报告。",
                "Android mainline build remains required"),
            new(LearningLevel.Advanced, "签名与商店",
                "Tizen 证书、profile、seller/商店规则与 Android 完全独立；secret、owner、备份和轮换需另建发布手册。",
                "workload manifest + signing owner gate"),
            new(LearningLevel.Expert, "版本周期独立",
                "Samsung SDK/maui-tizen 的节奏与 MAUI/.NET LTS 不等价；支持矩阵分别记录，不能用主线 EOS 推导 Tizen 支持。",
                "separate manifest"),
            new(LearningLevel.Expert, "Spike 出口",
                "空白页启动、共享 VM、一条 D-pad 焦点链、API 缺口、工具链清单和继续/冻结/放弃 ADR 完整后才决定采用。",
                "decision + evidence labs"),
            new(LearningLevel.Expert, "诚实声明",
                "本项目只验证 Android 降级与决策代码；未安装/未构建 Tizen 即明确 N/A，不能把条件代码编译当真机支持。",
                "AndroidFallbackCapabilities"),
        ];

        LearningLab[] labs =
        [
            new("做业务决策", "以当前无 Tizen 合同的基线得到明确 N/A。",
                unusedToken =>
                {
                    TizenBusinessCase businessCase = new(false, false, false, false, true);
                    return Task.FromResult($"decision={businessCase.Decide()}; mainline blocked=false");
                }),
            new("输出能力缺口", "列出当前平台能力、fallback 与证据，避免假设 API 对等。",
                unusedToken => Task.FromResult(
                    $"runtime={capabilities.PlatformName}\n" +
                    string.Join(
                        '\n',
                        capabilities.Inspect().Select(item =>
                            $"{item.Name}: supported={item.Supported}; fallback={item.Fallback}; evidence={item.Evidence}")))),
            new("模拟遥控焦点", "在二维焦点图中从播放按钮向右寻找下一可用节点。",
                unusedToken =>
                {
                    TvFocusNode current = new("play", 0, 0, true);
                    TvFocusNode[] nodes =
                    [
                        current,
                        new("details", 0, 1, true),
                        new("disabled", 0, 2, false),
                        new("settings", 1, 1, true),
                    ];
                    TvFocusNode? next = new TvFocusNavigator().Move(current, nodes, 0, 1);
                    return Task.FromResult($"from={current.Id}, right={next?.Id ?? "none"}");
                }),
            new("验证版本清单", "不完整工具链清单必须判为不可复现。",
                unusedToken =>
                {
                    OptionalWorkloadManifest manifest = new(
                        "maui-tizen (verify current name)",
                        string.Empty,
                        string.Empty,
                        "Samsung TV model TBD",
                        DateOnly.FromDateTime(DateTime.Today));
                    return Task.FromResult($"reproducible={manifest.IsReproducible}; action=run official-doc spike");
                }),
            new("生成 N/A 记录", "输出不阻塞主线的书面验收证据。",
                unusedToken => Task.FromResult(
                    "Stage 23: N/A\n" +
                    "Reason: no contractual Tizen target or test device.\n" +
                    "Mainline: Android/iOS/desktop milestones remain authoritative.\n" +
                    "Revisit trigger: signed Samsung TV requirement.")),
        ];

        return new LearningStage(
            23,
            "Tizen（可选）",
            "第8部分-阶段23-Tizen可选.md",
            topics,
            labs);
    }
}
