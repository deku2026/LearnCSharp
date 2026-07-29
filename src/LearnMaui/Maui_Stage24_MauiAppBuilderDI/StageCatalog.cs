using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage24_MauiAppBuilderDI;

internal static class StageCatalog
{
    public static LearningStage Create(
        LifetimeProbe lifetimeProbe,
        CompositionLedger ledger)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "平台入口到共享入口",
                "Android/iOS/Windows/Mac 各自由原生 runtime 创建宿主，再统一进入 MauiProgram.CreateMauiApp；共享层不反向控制原生进程。",
                "StartupCallChain"),
            new(LearningLevel.Beginner, "MauiAppBuilder",
                "CreateBuilder 创建可变装配器，UseMauiApp 注册 Application，随后依次配置字体、Handler、生命周期、配置、日志和服务。",
                "MauiProgram ordered composition"),
            new(LearningLevel.Beginner, "Build 边界",
                "builder.Build 生成 MauiApp 与根 IServiceProvider；之后服务图视为不可变，测试替换必须在 Build 之前完成。",
                "CompositionLedger"),
            new(LearningLevel.Beginner, "构造注入",
                "App、Page、ViewModel 和服务从构造函数声明依赖；禁止 Application.Current 取容器或静态 ServiceLocator。",
                "App/MainPage constructors"),
            new(LearningLevel.Intermediate, "组合根唯一",
                "平台注册、配置绑定、日志、HttpClient、数据库和页面装配集中在 MauiProgram 的模块化扩展方法，业务代码不注册自身。",
                "ServiceRegistration extensions"),
            new(LearningLevel.Intermediate, "Singleton",
                "进程内一份，适合线程安全的配置、缓存协调与会话；不能抓 Page/View/Activity，否则形成长生命周期 captive reference。",
                "AppSession"),
            new(LearningLevel.Intermediate, "Transient",
                "每次解析新实例，适合轻量无状态服务与页面；它依赖的昂贵 IDisposable 仍需明确 owner。",
                "OperationToken + MainPage"),
            new(LearningLevel.Intermediate, "Scoped",
                "MAUI 没有 HTTP request 自动 scope；页面/工作流需要显式 CreateScope 并在离开时 Dispose，不能从 root 直接长期解析 scoped。",
                "LifetimeProbe explicit scopes"),
            new(LearningLevel.Intermediate, "配置对象",
                "endpoint、timeout 和 feature policy 以验证后的强类型配置注入；secret 不进 appsettings/源码，移动客户端也不保存服务端秘密。",
                "EndpointOptions"),
            new(LearningLevel.Advanced, "MauiContext",
                "Handler 通过 MauiContext 接到同一个服务树和平台上下文；它是框架桥，不是业务随处 GetService 的许可。",
                "StartupCallChain handler segment"),
            new(LearningLevel.Advanced, "注册顺序",
                "多个 Configure/mapper 委托按注册顺序组合；Prepend/Modify/Append 的覆盖关系要有测试，避免隐含最后写入胜出。",
                "ordered MauiProgram"),
            new(LearningLevel.Advanced, "模块化注册",
                "AddApplicationCore/AddPresentation 按依赖方向拆分并返回 IServiceCollection，便于测试宿主复用和审查注册归属。",
                "ServiceRegistration"),
            new(LearningLevel.Advanced, "捕获依赖",
                "Singleton 构造注入 Scoped/Transient 可能把短寿命对象提升为进程寿命；通过 scope validation、生命周期图和泄漏测试发现。",
                "LifetimeEvidence"),
            new(LearningLevel.Advanced, "平台对比",
                "各平台原生入口与生命周期回调不同，但 Build 后都由同一 MauiApp/Services 创建 App、Window、Page 和 Handler。",
                "shared call-chain model"),
            new(LearningLevel.Advanced, "日志装配",
                "provider 和最小级别在 Build 前按 Debug/Release 配置；日志注入 ILogger<T>，不得写 token、密钥或个人数据。",
                "composition surface documented"),
            new(LearningLevel.Expert, "源码断点链",
                "断点 CreateMauiApp、Build、App 构造、CreateWindow、Page 构造、SetHandler/ConnectHandler，记录线程、实例和服务解析路径。",
                "StartupCallChain"),
            new(LearningLevel.Expert, "测试替换",
                "测试建立自己的 ServiceCollection/host，在 Build 前用 fake 替换接口；不要修改已运行应用的根容器。",
                "extension registration seam"),
            new(LearningLevel.Expert, "线程安全",
                "DI 容器保证解析并不等于服务状态线程安全；Singleton 可被 UI、后台和 Handler 同时调用，内部同步需自行设计。",
                "stateless AppSession identity only"),
            new(LearningLevel.Expert, "Dispose 所有权",
                "容器释放其创建的 IDisposable；手工 new 的对象由调用方释放，scope 必须短而明确，App 退出不能依赖终结器。",
                "PageSession + using scopes"),
            new(LearningLevel.Expert, "专家验收",
                "能够画出 platform entry→builder→Build→provider→App/Window→Handler/MauiContext，并用实例 ID 证明三种寿命。",
                "runnable lifetime and chain labs"),
        ];

        LearningLab[] labs =
        [
            new("验证三种寿命", "在两个显式 scope 中比较 Singleton、Scoped 和 Transient 实例身份。",
                unusedToken =>
                {
                    LifetimeEvidence evidence = lifetimeProbe.Inspect();
                    return Task.FromResult(
                        $"singleton same={evidence.SingletonSameAcrossScopes} ({evidence.SingletonId})\n" +
                        $"scoped same in scope={evidence.ScopedSameWithinScope}\n" +
                        $"scoped differs={evidence.ScopedDifferentAcrossScopes} ({evidence.FirstScopeId} / {evidence.SecondScopeId})\n" +
                        $"transient differs={evidence.TransientDifferent}");
                }),
            new("演示 Build 不可变", "密封组合账本后，任何迟到注册都被拒绝。",
                unusedToken =>
                {
                    if (ledger.Phase == CompositionPhase.Created)
                    {
                        ledger.Register("Core");
                        ledger.Advance(CompositionPhase.FrameworkConfigured);
                        ledger.Advance(CompositionPhase.ServicesRegistered);
                        ledger.Advance(CompositionPhase.Built);
                    }

                    try
                    {
                        ledger.Register("Late override");
                        return Task.FromResult("unexpected: late registration accepted");
                    }
                    catch (InvalidOperationException exception)
                    {
                        return Task.FromResult($"rejected: {exception.Message}");
                    }
                }),
            new("打印 Android 启动链", "输出平台入口到 Handler/MauiContext 的共享解析路径。",
                unusedToken => Task.FromResult(StartupCallChain.DescribeAndroid())),
            new("检查 Captive Dependency", "用寿命矩阵识别 Singleton 捕获页面 Scope 的危险。",
                unusedToken => Task.FromResult(
                    "AppSession(Singleton) -> config/cache: valid\n" +
                    "AppSession(Singleton) -> PageSession(Scoped): BLOCK\n" +
                    "Page workflow scope -> PageSession: valid; dispose on exit")),
            new("验证模块边界", "列出组合根模块与依赖方向。",
                unusedToken => Task.FromResult(
                    "MauiProgram\n" +
                    "  -> AddApplicationCore (options, domain services, lifetimes)\n" +
                    "  -> AddPresentation (pages/view-models)\n" +
                    "Business code -> interfaces only; no container access")),
        ];

        return new LearningStage(
            24,
            "MauiAppBuilder、Host 与 DI 装配",
            "第9部分-01-MauiAppBuilder-Host-DI.md",
            topics,
            labs);
    }
}
