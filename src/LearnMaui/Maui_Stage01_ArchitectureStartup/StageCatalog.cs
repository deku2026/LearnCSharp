using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage01_ArchitectureStartup;

internal static class StageCatalog
{
    public static LearningStage Create(
        IStartupClock clock,
        AppSession session,
        IServiceProvider services)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "端到端启动链",
                "平台原生入口只负责进入 .NET；共享组合根依次创建 MauiAppBuilder、注册能力、Build，随后 Application.CreateWindow 创建 Window/Page，最后 Handler 把虚拟视图映射到原生视图。",
                "MainApplication.CreateMauiApp → MauiProgram.CreateMauiApp → builder.Build → App.CreateWindow"),
            new(LearningLevel.Beginner, "组合根只有一个",
                "MauiProgram.CreateMauiApp 是应用装配的唯一中心。业务服务、页面、日志、字体和 Handler 在 Build 前注册，平台入口不直接 new 业务单例。",
                "MauiProgram.CreateMauiApp + AddArchitectureStartup"),
            new(LearningLevel.Beginner, "MauiAppBuilder 与 Build",
                "Builder 聚合 Services、Configuration、Logging、Fonts、Handlers 和生命周期配置。Build 冻结注册并产生 MauiApp 及根 IServiceProvider。",
                "MauiApp.CreateBuilder(); builder.Build()"),
            new(LearningLevel.Beginner, "Application、Window、Page 分工",
                "Application 管全局资源与建窗策略；Window 表示可激活的顶层窗口；Page 管页面内容与交互。业务表单不应塞进 App。",
                "App.CreateWindow returns new Window(_mainPage)"),
            new(LearningLevel.Beginner, "Android 原生入口",
                "MainActivity 承载 Activity 与配置变化，MainApplication 继承 MauiApplication 并把 CreateMauiApp 转给共享组合根。iOS/Mac Catalyst/Windows 使用不同原生桥，但共享入口不变。",
                "Platforms/Android/MainApplication.cs and MainActivity.cs"),
            new(LearningLevel.Intermediate, "构造函数注入",
                "页面显式声明 IStartupClock、AppSession、IServiceProvider 与 ILogger 依赖。可测试依赖用构造注入；只有框架边界或诊断实验才显式使用 Services。",
                "MainPage(IStartupClock, AppSession, IServiceProvider, ILogger<MainPage>)"),
            new(LearningLevel.Intermediate, "客户端 DI 生命周期",
                "Singleton 在根容器内复用，Transient 每次解析产生新对象。Scoped 只有显式 CreateScope 才有边界，移动应用不会自动按请求或页面建 Scope。",
                "AddSingleton<AppSession>(); AddTransient<StartupProbe>()"),
            new(LearningLevel.Intermediate, "Page 与 ViewModel 生命周期",
                "有可变页面状态的 Page/ViewModel 通常注册为 Transient，避免跨导航串状态；应用级缓存与协调器才适合 Singleton。单窗 Shell 可复用，多窗需按窗隔离。",
                "services.AddTransient<MainPage>()"),
            new(LearningLevel.Intermediate, "CreateWindow 延迟解析",
                "根页面应在 Application 初始化资源之后通过 DI 得到，再传给 CreateWindow。这样避免过早解析页面导致 StaticResource 尚未准备好。",
                "App constructor receives MainPage after InitializeComponent is available"),
            new(LearningLevel.Intermediate, "日志证明启动顺序",
                "ILogger<T> 在服务和页面构造时记录时间与实例标识，用证据判断构造顺序和生命周期，而不是依赖断点猜测。",
                "builder.Logging.AddProvider(new StartupLoggerProvider()); ILogger<SystemStartupClock>"),
            new(LearningLevel.Intermediate, "字体与 Handler 装配面",
                "ConfigureFonts 和 ConfigureMauiHandlers 属于组合根。字体别名与全局原生映射在启动时一次注册，不应散落在页面事件中。",
                "UseMauiApp<App>().ConfigureFonts(...)"),
            new(LearningLevel.Advanced, "模块化注册",
                "用返回 IServiceCollection 的扩展方法按 Core、Infrastructure、Presentation 拆分注册，同时让 CreateMauiApp 保持可顺序阅读。",
                "AddArchitectureStartup(this IServiceCollection services)"),
            new(LearningLevel.Advanced, "Build 后不可变",
                "服务描述在 Build 时转成服务提供器；运行期不要试图追加注册。测试替换应在 Build 前完成，或构建一个独立测试容器。",
                "all AddSingleton/AddTransient calls precede builder.Build()"),
            new(LearningLevel.Advanced, "激活状态与多窗口",
                "CreateWindow 接收 IActivationState。单窗口示例复用根页；产品级多窗应为每个 Window 创建独立导航根和窗口状态，不能把当前页面存在静态字段。",
                "CreateWindow(IActivationState? activationState)"),
            new(LearningLevel.Advanced, "显式 Scope 的真实边界",
                "需要一次工作单元时可由拥有者 CreateScope，并在任务结束释放；不要把 Scoped 误认为页面自动作用域，也不要让 Scope 内服务泄漏到 Singleton。",
                "IServiceScopeFactory.CreateScope is deliberate, not automatic"),
            new(LearningLevel.Expert, "Generic Host 的同构与边界",
                "MAUI 借用 Microsoft.Extensions 的 DI、配置与日志抽象，但它没有 ASP.NET Core 的每请求管线与自动 Request Scope；UI 生命周期仍由平台和 Window/Page 驱动。",
                "MauiAppBuilder.Services/Configuration/Logging"),
            new(LearningLevel.Expert, "Handler 与 MauiContext",
                "Element 获得 Handler 后，Handler 通过 MauiContext 接入同一 IServiceProvider 并创建 PlatformView；这是共享 UI、依赖服务和平台视图汇合的位置。",
                "Element.Handler → IMauiContext.Services → PlatformView"),
            new(LearningLevel.Expert, "源码断点与所有权",
                "从 MauiApplication.CreateMauiApp、MauiAppBuilder.Build、Application.CreateWindow 到 Element.ToPlatform 设置断点，可以验证对象所有权：MauiApp 拥有根服务，Window 拥有页面树，Handler 连接平台对象。",
                "MainApplication.cs + MauiProgram.cs + App.xaml.cs"),
            new(LearningLevel.Expert, "现代化边界",
                "新代码使用 Window 模型和 Handler，不沿用 Xamarin.Forms Renderer、DependencyService 或在 App 构造函数直接设置旧 MainPage 属性的主线做法。",
                "CreateWindow + constructor DI + handlers"),
        ];

        LearningLab[] labs =
        [
            new("打印启动链", "观察组合根完成后，应用、窗口和页面所处的启动顺序。",
                _ => Task.FromResult(
                    $"平台入口 → CreateMauiApp → Build → App → CreateWindow → MainPage → Handler\n" +
                    $"当前时间：{clock.GetNow():O}\n应用会话：{session.Id}\n" +
                    $"已捕获日志：{StartupLoggerProvider.Snapshot.Count} 条")),
            new("验证 DI 生命周期", "比较 Singleton 与 Transient 连续两次解析是否为同一实例。",
                _ =>
                {
                    AppSession firstSession = services.GetRequiredService<AppSession>();
                    AppSession secondSession = services.GetRequiredService<AppSession>();
                    StartupProbe firstProbe = services.GetRequiredService<StartupProbe>();
                    StartupProbe secondProbe = services.GetRequiredService<StartupProbe>();
                    return Task.FromResult(
                        $"Singleton: {firstSession.Id} / {secondSession.Id}, Same={ReferenceEquals(firstSession, secondSession)}\n" +
                        $"Transient: {firstProbe.Id} / {secondProbe.Id}, Same={ReferenceEquals(firstProbe, secondProbe)}");
                }),
            new("检查服务定位边界", "演示 IServiceProvider 只用于框架边界与诊断；业务代码仍应构造注入。",
                _ => Task.FromResult(
                    $"IStartupClock 可解析：{services.GetService<IStartupClock>() is not null}\n" +
                    $"MainPage 使用构造注入；本实验保留 IServiceProvider 仅用于观察容器。")),
        ];

        return new LearningStage(
            1,
            "架构、启动与组合根",
            "第2部分-阶段1-架构与启动.md",
            topics,
            labs);
    }
}
