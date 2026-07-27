# Avalonia_Stage14_CustomControls

对应文档：`ArchitectureDesign/AvaloniaStudy/第6部分-阶段14-自定义控件.md`

Stage 14 · 自定义控件 —— Avalonia 12 自定义控件选型（UserControl/TemplatedControl/ContentControl/Panel/绘制 Control）、UserControl 快路径、自定义 Panel（MeasureOverride/ArrangeOverride）、自定义绘制 Control（AffectsRender/AutomationPeer/HitTestCore）、TemplatedControl 结构（OnApplyTemplate/ControlTheme）、TemplateBinding vs Binding、Named parts（PART_ 前缀/卸载旧事件）、伪类（PseudoClasses.Set/:pointerover/:pressed）、属性三类（Styled/Direct/Attached + Affects*）、路由事件（RoutedEvent.Register）、命令（Command 属性）、AutomationPeer、组件库打包（Themes/Fluent/Index.axaml）、Design-time、AOT 友好、RatingControl 端到端、组件库工程化。本应用以 nav-shell 方式承载 20 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage14_CustomControls -c Debug
```

## 知识点（20）

- §1 三类（+Panel/绘制）对照
- §2.1 UserControl 快路径
- §2.2 自定义 Panel
- §2.3 自定义绘制 Control
- §2.4 TemplatedControl 结构
- §2.5 TemplateBinding vs Binding
- §2.6 Named parts 约定
- §2.7 伪类（Pseudoclasses）
- §2.8 属性：Styled / Direct / Attached
- §2.9 路由事件
- §2.10 命令
- §2.11 AutomationPeer
- §2.12 默认主题与组件库打包
- §2.13 Design-time
- §2.14 AOT 友好
- §3 完整端到端：RatingControl
- §4 跨平台差异
- §5 Avalonia 12 / 12.1 变化
- §6 过时与禁止
- §11 组件库工程化

详见 `Docs/Stage14.md`。
