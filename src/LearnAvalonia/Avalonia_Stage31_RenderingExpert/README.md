# Avalonia_Stage31_RenderingExpert

对应文档：`ArchitectureDesign/AvaloniaStudy/第11部分-阶段31-渲染专家档.md`

Stage 31 · 渲染专家档 —— Avalonia 12.1 自绘渲染。本应用以 nav-shell 方式承载 17 个知识点 demo，覆盖 retained-mode、scene/composition、dirty region、hit testing（含非矩形 HitTestCore）、GPU 上下文、Skia 主线、软件渲染、刷新率、UI/渲染线程、GPU 失效恢复、自定义 Render 示例等。Impeller 标记为 🔴 实验跟踪，不在生产路径。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage31_RenderingExpert -c Debug
```

## 知识点（17）

- §1 Retained-mode
- §2 Scene / Composition
- §3 Dirty Region
- §4 Hit Testing（含非矩形 HitTestCore）
- §5 GPU 上下文
- §6 Skia 主线
- §7 Metal
- §8 OpenGL / EGL
- §9 CanvasKit (Browser)
- §10 软件渲染
- §11 刷新率/帧时序
- §12 UI vs 渲染线程
- §13 GPU 失效恢复
- §14 Overdraw / 关键字限制
- §15 自定义 Render 示例
- §16 Impeller (🔴 跟踪)
- §17 Direct2D1 移除 / 平台表

详见 `Docs/Stage31.md`。