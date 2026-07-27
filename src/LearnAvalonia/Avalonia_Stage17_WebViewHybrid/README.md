# Avalonia_Stage17_WebViewHybrid

对应文档：`ArchitectureDesign/AvaloniaStudy/第7部分-阶段17-WebView与Hybrid-UI.md`

Stage 17 · WebView 与 Hybrid UI —— 版本 pin（`Avalonia.Controls.WebView` 12.0.1，单独包）、NativeWebView（嵌入 XAML，Source/NavigationCompleted）、NativeWebDialog（独立窗口承载 webview）、导航（GoBack/GoForward/Reload）、本地 vs 远程资源（avares:// vs https://）、WebViewEnvironment 平台配置、原生引擎细节（Win WebView2 / macOS WKWebView / Linux WPE WebKit）、JS ↔ C# 双向互操作（InvokeScript + WebMessageReceived）、资源拦截 / 虚拟主机映射、OAuth + WebAuthenticationBroker、Cookie / localStorage 隔离、CSP / XSS / 安全清单、选型对比（WebView vs 浏览器 vs Blazor Hybrid）、WASM 硬边界、平台差异细节、Avalonia 12 变化、最小实验 A-G。本应用以 nav-shell 方式承载 17 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

csproj 额外引用：`CommunityToolkit.Mvvm`、`Avalonia.Controls.WebView`（CPM 统一管版本）。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage17_WebViewHybrid -c Debug
```

## 知识点（17）

- §1 版本 pin + 包清单
- §2.1 嵌入远程页（XAML）
- §2.2 独立对话框窗口
- §2.3 前进/后退/刷新
- §2.4 本地资源 vs 远程
- §2.5 Environment 与初始化
- §3 原生引擎细节
- §4 JS ↔ C# 互操作
- §5 资源拦截与虚拟主机
- §6 OAuth + WebAuthBroker
- §7 Cookie / 存储 / 隐私
- §8 CSP / XSS / 安全
- §9 选型对比 / 工具表
- §10 WASM 硬边界
- §11 平台差异细节
- §12 Avalonia 12 变化
- §15 最小实验 A-G

详见 `Docs/Stage17.md`。