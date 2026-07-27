# Avalonia_Stage09_DataBindingMVVM

对应文档：`ArchitectureDesign/AvaloniaStudy/第4部分-阶段9-数据绑定与MVVM.md`

Stage 09 · 数据绑定与 MVVM —— Avalonia 12 DataContext、绑定模式、compiled vs reflection、INPC、ObservableCollection、命令与异步命令、校验、VM 生命周期、DI、messaging、导航/对话框抽象、Behaviors、MVVM 框架选型。本应用以 nav-shell 方式承载 16 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage09_DataBindingMVVM -c Debug
```

## 知识点（16）

- §A1-A2 DataContext 与绑定模式
- §A3-A4 路径与 Fallback/TargetNull
- §A5-A6 Converter 与 MultiBinding·诊断
- §B1-B2 Compiled vs Reflection
- §B3-B4 代码侧绑定与何时 Reflection
- §C1-C5 INPC·集合·命令
- §D1-D3 校验与 DataAnnotations
- §E1-E2 VM 生命周期与 DI
- §E3 Messaging
- §E4 导航/对话框抽象
- §F1-F2 Behaviors
- §G1-G4 MVVM 框架选型表
- §H Avalonia 12 绑定变化
- §I 串联清单
- §J1-J3 平台差·过时·反模式
- §K1-K4 最小实验·验收·链接

详见 `Docs/Stage09.md`。
