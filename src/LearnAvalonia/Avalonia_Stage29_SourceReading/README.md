# Avalonia_Stage29_SourceReading

对应文档：`ArchitectureDesign/AvaloniaStudy/第11部分-阶段29-必读子系统源码精读.md`

Stage 29 · 必读子系统源码精读 —— Avalonia 12.1 反射探查。本应用以 nav-shell 方式承载 18 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo（用 `System.Type` 反射列出 Avalonia 子系统的类型/方法/属性/接口），底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage29_SourceReading -c Debug
```

## 知识点（18）

- §1 AppBuilder
- §2 Application/Lifetime
- §3 AvaloniaObject
- §4 AvaloniaProperty
- §5 Property Store / 优先级
- §6 Logical Tree
- §7 Visual Tree
- §8 Routed Events
- §9 Layout Manager
- §10 Binding Engine / Compiled
- §11 Selector/Style
- §12 ControlTheme
- §13 TopLevel/Window
- §14 Dispatcher
- §15 Input/Focus
- §16 Renderer/Compositor/Skia
- §17 Platform Storage
- §18 Accessibility Peers

详见 `Docs/Stage29.md`。