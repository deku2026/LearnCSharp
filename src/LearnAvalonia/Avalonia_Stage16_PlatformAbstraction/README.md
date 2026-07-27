# Avalonia_Stage16_PlatformAbstraction

对应文档：`ArchitectureDesign/AvaloniaStudy/第7部分-阶段16-统一平台抽象.md`

Stage 16 · 统一平台抽象 —— TopLevel 统一抽象（三模式 / 安全访问器）、StorageProvider（OpenFilePicker/OpenFolderPicker/SaveFilePicker，三平台差异与沙盒书签）、Clipboard（12 起 IAsyncDataTransfer，全异步）、Launcher（URI / 外部程序）、Screens（12 起 Screen abstract）、PlatformSettings / HotkeyConfiguration、PlatformFeedback 触觉反馈、拖放（应用内 vs 应用外 DnD）、原生菜单与通知（NativeMenu vs Menu；TrayIcon 必须 NativeMenu）、接口能力清单、平台差异总表、Avalonia 12/12.1 变化、最小实验。本应用以 nav-shell 方式承载 18 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

csproj 额外引用：`CommunityToolkit.Mvvm`（CPM 统一管版本）。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage16_PlatformAbstraction -c Debug
```

## 知识点（18）

- §1 TopLevel 统一抽象
- §2.1 StorageProvider 职责
- §2.2 最小文件选择器
- §2.3 沙盒/书签/安全
- §2.4 文件夹选择器
- §2.5 平台差异（Storage）
- §3 Clipboard (IAsyncDataTransfer)
- §4 Launcher（URI/外部）
- §5 Screens（Screen abstract）
- §6 PlatformSettings / Hotkey
- §7 PlatformFeedback（触觉）
- §8 拖放（DnD）
- §9 原生菜单/通知
- §10 WebView 在本阶段位置
- §11 接口能力清单
- §12 平台差异总表
- §13 Avalonia 12/12.1 变化
- §16 最小实验 A-E

详见 `Docs/Stage16.md`。