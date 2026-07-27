# Avalonia_Stage28_WpfMigration

对应文档：`ArchitectureDesign/AvaloniaStudy/第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md`

Stage 28 · WPF/WinUI/WinForms 迁移与互操作 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 8 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage28_WpfMigration -c Debug
```

## 知识点（8）

- §0 导读
- §1 概念映射总表
- §2 XAML/样式/模板/绑定/路由事件差异
- §3 Dispatcher 与线程
- §4 平台服务差异
- §5 增量迁移战术
- §6 💼 Avalonia XPF 边界
- §7 原生宿主与互操作

详见 `Docs/Stage28.md`。
