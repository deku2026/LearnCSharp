using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage21_BlazorHybrid;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "两条 Hybrid 路径",
                "BlazorWebView 运行 Razor 组件和 Blazor 渲染器；HybridWebView 承载任意本地 HTML/JS/CSS，不运行 Blazor。先按复用资产和团队能力做 ADR。",
                "HybridLabPage uses HybridWebView; Blazor package is intentionally absent"),
            new(LearningLevel.Beginner, "适用边界",
                "复用成熟 Web UI、报表或本地知识库是绿灯；纯原生体验、极致冷启动、复杂手势或 Native AOT 硬约束默认优先原生。",
                "selection topics + runnable local web lab"),
            new(LearningLevel.Beginner, "本地资源根",
                "HybridRoot 默认映射 Resources/Raw/wwwroot，DefaultFile 为 index.html；资产随签名 App 版本发布并可离线。",
                "Resources/Raw/wwwroot"),
            new(LearningLevel.Intermediate, "原生壳分工",
                "Shell/NavigationPage 管系统返回、深链和平台导航，Web 内 history 只处理当前页面内部状态，避免两套导航互相争抢。",
                "NavigationPage opens HybridLabPage"),
            new(LearningLevel.Intermediate, ".NET → JavaScript",
                "InvokeJavaScriptAsync 调具名函数，参数与返回值提供 JsonTypeInfo，避免反射并让 trim/AOT 风险可见；脚本异常必须回抛。",
                "HybridJsonContext + InvokeJavaScriptAsync"),
            new(LearningLevel.Intermediate, "JavaScript → .NET",
                "SetInvokeJavaScriptTarget 暴露最小白名单对象；public 方法就是攻击面，必须校验参数、限长且不得暴露 SecureStorage 等高权限能力。",
                "HybridBridge"),
            new(LearningLevel.Intermediate, "Raw 消息",
                "SendRawMessage/RawMessageReceived 适合非 RPC 事件；定义 envelope、版本和大小上限，并在页面离开时解除事件订阅。",
                "OnRawMessageReceived + cleanup"),
            new(LearningLevel.Intermediate, "浏览器引擎差异",
                "Android 使用系统 Chromium WebView，Apple 使用 WKWebView，Windows 使用 WebView2；CSS、键盘、安全区、文件选择与返回手势需逐平台验证。",
                "HandlerChanged platform hook"),
            new(LearningLevel.Advanced, "初始化钩子",
                "WebViewInitializing/Initialized 分别覆盖原生控件创建前后；只做安全收紧和必要平台配置，不开启任意文件访问。",
                "initialization events + Android settings"),
            new(LearningLevel.Advanced, "请求拦截",
                "WebResourceRequested 必须同步；Handled=true 时必须 SetResponse。长内容把 Task<Stream> 交给响应，不在 WebView 线程阻塞。",
                "offline /health.json response"),
            new(LearningLevel.Advanced, "拦截能力矩阵",
                "Android 不能改写后继续且不支持 custom scheme；Apple 不拦 http/https；Windows 能力最全。离线与鉴权方案不能假设四端一致。",
                "platform limitation is explicit in topics"),
            new(LearningLevel.Advanced, "本地与远程信任域",
                "本地可信根可接有限桥；远程页面默认只展示。远程内容和高权限 .NET 桥不得无隔离共存。",
                "local-only index + no remote navigation"),
            new(LearningLevel.Advanced, "CSP 与 XSS",
                "CSP 限定 self，禁止未净化 innerHTML/MarkupString，避免 unsafe-eval；调试工具只在 Debug 注册。",
                "index.html CSP + conditional developer tools"),
            new(LearningLevel.Advanced, "DI 作用域",
                "Blazor Scoped 以根组件电路/作用域为界而非 HTTP 请求；Singleton 不抓 UI，原生代码解析 Scoped 时要显式创建和释放 scope。",
                "singleton bridge has no View reference"),
            new(LearningLevel.Expert, "互操作所有权",
                "Blazor 的 IJSObjectReference/DotNetObjectReference 要有 owner 和 Dispose；HybridWebView 的事件与桥对象同样必须随页面生命周期解绑。",
                "OnDisappearing cleanup"),
            new(LearningLevel.Expert, "Trim 与 Native AOT",
                ".NET 10 full trim/Native AOT 默认可能移除 HybridWebView；SourceGen JSON 只解决部分互操作元数据，发布矩阵需单独实测并记录决策。",
                "HybridJsonContext + explicit limitation"),
            new(LearningLevel.Expert, "JS 异常边界",
                "启用 HybridWebView.InvokeJavaScriptThrowsExceptions，让 JS 失败成为可捕获的 .NET 异常，而不是静默返回假成功。",
                "MauiProgram static switch"),
            new(LearningLevel.Expert, "Windows WebView2 数据目录",
                "安装到 Program Files 时缓存目录可能不可写；任何 WebView 创建前把 WEBVIEW2_USER_DATA_FOLDER 指向 AppDataDirectory。",
                "cross-platform requirement documented; Android build ignores Windows-only item"),
            new(LearningLevel.Expert, "发布安全门",
                "Release 关闭开发者工具，审计第三方前端依赖和 CSP，在真机验证离线、异常、导航、桥滥用与平台 WebView 版本。",
                "DEBUG-only tooling + local signed assets"),
            new(LearningLevel.Expert, "选型证据",
                "ADR 记录为什么选原生、BlazorWebView 或 HybridWebView，以及启动、包体、AOT、团队、无障碍和平台差异的退出条件。",
                "decision lab"),
        ];

        LearningLab[] labs =
        [
            new("验证桥白名单", "运行参数校验、限长和稳定返回契约。",
                _ =>
                {
                    HybridBridge bridge = new();
                    string ok = bridge.Echo("hello");
                    bool rejected;
                    try
                    {
                        bridge.Echo(new string('x', 1025));
                        rejected = false;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        rejected = true;
                    }

                    return Task.FromResult($"{ok}\noversize rejected={rejected}");
                }),
            new("检查本地资源", "确认入口、桥脚本和 CSP 资产已打包进 App。",
                async token =>
                {
                    string[] files = ["wwwroot/index.html", "wwwroot/HybridWebView.js", "wwwroot/app.css"];
                    List<string> evidence = [];
                    foreach (string file in files)
                    {
                        await using Stream stream = await FileSystem.OpenAppPackageFileAsync(file);
                        evidence.Add($"{file}: {stream.Length} bytes");
                        token.ThrowIfCancellationRequested();
                    }

                    return string.Join('\n', evidence);
                }),
            new("模拟信任决策", "证明只有 app-local 源获得有限桥能力。",
                _ =>
                {
                    string[] origins = ["app://local", "https://docs.example", "http://legacy.example"];
                    return Task.FromResult(string.Join(
                        '\n',
                        origins.Select(origin =>
                            $"{origin} => bridge={(origin == "app://local" ? "limited" : "disabled")}")));
                }),
            new("输出平台引擎", "显示当前平台应验证的 WebView 引擎和关键差异。",
                _ => Task.FromResult(DeviceInfo.Current.Platform == DevicePlatform.Android
                    ? "Android WebView / Chromium component; test keyboard, file chooser, back and WebView version"
                    : $"{DeviceInfo.Current.Platform}: verify platform engine matrix")),
            new("生成 Hybrid ADR", "形成可审查的选型结论与退出条件。",
                _ => Task.FromResult(
                    "Decision: HybridWebView for packaged local knowledge UI.\n" +
                    "Native owns navigation and sensitive capabilities.\n" +
                    "Exit: full Native AOT requirement, unacceptable startup/memory, or inaccessible Web UX.")),
        ];

        return new LearningStage(
            21,
            "Blazor Hybrid 与 HybridWebView",
            "第8部分-阶段21-Blazor-Hybrid与HybridWebView.md",
            topics,
            labs);
    }
}
