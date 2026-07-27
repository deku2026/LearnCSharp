# Avalonia_Stage10_WindowsDialogs

对应文档：`ArchitectureDesign/AvaloniaStudy/第5部分-阶段10-桌面窗口对话框与系统集成.md`

Stage 10 · 桌面窗口、对话框与系统集成 —— Avalonia 12 Desktop Lifetime、多窗口、模态/Owner、WindowDecorations/WindowDrawnDecorations、TrayIcon+NativeMenu、NativeMenuBar、窗口状态持久化、多屏 DPI、关闭拦截、单实例 IPC、文件关联、StorageProvider、IDialogService 抽象、任务栏图标。本应用以 nav-shell 方式承载 19 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage10_WindowsDialogs -c Debug
```

## 知识点（19）

- §1 为什么桌面窗口是独立阶段
- §2.1 Lifetime 与多窗口
- §2.2 位置、尺寸、状态
- §2.3 WindowDecorations 与自绘标题栏
- §2.4 透明、亚克力与 Topmost
- §2.5 TrayIcon 必须配 NativeMenu
- §2.6 NativeMenuBar 与 macOS Dock
- §2.7 窗口状态持久化
- §2.8 多显示器、DPI、拖拽区域
- §2.9 关闭拦截与未保存
- §2.10 单实例与 IPC 激活
- §2.11 文件关联与 URI
- §2.12 StorageProvider 取代旧 FileDialog
- §2.13 对话框服务抽象
- §2.14 任务栏与窗口图标
- §3 跨平台差异
- §4 Avalonia 12 / 12.1 变化
- §5 过时与禁止
- §6 最小实验清单

详见 `Docs/Stage10.md`。
