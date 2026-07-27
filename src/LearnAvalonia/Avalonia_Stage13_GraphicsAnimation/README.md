# Avalonia_Stage13_GraphicsAnimation

对应文档：`ArchitectureDesign/AvaloniaStudy/第5部分-阶段13-图形动画与效果.md`

Stage 13 · 图形、动画与效果 —— Avalonia 12 Skia 渲染栈：Brush 族（Solid/Linear/Radial/Conic/Image/Visual）、Geometry/Path/Shape、DrawingContext 即时模式（AffectsRender/Render 纪律）、位图（Bitmap/RenderTargetBitmap）、SVG 生态选型、Transforms（RenderTransform/LayoutTransform）、Clip/Opacity/OpacityMask、DropShadowEffect、Keyframe 动画（Style.Animations）、Transitions（TransformOperations/Brush/Double）、Easing、PlaybackBehavior 不可见停动画、页面过渡、HarfBuzz shaping、可动画仪表组合、性能配方。本应用以 nav-shell 方式承载 21 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage13_GraphicsAnimation -c Debug
```

## 知识点（21）

- §1 为什么图形与动画单独成章
- §2.1 Brush 族
- §2.2 Geometry / Path / Shape
- §2.3 DrawingContext（即时模式）
- §2.4 位图
- §2.5 SVG 生态（选学前查）
- §2.6 Transforms
- §2.7 Clip / Opacity / OpacityMask
- §2.8 Effects / 阴影
- §2.9 动画：Keyframe
- §2.10 Transitions
- §2.11 Easing
- §2.12 PlaybackBehavior 与不可见停动画
- §2.13 页面过渡
- §2.14 HarfBuzz 文本 shaping
- §2.15 组合示例：可动画仪表
- §3 跨平台差异
- §4 Avalonia 12 / 12.1 变化
- §5 过时与禁止
- §6 最小实验清单
- §10 性能配方

详见 `Docs/Stage13.md`。
