# Avalonia_Stage24_TestingSystem

对应文档：`ArchitectureDesign/AvaloniaStudy/第10部分-阶段24-测试体系.md`

Stage 24 · 测试体系 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 14 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

配套测试项目 `Avalonia_Stage24_TestingSystem.Tests` 落地 Headless 测试样例（Binding/Layout/Control/Focus/Theme/VisualRegression/Input）。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage24_TestingSystem -c Debug
```

## 知识点（14）

- §0 导读
- §1 分层职责与命名
- §2 Unit：ViewModel 与服务
- §3 Headless 测试完整示例
- §4 Visual regression
- §5 Appium 与真窗口
- §6 专题测试清单
- §7 CI 集成
- §8 平台差异
- §9 Avalonia 12/12.1 变化
- §10 过时
- §12 完整样例仓库清单
- §13 验收
- §17 一页总结

详见 `Docs/Stage24.md`。
