# Avalonia_Stage26_TrimmingAOT

对应文档：`ArchitectureDesign/AvaloniaStudy/第10部分-阶段26-Trimming与Native-AOT.md`

Stage 26 · Trimming 与 Native AOT —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 9 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage26_TrimmingAOT -c Debug
```

## 知识点（9）

- §0 导读
- §1 项目配置基础
- §2 Avalonia 特有友好点
- §3 动态 XAML 禁区
- §4 反射、DI、序列化、插件
- §5 ViewLocator 不友好
- §7 警告处理流程
- §8 符号与崩溃诊断
- §9 对比实验设计

详见 `Docs/Stage26.md`。
