using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：数据面全图", "第9部分-阶段22-数据存储网络与认证.md §0", "口述数据面四大目标", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 HttpClient 与 IHttpClientFactory", "第9部分-阶段22-数据存储网络与认证.md §1", "命名客户端 + 委托处理程序", () => new HttpClientFactoryDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 STJ 与 Source Gen", "第9部分-阶段22-数据存储网络与认证.md §2", "STJ source gen 主 DTO", () => new StjSourceGenDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 REST · gRPC · WebSocket", "第9部分-阶段22-数据存储网络与认证.md §3", "REST 主路径 + 错误映射", () => new RestGrpcWebSocketDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 弹性、重试、取消", "第9部分-阶段22-数据存储网络与认证.md §4", "取消贯穿列表加载", () => new ResilienceCancellationDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 SQLite 与 EF Core", "第9部分-阶段22-数据存储网络与认证.md §5", "SQLite 可读写", () => new SqliteEfCoreDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 Preferences", "第9部分-阶段22-数据存储网络与认证.md §6", "主题进 Preferences", () => new PreferencesDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§7 安全存储 ISecureStore", "第9部分-阶段22-数据存储网络与认证.md §7", "Preferences 与 SecureStore 分离", () => new SecureStoreDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§8 同步与冲突", "第9部分-阶段22-数据存储网络与认证.md §8", "冲突策略可演示", () => new SyncConflictDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§9 OAuth2/OIDC/PKCE", "第9部分-阶段22-数据存储网络与认证.md §9", "PKCE 流文档 + 演示", () => new OAuthPkceDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§10 WebView 登录与系统浏览器", "第9部分-阶段22-数据存储网络与认证.md §10", "三种登录方式对比", () => new WebViewLoginDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§11 Token 存储与内存模型", "第9部分-阶段22-数据存储网络与认证.md §11", "access 内存 + refresh 安全存储", () => new TokenStorageDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§12 平台差异总表", "第9部分-阶段22-数据存储网络与认证.md §12", "桌面/移动/Browser 差异书面", () => new PlatformDiffDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§13 Avalonia 12 相关", "第9部分-阶段22-数据存储网络与认证.md §13", "WebView/StorageProvider/平台抽象", () => new Avalonia12Demo()) { Order = 14 });
        Topics.Add(new TopicItem("§14 过时内容", "第9部分-阶段22-数据存储网络与认证.md §14", "点名过时与替代", () => new DeprecatedDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§15 实验边界", "第9部分-阶段22-数据存储网络与认证.md §15", "区分必做与加分", () => new ExperimentBoundaryDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§16 最小实验 A-H", "第9部分-阶段22-数据存储网络与认证.md §16", "完成 8 个最小实验", () => new MinExperimentsDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§17 验收标准", "第9部分-阶段22-数据存储网络与认证.md §17", "勾选 11 条验收清单", () => new AcceptanceChecklistDemo()) { Order = 18 });
        Topics.Add(new TopicItem("§18 源码阅读 + 登录序列图", "第9部分-阶段22-数据存储网络与认证.md §18", "登录到首次 API 调用序列图", () => new SourceReadingDemo()) { Order = 19 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
