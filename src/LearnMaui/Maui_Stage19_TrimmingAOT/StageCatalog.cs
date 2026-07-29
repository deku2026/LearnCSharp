using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage19_TrimmingAOT;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "Trimming 做什么",
                "linker 静态分析可达 IL 并删除未使用代码；字符串反射、动态加载和序列化约定可能隐藏依赖，所以必须发布构建验证。",
                "PublishTrimmed + TrimMode=partial in Release"),
            new(LearningLevel.Beginner, "partial vs full",
                "partial 只裁明确可裁程序集，风险较低；full 更激进且要求整个依赖图 trim-safe。先清警告/行为测试再扩大。",
                "Release TrimMode=partial"),
            new(LearningLevel.Beginner, "JIT 与 AOT",
                "JIT 运行时编译、启动可慢但动态友好；AOT 提前编译、启动/平台合规更好但包体和动态代码限制更强。",
                "RuntimeFeature flags"),
            new(LearningLevel.Beginner, "警告是正确性信号",
                "IL2026/IL2057/IL3050 等不能全局 suppress；定位调用链，改 source-gen/显式注册/注解，例外记录平台、理由和退出条件。",
                "EnableTrimAnalyzer + SuppressTrimAnalysisWarnings=false"),
            new(LearningLevel.Intermediate, "DynamicallyAccessedMembers",
                "当 API 接收 Type/泛型并反射构造时，把所需成员契约标到数据流上，让 linker 保留精确成员并验证调用者。",
                "TrimSafeActivator.Create<T>"),
            new(LearningLevel.Intermediate, "RequiresUnreferencedCode",
                "无法静态保证的反射 API明确标记，把警告推给调用边界；不是消音，而是要求上层隔离或避免。",
                "DiscoverByConvention"),
            new(LearningLevel.Intermediate, "RequiresDynamicCode",
                "表达 API 在 Native AOT 可能依赖运行时代码生成；调用者选择 source-gen、预生成或平台降级。",
                "DiscoverByConvention dynamic boundary"),
            new(LearningLevel.Intermediate, "DynamicDependency",
                "仅当数据流注解无法表达时精确保留成员；它易过保留且重构脆弱，需测试和注释。",
                "GetPreservedPropertyNames"),
            new(LearningLevel.Intermediate, "JSON SourceGen",
                "JsonSerializerContext 明确根 DTO/集合并生成元数据，消除反射序列化警告；DTO→Domain 仍分层。",
                "ReleaseJsonContext"),
            new(LearningLevel.Intermediate, "GeneratedRegex",
                "正则源生成器把模式代码化，AOT 友好且避免运行时编译；给外部输入设置超时和长度限制。",
                "InputPatterns.SafeIdentifier"),
            new(LearningLevel.Advanced, "显式 DI/视图工厂",
                "不要扫描命名约定自动找 Page/ViewModel；显式字典/DI 注册让类型可达、生命周期清晰、未知 key 提前失败。",
                "AotFriendlyViewFactory"),
            new(LearningLevel.Advanced, "XAML 与编译绑定",
                "XAML SourceGen/XamlC、x:DataType 和严格编译减少反射；动态 LoadFromXaml、字符串路径和模板上下文需发布审计。",
                "MauiXamlInflator=SourceGen + MauiStrictXamlCompilation"),
            new(LearningLevel.Advanced, "Shell 导航参数",
                "IQueryAttributable 显式读取标量/类型化 intent；反射属性路径在 full trim/AOT 下必须核对当前支持。",
                "stage 7 explicit route parsing"),
            new(LearningLevel.Advanced, "第三方库矩阵",
                "每个包记录版本、trim/AOT 警告、动态代码、平台、发布 smoke 与替代方案；仅 Debug 能跑不等于兼容。",
                "dependency gate before package adoption"),
            new(LearningLevel.Advanced, "Native AOT iOS/Mac",
                "在 Mac 工具链按 TFM 开启 PublishAot，逐项验证反射、泛型、P/Invoke、序列化、资源和崩溃符号；收益与包体/构建时权衡。",
                "Android project documents but does not fake Apple publish"),
            new(LearningLevel.Advanced, "Android AOT",
                "Release RunAOTCompilation 改善启动但增加构建/包体；设备测 APK/AAB 各 ABI、启动、内存和功能，不能用 Debug 判断。",
                "RunAOTCompilation=true in Release"),
            new(LearningLevel.Expert, "符号与诊断",
                "裁剪/AOT 后 stack 仍需 managed/native symbols、mapping 和 commit/version；商店符号分离上传，生产保留脱敏遥测。",
                "release artifact must archive symbols/binlog"),
            new(LearningLevel.Expert, "Mono vs CoreCLR",
                "Android 默认运行时与实验 CoreCLR 的 JIT/AOT/diagnostics 行为不同；趋势功能需官方支持矩阵和独立基线，不进默认主线。",
                "RuntimeCapabilities captures actual framework"),
            new(LearningLevel.Expert, "发布验证",
                "Debug→Release build→Publish+binlog→零未解释警告→安装真实产物→冷启/导航/JSON/DI/权限/离线/深链行为表。",
                "Release properties + behavior labs"),
            new(LearningLevel.Expert, "故障演习",
                "商店白屏先查资源/XAML trim，iOS JSON 崩查 Context root，跨平台参数丢查反射属性；保留最小复现和警告账本。",
                "source-generated and explicit paths reduce blast radius"),
        ];

        LearningLab[] labs =
        [
            new("JSON SourceGen 往返", "使用生成的 JsonTypeInfo 序列化/反序列化，不调用基于反射的泛型 overload。",
                _ =>
                {
                    ReleaseArticle article = new(
                        Guid.Parse("00000000-0000-0000-0000-000000000019"),
                        "Trim safe");
                    string json = TrimSafeJson.Serialize(article);
                    ReleaseArticle? restored = TrimSafeJson.Deserialize(json);
                    return Task.FromResult($"{json}\nroundtrip={restored == article}");
                }),
            new("GeneratedRegex", "验证源生成 identifier 正则对安全值与路径穿越的处理。",
                _ => Task.FromResult(
                    $"article-19={InputPatterns.SafeIdentifier().IsMatch("article-19")}\n" +
                    $"../secret={InputPatterns.SafeIdentifier().IsMatch("../secret")}")),
            new("显式视图工厂", "按显式 key 创建 Page，并列出所有 linker 可见的注册项。",
                _ =>
                {
                    AotFriendlyViewFactory factory = new();
                    Page page = factory.Create("settings");
                    return Task.FromResult(
                        $"page={page.Title}, keys={string.Join(", ", factory.Keys)}");
                }),
            new("运行时能力", "读取实际 Android 运行时是否支持/编译动态代码与进程架构。",
                _ =>
                {
                    RuntimeCapabilities capabilities = RuntimeCapabilities.Capture();
                    return Task.FromResult(
                        $"framework={capabilities.Framework}\narchitecture={capabilities.Architecture}\n" +
                        $"dynamic supported={capabilities.DynamicCodeSupported}, compiled={capabilities.DynamicCodeCompiled}");
                }),
            new("注解反射边界", "验证 DynamicallyAccessedMembers 构造与 DynamicDependency 属性保留。",
                _ =>
                {
                    LegacyPayload payload = TrimSafeActivator.Create<LegacyPayload>();
                    IReadOnlyList<string> properties =
                        TrimSafeActivator.GetPreservedPropertyNames();
                    return Task.FromResult(
                        $"created={payload.GetType().Name}, properties={string.Join(", ", properties)}");
                }),
        ];

        return new LearningStage(
            19,
            "Trimming、AOT 与运行时",
            "第7部分-阶段19-Trimming-AOT-运行时.md",
            topics,
            labs);
    }
}
