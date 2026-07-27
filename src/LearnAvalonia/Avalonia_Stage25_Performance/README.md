# Avalonia_Stage25_Performance

对应文档：`ArchitectureDesign/AvaloniaStudy/第10部分-阶段25-性能与诊断.md`

Stage 25 · 性能与诊断 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 11 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage25_Performance -c Debug
```

## 知识点（11）

- §0 导读
- §1 启动：冷/热启动
- §2 UI 线程健康
- §3 布局次数与树规模
- §4 绑定与分配
- §5 GC 与内存
- §6 图片、字体、文本
- §7 虚拟化
- §8 Overdraw、Compositor、帧率
- §9 GPU vs 软件渲染
- §11 工具箱

详见 `Docs/Stage25.md`。