# Avalonia_Stage01_AppBuilder

对应文档：`ArchitectureDesign/AvaloniaStudy/第2部分-阶段1-AppBuilder与应用启动.md`

Stage 01 · AppBuilder & 应用启动 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 15 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage01_AppBuilder -c Debug
```

## 知识点（15）

- §1 启动链总览
- §2 AppBuilder
- §3 完整桌面示例
- §4 IApplicationLifetime
- §5 ShutdownMode
- §6 手动 lifetime
- §7 Android 入口
- §8 iOS scene
- §9 其他平台入口
- §10 TopLevel/VisualTree
- §11 跨平台差异表
- §12 Avalonia 12 最新变化
- §13 过时与禁止
- §14 最小实验 A-E
- §15 验收标准

详见 `Docs/Stage01.md`。
