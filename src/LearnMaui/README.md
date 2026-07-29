# LearnMaui 文档—代码映射

本目录把 `ArchitectureDesign/MauiStudy` 中与现有代码项目一一对应的 28 个主题实现为独立 .NET MAUI Android 示例。每个项目都包含：

- `StageCatalog.cs`：从入门、进阶、高级到专家级的可搜索知识索引和可运行实验；
- 主题专属的真实实现，例如 DI、Handler、离线仓储、权限、Shell、SourceGen、HybridWebView 或 CollectionView；
- 共享的 `LearningStageView` 交互界面；
- 可由 Visual Studio MSBuild 单独还原和编译的 Android 项目。

| 阶段 | 项目 | 对应文档 |
|---:|---|---|
| 1 | `Maui_Stage01_ArchitectureStartup` | `第2部分-阶段1-架构与启动.md` |
| 2 | `Maui_Stage02_LifecycleWindowState` | `第2部分-阶段2-生命周期窗口与状态.md` |
| 3 | `Maui_Stage03_UIThreadAsync` | `第2部分-阶段3-UI线程与异步纪律.md` |
| 4 | `Maui_Stage04_XamlMarkup` | `第3部分-阶段4-XAML-Markup-架构选型.md` |
| 5 | `Maui_Stage05_ControlsLayout` | `第3部分-阶段5-控件布局自适应资源.md` |
| 6 | `Maui_Stage06_BindingMVVM` | `第3部分-阶段6-绑定状态MVVM.md` |
| 7 | `Maui_Stage07_NavigationShell` | `第3部分-阶段7-导航与Shell.md` |
| 8 | `Maui_Stage08_StylesThemesAnimation` | `第3部分-阶段8-样式主题动画手势.md` |
| 9 | `Maui_Stage09_HandlerNativeControls` | `第4部分-阶段9-Handler与原生控件.md` |
| 10 | `Maui_Stage10_DevicePermissions` | `第4部分-阶段10-设备能力权限平台服务.md` |
| 11 | `Maui_Stage11_NetworkDataAuth` | `第5部分-阶段11-网络数据与认证.md` |
| 12 | `Maui_Stage12_LocalStorageOffline` | `第5部分-阶段12-本地存储与离线优先.md` |
| 13 | `Maui_Stage13_BackgroundPush` | `第5部分-阶段13-后台推送通知.md` |
| 14 | `Maui_Stage14_AccessibilityI18n` | `第5部分-阶段14-无障碍本地化平台体验.md` |
| 15 | `Maui_Stage15_WindowsMacAdaptive` | `第6部分-阶段15-Windows-MacCatalyst与自适应.md` |
| 16 | `Maui_Stage16_GraphicsMedia` | `第6部分-阶段16-Graphics媒体高级交互.md` |
| 17 | `Maui_Stage17_TestingSystem` | `第7部分-阶段17-测试体系.md` |
| 18 | `Maui_Stage18_PerformanceDiagnostics` | `第7部分-阶段18-性能与诊断.md` |
| 19 | `Maui_Stage19_TrimmingAOT` | `第7部分-阶段19-Trimming-AOT-运行时.md` |
| 20 | `Maui_Stage20_PackagingCI` | `第7部分-阶段20-打包签名商店CI.md` |
| 21 | `Maui_Stage21_BlazorHybrid` | `第8部分-阶段21-Blazor-Hybrid与HybridWebView.md` |
| 22 | `Maui_Stage22_XamarinMigration` | `第8部分-阶段22-Xamarin-Forms迁移清理地图.md` |
| 23 | `Maui_Stage23_Tizen` | `第8部分-阶段23-Tizen可选.md` |
| 24 | `Maui_Stage24_MauiAppBuilderDI` | `第9部分-01-MauiAppBuilder-Host-DI.md` |
| 25 | `Maui_Stage25_BindableBindingXaml` | `第9部分-02-BindableObject-Binding-XAML编译.md` |
| 26 | `Maui_Stage26_HandlerMapperLayout` | `第9部分-03-Handler-Mapper-Layout.md` |
| 27 | `Maui_Stage27_WindowShellDispatcher` | `第9部分-04-Window-Shell-Dispatcher.md` |
| 28 | `Maui_Stage28_CollectionViewRuntime` | `第9部分-05-CollectionView-Essentials-Runtime.md` |

## 构建

单项目 Android 验证使用 Visual Studio MSBuild：

```powershell
& "$env:ProgramFiles\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
  .\Maui_Stage01_ArchitectureStartup\Maui_Stage01_ArchitectureStartup.csproj `
  /restore /t:Build /p:TargetFramework=net10.0-android /p:Configuration=Debug /m
```

仓库整体门禁使用根目录的非 CI 解决方案：

```powershell
& "$env:ProgramFiles\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
  .\LearnCSharp.slnx /restore /t:Build /p:Configuration=Debug /m
```

阶段 21 的原项目没有 Blazor 包，因此实现可直接编译运行的 `HybridWebView` 路径，并保留 BlazorWebView 的选型和 DI 知识；阶段 23 按文档默认 N/A 路径保留 Android 主线，只有具备 Samsung 目标设备、工具链和签名责任人后才启用 Tizen spike。
