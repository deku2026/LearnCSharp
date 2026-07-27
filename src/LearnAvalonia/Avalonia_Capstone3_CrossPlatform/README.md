# Avalonia_Capstone3_CrossPlatform

对应文档：`ArchitectureDesign/AvaloniaStudy/第8部分-Capstone3-全平台Avalonia应用.md`

Capstone 3 · 跨平台 Avalonia 应用 —— 已 **adapted to desktop three-platform (Win/macOS/Linux)**；移动/Browser 不实现（见 roadmap）。覆盖：解决方案结构（共享 Core + Desktop + Headless.Tests）、共享层（Views/VM/Service contracts 共享 vs 平台特化）、Host 角色（IClassicDesktopStyleApplicationLifetime 入口与生命周期）、Shell 模式（Menu+Content / SplitView+Content；移动 NavigationPage 不实现）、路由策略（ContentControl + ViewBinder；移动 ShellContent 不实现）、存储抽象（IFileStorage 接口 + 三平台 LocalApplicationData 路径）、平台差异注册（DI 容器选型 + #if 平台服务）、日志（三平台 sink 与调试脚本）、Headless 测试（Avalonia.Headless + xUnit）、`#if WINDOWS/MACOS/LINUX` 编译开关例子。本应用以 nav-shell 方式承载 10 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

**adapted to desktop three-platform (Win/macOS/Linux); mobile/browser not implemented**。相关章节（§3.2 桌面、§3.3 移动、§3.4 Browser、§4 Browser 强制清单、§9 应力测试、§10.3 平台 smoke、§17 源码级约束）以「see roadmap, not implemented」在 demo Notes 中标注。

csproj 额外引用：`CommunityToolkit.Mvvm`（CPM 统一管版本）。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Capstone3_CrossPlatform -c Debug
```

## 知识点（10）

- §1 解决方案结构
- §2.1 共享什么
- §2.2 Host 做什么
- §2.3 Shell 模式
- §3.1 路由策略（桌面）
- §5 存储抽象
- §6 平台差异注册（DI）
- §8 日志
- §10.2 Headless 测试
- §11 #if 编译开关

详见 `Docs/Capstone3.md`。
