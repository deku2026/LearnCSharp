# Avalonia_Stage23_AccessibilityI18n

对应文档：`ArchitectureDesign/AvaloniaStudy/第10部分-阶段23-无障碍国际化与文本.md`

Stage 23 · 无障碍、国际化与文本 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 12 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage23_AccessibilityI18n -c Debug
```

## 知识点（12）

- §0 导读
- §1 AutomationPeer 与 a11y 树
- §2 键盘导航与焦点
- §3 平台读屏桥接
- §4 语义、高对比、减弱动效、动态字号
- §5 RTL、BiDi 与布局镜像
- §6 Unicode、Grapheme、Shaping、字体回退、Emoji、IME
- §7 国际化：RESX、Culture、复数、运行时切语言
- §8 平台差异汇总
- §9 Avalonia 12/12.1 相关变化
- §10 过时与禁止
- §12 验收清单

详见 `Docs/Stage23.md`。