# Avalonia_Capstone1_DesktopFoundation

对应文档：`ArchitectureDesign/AvaloniaStudy/第4部分-Capstone1-生产级桌面基础应用.md`

Capstone 1 · 生产级桌面基础应用 —— 将阶段 6-9（布局、控件、样式/ControlTheme、绑定/MVVM）收束为一款可在 Windows/macOS/Linux 运行的生产级桌面基础应用规格书。本应用以 nav-shell 方式承载 10 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Capstone1_DesktopFoundation -c Debug
```

## 知识点（10）

- §0 规格导读与目标
- §1 功能清单 P0/P1/P2
- §2 架构建议与组合根
- §3 平台要求与差异
- §4 必须项详述（XAML/DI/Theme/导航/对话框/Storage）
- §5 UI 信息架构与线框
- §6 分周实施计划
- §7 验收勾选清单
- §8 反模式清单
- §9-18 目录骨架·测试·交付·接口

详见 `Docs/Capstone1.md`。
