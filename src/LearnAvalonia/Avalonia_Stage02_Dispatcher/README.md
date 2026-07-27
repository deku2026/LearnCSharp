# Avalonia_Stage02_Dispatcher

对应文档：`ArchitectureDesign/AvaloniaStudy/第2部分-阶段2-Dispatcher线程与UI亲和性.md`

Stage 02 · Dispatcher 线程与 UI 亲和性 —— Avalonia 12.1 桌面主线。本应用以 nav-shell 方式承载 15 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage02_Dispatcher -c Debug
```

## 知识点（15）

- §1 UI 线程纪律
- §2 Dispatcher 家族 API
- §3 Post/InvokeAsync/优先级
- §4 Yield/Resume/SyncContext
- §5 多 Dispatcher 单 UI 线程
- §6 后台→UI 正误写法
- §7 剪贴板死锁禁区
- §8 async/await 死锁场景
- §9 产品级异步 UI 模式
- §10 跨平台差异表
- §11 Avalonia 12 最新变化
- §12 过时与禁止
- §13 最小实验 A-F
- §14 源码阅读路径
- §15 验收标准

详见 `Docs/Stage02.md`。
