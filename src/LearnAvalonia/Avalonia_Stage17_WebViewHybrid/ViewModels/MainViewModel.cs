using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 版本 pin + 包清单", "第7部分-阶段17-WebView与Hybrid-UI.md §1", "CPM pin WebView 12.0.1", () => new VersionPinDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 嵌入远程页（XAML）", "第7部分-阶段17-WebView与Hybrid-UI.md §2.1", "NativeWebView Source=URL", () => new EmbedRemoteDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 独立对话框窗口", "第7部分-阶段17-WebView与Hybrid-UI.md §2.2", "NativeWebDialog.Show()", () => new DialogWindowDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 前进/后退/刷新", "第7部分-阶段17-WebView与Hybrid-UI.md §2.3", "GoBack/GoForward/Reload", () => new NavigationDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§2.4 本地资源 vs 远程", "第7部分-阶段17-WebView与Hybrid-UI.md §2.4", "avares:// vs https://", () => new LocalVsRemoteDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§2.5 Environment 与初始化", "第7部分-阶段17-WebView与Hybrid-UI.md §2.5", "WebViewEnvironment 平台配置", () => new EnvironmentDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§3 原生引擎细节", "第7部分-阶段17-WebView与Hybrid-UI.md §3", "Win WebView2 / macOS WKWebView", () => new NativeEngineDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§4 JS ↔ C# 互操作", "第7部分-阶段17-WebView与Hybrid-UI.md §4", "InvokeScript + WebMessageReceived", () => new JsInteropDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§5 资源拦截与虚拟主机", "第7部分-阶段17-WebView与Hybrid-UI.md §5", "WebResourceRequested 拦截", () => new ResourceInterceptDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§6 OAuth + WebAuthBroker", "第7部分-阶段17-WebView与Hybrid-UI.md §6", "WebAuthenticationBroker.AuthenticateAsync", () => new OAuthDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§7 Cookie / 存储 / 隐私", "第7部分-阶段17-WebView与Hybrid-UI.md §7", "Cookie + localStorage 隔离", () => new CookieStorageDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§8 CSP / XSS / 安全", "第7部分-阶段17-WebView与Hybrid-UI.md §8", "CSP/转义/白名单", () => new SecurityDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§9 选型对比 / 工具表", "第7部分-阶段17-WebView与Hybrid-UI.md §9", "WebView vs 浏览器 vs Blazor", () => new SelectionMatrixDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§10 WASM 硬边界", "第7部分-阶段17-WebView与Hybrid-UI.md §10", "Browser 端无 WebView", () => new WasmBoundaryDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§11 平台差异细节", "第7部分-阶段17-WebView与Hybrid-UI.md §11", "三平台 webview 后端", () => new PlatformDiffDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§12 Avalonia 12 变化", "第7部分-阶段17-WebView与Hybrid-UI.md §12", "WebView 12 单独包", () => new Avalonia12ChangesDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§15 最小实验 A-G", "第7部分-阶段17-WebView与Hybrid-UI.md §15", "完成最小实验 A-G", () => new MinExperimentsDemo()) { Order = 17 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
