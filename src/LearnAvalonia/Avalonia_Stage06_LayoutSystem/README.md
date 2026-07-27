# Avalonia_Stage06_LayoutSystem

对应文档：`ArchitectureDesign/AvaloniaStudy/第4部分-阶段6-布局系统.md`

Stage 06 · 布局系统 —— Avalonia 12.1 Measure/Arrange 两阶段管线、Panel 全家桶、虚拟化、自定义 Panel、DPI 与平台差。本应用以 nav-shell 方式承载 17 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage06_LayoutSystem -c Debug
```

## 知识点（17）

- §A1 两阶段管线
- §A1.1 尺寸三元组
- §A1.2 无限可用空间
- §B1 Invalidation 规则
- §B2-B3 Layout Rounding 与 DPI
- §C1-C2 Alignment 与 Margin/Padding
- §C3-C4 Width/Min/Max 与 Clip
- §D1 Panel 选型总表
- §D2 StackPanel
- §D3 Grid
- §D4 WrapPanel
- §D5 DockPanel
- §D6 Canvas
- §D7-D8 RelativePanel 与 UniformGrid
- §E1-E2 Viewbox 与 ScrollViewer
- §F 自定义 Panel
- §G-J 平台差·12变化·专家·验收

详见 `Docs/Stage06.md`。
