# Avalonia_Capstone4_CustomControlAOT

对应文档：`ArchitectureDesign/AvaloniaStudy/第10部分-Capstone4-自定义控件渲染与Native-AOT.md`

Capstone 4 · 自定义控件渲染与 Native AOT —— Avalonia 12.1。本应用以 nav-shell 方式承载 15 个知识点 demo，并实现一个真实的 `GaugeControl` TemplatedControl（StyledProperty + DirectProperty + 路由事件 + ControlTheme + 自定义 Render + AutomationPeer + 动画），含自定义 `RadialPanel` 布局、Benchmark 概念、Trim/AOT 注解（`<IsAotCompatible>true</IsAotCompatible>`）。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Capstone4_CustomControlAOT -c Debug
```

## 知识点（15）

- §1 架构与示例题
- §2 GaugeControl 控件运行
- §3 StyledProperty + DirectProperty
- §4 路由事件 ValueChanged
- §5 ControlTheme
- §6 RadialPanel 自定义布局
- §7 自定义 Render (DrawingContext)
- §8 动画（值变更过渡）
- §9 无障碍 AutomationPeer
- §10 本地化
- §11 测试结构
- §12 Benchmark 概念
- §13 Trim / Native AOT
- §14 源码精读一面
- §15 平台表 + 12.1 变更

详见 `Docs/Capstone4.md`。