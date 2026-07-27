# Avalonia_Stage15_Virtualization

对应文档：`ArchitectureDesign/AvaloniaStudy/第6部分-阶段15-虚拟化与大数据UI.md`

Stage 15 · 虚拟化与大数据 UI —— 虚拟化原理（容器生成/recycle 复用/三启动条件）、TableView（12.1 核心包，只读表首选）、DataGrid（⚠️ deprecated，仅演示）、TreeDataGrid（💼 Pro，仅说明不实现）、ItemsRepeater（retired，演示用）、选择模式、数据分组、滚动 FPS 与内存分析（概念性）、5 种反模式、与 MVVM 协作、跨平台差异、12/12.1 变化、过时/禁止 API、最小实验、验收清单。本应用以 nav-shell 方式承载 17 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

csproj 额外引用：`Avalonia.Controls.DataGrid`、`Avalonia.Controls.ItemsRepeater`（CPM 统一管版本）。App.axaml 已 `StyleInclude` DataGrid 主题。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage15_Virtualization -c Debug
```

## 知识点（17）

- §1 为什么虚拟化是分水岭
- §2.1 虚拟化原理与容器
- §2.2 容器与要满足的条件
- §2.3 TableView（12.1 新）
- §2.4 DataGrid（deprecated）
- §2.5 TreeDataGrid（Pro）
- §2.6 ItemsRepeater
- §2.7 选择模式
- §2.8 数据分组
- §2.9 滚动 FPS 与内存分析
- §2.10 反模式：虚拟化失败
- §2.11 与 MVVM
- §3 跨平台差异
- §4 Avalonia 12/12.1 变化
- §5 过时/禁止
- §6 最小实验 A-E
- §7 验收清单

详见 `Docs/Stage15.md`。