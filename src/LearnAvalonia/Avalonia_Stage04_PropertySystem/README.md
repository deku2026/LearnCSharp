# Avalonia_Stage04_PropertySystem

对应文档：`ArchitectureDesign/AvaloniaStudy/第3部分-阶段4-属性系统.md`

Stage 04 · 属性系统 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 18 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage04_PropertySystem -c Debug
```

## 知识点（18）

- §1 为什么需要属性系统
- §2 三种属性 Styled/Direct/Attached
- §3 注册与 CLR 包装
- §4 Get/Set/SetCurrent/Clear/Bind
- §5 值优先级
- §6 Metadata 默认/校验/强制/继承
- §7 变更通知与响应式
- §8 完整自定义控件
- §9 对比 CLR/WPF/MAUI/Avalonia
- §10 WindowState 变 Direct
- §11 数据验证与错误展示
- §12 常见陷阱
- §13 跨平台差异表
- §14 Avalonia 12 最新变化
- §15 过时与禁止
- §16 最小实验 A-F
- §17 源码阅读路径
- §18 验收标准

详见 `Docs/Stage04.md`。
