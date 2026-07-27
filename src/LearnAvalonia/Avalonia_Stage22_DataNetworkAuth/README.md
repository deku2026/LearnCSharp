# Avalonia_Stage22_DataNetworkAuth

对应文档：`ArchitectureDesign/AvaloniaStudy/第9部分-阶段22-数据存储网络与认证.md`

Stage 22 · 数据存储、网络与认证 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 19 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage22_DataNetworkAuth -c Debug
```

## 知识点（19）

- §0 导读：数据面全图
- §1 HttpClient 与 IHttpClientFactory
- §2 STJ 与 Source Gen
- §3 REST · gRPC · WebSocket
- §4 弹性、重试、取消
- §5 SQLite 与 EF Core
- §6 Preferences
- §7 安全存储 ISecureStore
- §8 同步与冲突
- §9 OAuth2/OIDC/PKCE
- §10 WebView 登录与系统浏览器
- §11 Token 存储与内存模型
- §12 平台差异总表
- §13 Avalonia 12 相关
- §14 过时内容
- §15 实验边界
- §16 最小实验 A-H
- §17 验收标准
- §18 源码阅读 + 登录序列图

详见 `Docs/Stage22.md`。