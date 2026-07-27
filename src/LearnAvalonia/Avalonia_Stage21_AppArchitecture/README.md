# Avalonia_Stage21_AppArchitecture

对应文档：`ArchitectureDesign/AvaloniaStudy/第9部分-阶段21-应用架构与工程结构.md`

Stage 21 · 应用架构与工程结构 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 20 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage21_AppArchitecture -c Debug
```

## 知识点（20）

- §0 导读：从会写到可演进
- §1 UI 架构选型 MVVM/Rx/MVU
- §2 分层 vs 垂直切片
- §3 模块化与贡献点
- §4 插件与 AssemblyLoadContext
- §5 DI 与 Hosting 组合根
- §6 配置与日志
- §7 导航与对话框服务
- §8 状态/Undo/Redo/文档模型
- §9 Autosave 与崩溃恢复
- §10 多窗口状态
- §11 遥测边界
- §12 桌面生产补充
- §13 授权许可边界
- §14 平台差异（架构层）
- §15 Avalonia 12 相关
- §16 过时内容红线
- §17 实验边界
- §18 最小实验 A-H
- §19 验收标准

详见 `Docs/Stage21.md`。