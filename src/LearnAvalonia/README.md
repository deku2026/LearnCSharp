# LearnAvalonia · Avalonia 12 桌面主线学习模块

本模块填充 `src/LearnAvalonia/` 下 35 个项目中的 32 个为可运行 Avalonia 12.1 桌面演示应用，每个知识点一个可见 demo 页，与 `ArchitectureDesign/AvaloniaStudy/` 的阶段/Capstone 文档一一对应、全面覆盖。

## 跳过项

**本仓库只做桌面主线**（Windows / macOS / Linux X11）。以下 3 个项目保留骨架，仅补 README 注明跳过原因：

- Stage18 Mobile — 需 `net10.0-android/ios` workload
- Stage19 WebAssembly — 需 `Avalonia.Browser` + `net10.0-browser` TFM
- Stage20 EmbeddedLinux — 需 `Avalonia.LinuxFramebuffer` + 目标硬件

Capstone3（原「全平台」）改造为「桌面三平台（Windows/macOS/Linux）」版。

## 项目状态与知识点数

| 项目 | 状态 | demo 数 |
|---|---|---|
| Stage01_AppBuilder | filled | 15 |
| Stage02_Dispatcher | filled | 15 |
| Stage03_AvaloniaXaml | filled | 16 |
| Stage04_PropertySystem | filled | 18 |
| Stage05_LogicalVisualTree | filled | 18 |
| Stage06_LayoutSystem | filled | 17 |
| Stage07_ControlSystem | filled | 16 |
| Stage08_StylesThemes | filled | 14 |
| Stage09_DataBindingMVVM | filled | 16 |
| Capstone1_DesktopFoundation | filled | 10 |
| Stage10_WindowsDialogs | filled | 19 |
| Stage11_PagesNavigation | filled | 18 |
| Stage12_InputFocusGestures | filled | 18 |
| Stage13_GraphicsAnimation | filled | 21 |
| Stage14_CustomControls | filled | 20 |
| Stage15_Virtualization | filled | 17 |
| Stage16_PlatformAbstraction | filled | 18 |
| Stage17_WebViewHybrid | filled | 17 |
| Stage18_Mobile | skipped | 0 |
| Stage19_WebAssembly | skipped | 0 |
| Stage20_EmbeddedLinux | skipped | 0 |
| Stage21_AppArchitecture | filled | 20 |
| Stage22_DataNetworkAuth | filled | 19 |
| Capstone2_ComplexDesktop | filled | 15 |
| Stage23_AccessibilityI18n | filled | 12 |
| Stage24_TestingSystem | filled | 14 |
| Stage25_Performance | filled | 11 |
| Stage26_TrimmingAOT | filled | 9 |
| Stage27_PackagingCI | filled | 10 |
| Stage28_WpfMigration | filled | 8 |
| Capstone3_CrossPlatform | filled (desktop three-platform redesign) | 10 |
| Stage29_SourceReading | filled | 18 |
| Stage30_PlatformBackend | filled | 11 |
| Stage31_RenderingExpert | filled | 17 |
| Capstone4_CustomControlAOT | filled | 15 |

**合计：32 个项目填实质内容，492 个知识点 demo 页**。

## 统一结构（每个 filled 项目）

```
Avalonia_StageNN_<Topic>/
├── Program.cs / App.axaml / App.axaml.cs / MainWindow.axaml(.cs)   # 导航壳
├── ViewModels/{TopicItem.cs, MainViewModel.cs}                     # CommunityToolkit.Mvvm
├── Pages/{<Slug>Demo.axaml(.cs), DemoFactoryConverter.cs}         # 每个知识点一个 demo
├── Docs/StageNN.md                                                 # 知识点索引
└── README.md                                                        # 项目说明
```

- 导航壳：左侧 `ListBox` 列知识点 → 右侧 `ContentControl` 展示对应 demo
- 每个 demo `.axaml.cs` 顶部含 `Doc:` / `Title:` / `Stage:` / `Accept:` / `Notes:` 头注释
- `Avalonia_Stage24_TestingSystem.Tests` 测试项目（Headless.XUnit + xUnit v3）含 19 个测试

## 运行

```pwsh
# 编译全部（含 LearnAvalonia，含 Stage24.Tests）
dotnet build LearnCSharp.CI.slnx -c Release

# 运行 Stage24 测试
dotnet test src/LearnAvalonia/Avalonia_Stage24_TestingSystem.Tests --no-build

# 跑某个项目的演示
dotnet run --project src/LearnAvalonia/Avalonia_Stage01_AppBuilder -c Debug
```

## 版本基线

- Avalonia **12.1.0** · .NET 10 · C# 14 · `CommunityToolkit.Mvvm 8.4.2`
- 包版本统一在 `Directory.Packages.props`（CPM）
- compiled bindings 默认开；`TrayIcon` 用 `NativeMenu`；剪贴板/DnD 异步 `IAsyncDataTransfer`；旧 FileDialog 禁用 → `IStorageProvider`
- `SystemDecorations`→`WindowDecorations`（12）；`WindowState` 为 DirectProperty（不能靠 style 设）

## 文档引用

- 大纲：`ArchitectureDesign/AvaloniaStudy/Avalonia-12-全平台应用完整学习路线图.md`
- 阶段/Capstone 文档：`ArchitectureDesign/AvaloniaStudy/第N部分-阶段M-<名>.md`（35 份）
- 仓内 README：`README.md`（LearnCSharp 整体说明）

## 设计与实施

- 设计：`docs/superpowers/specs/2026-07-27-learnavalonia-fill-design.md`
- 实施计划：`docs/superpowers/plans/2026-07-27-learnavalonia-fill.md`