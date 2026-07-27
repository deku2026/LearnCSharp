# Avalonia_Stage27_PackagingCI

对应文档：`ArchitectureDesign/AvaloniaStudy/第10部分-阶段27-打包部署更新与CI.md`

Stage 27 · 打包、部署、更新与 CI —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 10 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage27_PackagingCI -c Debug
```

## 知识点（10）

- §0 导读
- §1 Windows
- §2 macOS
- §3 Linux 桌面
- §4 Android
- §5 iOS / iPadOS
- §6 Browser（WASM）
- §8 CI 矩阵
- §9 SBOM 与依赖审计
- §10 崩溃上报与可观测

详见 `Docs/Stage27.md`。