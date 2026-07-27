# Avalonia_Stage11_PagesNavigation

对应文档：`ArchitectureDesign/AvaloniaStudy/第5部分-阶段11-Pages与导航架构.md`

Stage 11 · Pages 与导航架构 —— Avalonia 12 Page 族（Content/Navigation/Tabbed/Drawer/Carousel/MultiPage）、INavigation 异步 API、INavigationService 抽象、IViewFactory（AOT 友好显式映射）、导航事件、TabbedPage、DrawerPage、CarouselPage+PipsPager、CommandBar、Transitions、Safe area/EffectiveBarHeight、端到端场景、ReactiveUI 路由选学、桌面 document/tab/multi-window。本应用以 nav-shell 方式承载 18 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage11_PagesNavigation -c Debug
```

## 知识点（18）

- §1 为什么需要 Pages
- §2.1 页面类型一览
- §2.2 ContentPage 最小页
- §2.3 NavigationPage 与 INavigation
- §2.4 导航事件
- §2.5 TabbedPage
- §2.6 DrawerPage
- §2.7 CarouselPage 与 PipsPager
- §2.8 CommandBar
- §2.9 Transitions
- §2.10 Safe area 与 EffectiveBarHeight
- §2.11 完整 NavigationPage 端到端场景
- §2.12 ReactiveUI 路由（选学）
- §2.13 桌面 document / tab / multi-window
- §3 跨平台差异
- §4 Avalonia 12 / 12.1 变化
- §5 过时与禁止
- §6 最小实验清单

详见 `Docs/Stage11.md`。
