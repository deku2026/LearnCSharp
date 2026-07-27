# Avalonia_Capstone2_ComplexDesktop

对应文档：`ArchitectureDesign/AvaloniaStudy/第9部分-Capstone2-复杂桌面生产应用.md`

Capstone 2 · 复杂桌面生产应用 —— Avalonia 12.1 三桌面主线。本应用以 nav-shell 方式承载 15 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Capstone2_ComplexDesktop -c Debug
```

## 知识点（15）

- §0 导读：复杂桌面生产应用
- §1 架构落位（强制）
- §2 大数据虚拟化
- §3 Document / Tab 模型
- §4 Undo/Redo
- §5 Autosave 与崩溃恢复
- §6 SQLite 本地索引
- §7 远程 API 与离线
- §8 插件边界
- §9 Tray · NativeMenu · DnD
- §10 全局激活 · 单实例 · CLI
- §11 更新服务
- §12 UI 自动化与测试
- §13 性能剖析
- §14 三桌面平台能力矩阵

详见 `Docs/Capstone2.md`。
