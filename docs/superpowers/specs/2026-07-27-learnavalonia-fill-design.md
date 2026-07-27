# 设计：填充 LearnAvalonia 32 个项目（Avalonia 12 桌面主线）

> 创建于 2026-07-27。对应路线图：`C:\MyFile\ArcForges\ArchitectureDesign\AvaloniaStudy\` 的 35 份阶段/Capstone 文档 + 大纲。

## 1 · 背景与目标

`LearnCSharp` 仓 `src/LearnAvalonia/` 下已有 35 个占位项目（每个 6 个骨架文件：`Program.cs` / `App.axaml(.cs)` / `MainWindow.axaml(.cs)` / `.csproj`），全部已挂进 `LearnCSharp.CI.slnx`。当前仅 Stage01 的 `Program.cs` 有点像样内容，其余 34 个全是 `MainWindow` 显示一行 "Hello from StageNN" 文本。

目标：把 32 个项目（跳过 18/19/20）填充为「可运行演示」的 Avalonia 桌面应用，每个知识点一个可见 demo 页，与对应阶段文档一一对应、全面覆盖（从入门到专家级），本地 `dotnet build LearnCSharp.CI.slnx -c Release` 通过，pre-commit 钩子全绿，push 开 PR。

**非目标：**
- 移动端（Stage18）、WebAssembly（Stage19）、Embedded Linux（Stage20）——这三个项目保留骨架占位，只补 README 注明本仓库只做桌面主线。
- Capstone3（全平台）改为「桌面三平台（Windows/macOS/Linux）」版，不要求移动/Browser 宿主。
- 不要求真机/模拟器/WASM 实测；CI 只验证编译 + 桌面启动。

## 2 · 方案选定（方案 A：每项目「导航壳 + 实验页」）

每 Stage 项目是独立可运行的 Avalonia 桌面应用，统一「学习导航壳」：

```
┌─────────────────────────────────────────────────────────────┐
│  Stage NN · <主题>                                            │
├──────────────┬────────────────────────────────────────────────┤
│ 知识点列表    │  当前知识点演示 (ContentControl)                │
│  • §1 ...    │  ┌──────────────────────────────────────────┐  │
│  • §2 ...    │  │  演示区：可见 UI / 交互 / 数据            │  │
│  • §3 ... ▶  │  │                                          │  │
│  ...         │  └──────────────────────────────────────────┘  │
│              │  📄 Doc: 第N部分-阶段M.md §X                    │
│              │  ✅ 验收: <对应验收项>                          │
└──────────────┴────────────────────────────────────────────────┘
```

**结构：**
- 左侧 `ListBox` 列知识点标题（绑 `MainViewModel.Topics`）
- 右侧 `ContentControl` 绑选中项的 `Demo` 控件
- 顶部/底部信息条显示当前知识点的文档引用 + 验收要点
- `GridSplitter` 可调左右比例

**理由：** 与 LearnBlazor「每项目独立可跑」惯例一致；CI.slnx 无需改项目清单；知识点粒度可见可验证；不引入跨项目共享库（避免改动牵动所有阶段）。

## 3 · 范围与跳过清单

| 项目 | 处理 |
|---|---|
| Stage01–17 | 全填（桌面主线） |
| **Stage18 Mobile** | **跳过**，保留骨架 + README 注明 |
| **Stage19 WebAssembly** | **跳过**，保留骨架 + README 注明 |
| **Stage20 EmbeddedLinux** | **跳过**，保留骨架 + README 注明 |
| Stage21–31 | 全填（架构/数据/测试/性能/AOT/打包/迁移/源码/后端/渲染；桌面可演示部分） |
| **Capstone1** 桌面基础 | 全填 |
| **Capstone2** 复杂桌面 | 全填 |
| **Capstone3** 全平台 → 改「桌面三平台」 | 改造为 Win/macOS/Linux 三平台共享 UI 演示 |
| **Capstone4** 自定义控件+渲染+AOT | 全填 |

**跳过的 3 个项目处理：** 保持现有可编译骨架不变，在项目根加 `README.md` 说明「本仓库只做桌面主线，移动/WASM/Embedded 见路线图但不实现；如需补完，参考 `ArchitectureDesign/AvaloniaStudy/第8部分-阶段NN.md`」。

## 4 · 各项目知识点覆盖矩阵（32 个填实质内容 + 3 个保留）

每个项目映射对应阶段文档的每个带编号小节为一个 demo 页。主题计数来自文档扫描：

| 项目 | 文档 | 知识点数 | 填实质 |
|---|---|---|---|
| Stage01 AppBuilder | 阶段1 | 15 | ✅ |
| Stage02 Dispatcher | 阶段2 | 15 | ✅ |
| Stage03 AvaloniaXaml | 阶段3 | 16 | ✅ |
| Stage04 PropertySystem | 阶段4 | 18 | ✅ |
| Stage05 LogicalVisualTree | 阶段5 | 18 | ✅ |
| Stage06 LayoutSystem | 阶段6 | 17 | ✅ |
| Stage07 ControlSystem | 阶段7 | 16 | ✅ |
| Stage08 StylesThemes | 阶段8 | 14 | ✅ |
| Stage09 DataBindingMVVM | 阶段9 | 16 | ✅ |
| Capstone1 DesktopFoundation | Capstone1 | 10 | ✅ |
| Stage10 WindowsDialogs | 阶段10 | 19 | ✅ |
| Stage11 PagesNavigation | 阶段11 | 18 | ✅ |
| Stage12 InputFocusGestures | 阶段12 | 18 | ✅ |
| Stage13 GraphicsAnimation | 阶段13 | 21 | ✅ |
| Stage14 CustomControls | 阶段14 | 20 | ✅ |
| Stage15 Virtualization | 阶段15 | 17 | ✅ |
| Stage16 PlatformAbstraction | 阶段16 | 18 | ✅ |
| Stage17 WebViewHybrid | 阶段17 | 17 | ✅ |
| Stage18 Mobile | — | — | ⏭️ 跳过 |
| Stage19 WebAssembly | — | — | ⏭️ 跳过 |
| Stage20 EmbeddedLinux | — | — | ⏭️ 跳过 |
| Stage21 AppArchitecture | 阶段21 | 20 | ✅ |
| Stage22 DataNetworkAuth | 阶段22 | 19 | ✅ |
| Capstone2 ComplexDesktop | Capstone2 | 15 | ✅ |
| Stage23 AccessibilityI18n | 阶段23 | 12 | ✅ |
| Stage24 TestingSystem | 阶段24 | 14 | ✅ |
| Stage25 Performance | 阶段25 | 11 | ✅ |
| Stage26 TrimmingAOT | 阶段26 | 9 | ✅ |
| Stage27 PackagingCI | 阶段27 | 10 | ✅ |
| Stage28 WpfMigration | 阶段28 | 8 | ✅ |
| Capstone3 CrossPlatform→桌面三平台 | Capstone3 | 10 | ✅ 改造 |
| Stage29 SourceReading | 阶段29 | 18 | ✅ |
| Stage30 PlatformBackend | 阶段30 | 11 | ✅ |
| Stage31 RenderingExpert | 阶段31 | 17 | ✅ |
| Capstone4 CustomControlAOT | Capstone4 | 15 | ✅ |

**合计：** 32 个项目填实质，共约 524 个知识点 demo 页。3 个项目保留骨架。

## 5 · 统一项目结构与命名约定

### 5.1 目录与文件

```
Avalonia_StageNN_<Topic>/
├── Program.cs                          # BuildAvaloniaApp + StartWithClassicDesktopLifetime
├── App.axaml                            # FluentTheme + DataTemplates (可选)
├── App.axaml.cs                         # OnFrameworkInitializationCompleted → MainWindow
├── MainWindow.axaml                     # 导航壳：左 ListBox + GridSplitter + 右 ContentControl + 底部信息
├── MainWindow.axaml.cs
├── ViewModels/
│   ├── MainViewModel.cs                 # Topics 列表 + SelectedTopic (CommunityToolkit.Mvvm)
│   └── TopicItem.cs                     # Title / Doc / Acceptance / Demo (Func<Control>) / Order
├── Pages/
│   ├── <TopicName>Demo.axaml            # 每个知识点一个 UserControl 演示
│   └── <TopicName>Demo.axaml.cs
├── Docs/
│   └── StageNN.md                       # 本阶段知识点清单 + 文档引用映射（本地索引）
├── README.md                            # 本项目说明 + 知识点列表 + 运行方式
└── Avalonia_StageNN_<Topic>.csproj
```

### 5.2 命名约定

- 演示页文件：`Pages/<SectionSlug>Demo.axaml`，slug 取自文档小节标题（去 `·/空格/标点`，PascalCase）。例：§2.3 "Skia 与 HarfBuzz" → `Pages/SkiaHarfBuzzDemo.axaml`。
- 命名空间：`LearnAvalonia.Avalonia_StageNN_Topic`（与项目 RootNamespace 一致）。
- demo 控件类名：`<SectionSlug>Demo`，例 `SkiaHarfBuzzDemo : UserControl`。
- `TopicItem.Doc` 字段格式：`第N部分-阶段M-<名>.md §<节号> <节标题>`。

### 5.3 演示页头部注释模板

每个 `*Demo.axaml.cs` 顶部：
```csharp
// Doc:    第N部分-阶段M-<名>.md §<节号>
// Title:  <节标题>
// Stage:  StageNN
// Accept: <对应验收项（简述）>
// Notes:  <本 demo 演示的关键点；2–4 行>
```

### 5.4 导航壳统一实现

为保持一致并减少重复，所有项目复用同一套 `MainViewModel`/`MainWindow` 结构（每项目各持一份拷贝，按需微调标题/列宽）：

- `MainViewModel` 用 `CommunityToolkit.Mvvm` 的 `[ObservableProperty]` / `[RelayCommand]`。
- `Topics : ObservableCollection<TopicItem>`，启动时按文档顺序填充。
- `SelectedTopic` 变化时，`ContentControl` 通过 `Content` 绑定 + `DataTemplates`（或 `FuncValueConverter`）展示对应 `Demo` 控件实例（每项目用 `DemoTemplate`：`TopicItem → Control`，懒构造缓存）。
- 底部信息条绑 `SelectedTopic.Doc` + `SelectedTopic.Acceptance`。

## 6 · 依赖与 CPM

### 6.1 新增 PackageVersion（加进 `Directory.Packages.props`）

```xml
<PackageVersion Include="CommunityToolkit.Mvvm" Version="8.4.2" />
<!-- Avalonia.Headless.XUnit 12.1.0（仅 Stage24 测试体系用） -->
<PackageVersion Include="Avalonia.Headless" Version="12.1.0" />
<PackageVersion Include="Avalonia.Headless.XUnit" Version="12.1.0" />
<!-- Stage17 WebView -->
<PackageVersion Include="Avalonia.Controls.WebView" Version="12.0.1" />
<!-- Stage15 DataGrid（维护边界，演示 deprecated 控件） -->
<PackageVersion Include="Avalonia.Controls.DataGrid" Version="12.1.0" />
<!-- Stage14/Stage15 ItemsRepeater -->
<PackageVersion Include="Avalonia.Controls.ItemsRepeater" Version="12.0.0" />
<!-- Stage8 ColorPicker -->
<PackageVersion Include="Avalonia.Controls.ColorPicker" Version="12.1.0" />
<!-- Stage23/21 Behaviors（可选） -->
<PackageVersion Include="Xaml.Behaviors.Avalonia" Version="12.0.5" />
<!-- Stage22 数据/网络（HttpClientFactory/STJ 已在 BCL 或 M.E） -->
<!-- Stage22 SQLite/EF Core（演示本地存储） -->
<PackageVersion Include="Microsoft.Data.Sqlite" Version="10.0.0" />
<PackageVersion Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.10" />
<!-- Stage21 Hosting/DI（已在 CPM） -->
<!-- Stage29/30/31 源码阅读：无额外包，用反射/Avalonia.Diagnostics API -->
```

> 注：版本号取自路线图 §1.3.2 校准表。`Microsoft.EntityFrameworkCore.Sqlite` 版本对齐仓内已有 `Microsoft.EntityFrameworkCore.Relational 10.0.10`。`Microsoft.Data.Sqlite` 跟 EF Core 10.0.10 对齐（10.0.0 系）。

### 6.2 各项目 `.csproj` 依赖

基础四个（所有 32 个填实质项目）：
```xml
<PackageReference Include="Avalonia" />
<PackageReference Include="Avalonia.Desktop" />
<PackageReference Include="Avalonia.Themes.Fluent" />
<PackageReference Include="Avalonia.Fonts.Inter" />
<PackageReference Include="CommunityToolkit.Mvvm" />
```

按需追加（见 §6.1 注释）。

### 6.3 测试项目（Stage24）

Stage24 的「测试体系」需含可运行测试。建一个 `Avalonia_Stage24_TestingSystem.Tests` 测试项目（net10.0；引用 Headless.XUnit + xunit.v3），挂进 `LearnCSharp.CI.slnx` 的 `/src/LearnAvalonia/` folder。其余 32 个主线项目保持「可运行应用」而非「带测试」——测试 demo 在 Stage24 项目本身内通过 Headless 演示（运行时弹窗 + 文档说明），单独测试项目做 CI 可执行验证。

## 7 · CI 与构建约束

### 7.1 `LearnCSharp.CI.slnx` 变更

- 不改既有 35 个项目条目。
- 新增 1 条：`Avalonia_Stage24_TestingSystem.Tests` 测试项目。
- 跳过的 Stage18/19/20 保持现有骨架（已能编译），不动。

### 7.2 构建验证

```pwsh
dotnet build LearnCSharp.CI.slnx -c Release
dotnet test LearnCSharp.CI.slnx --filter "FullyQualifiedName~Avalonia_Stage24"  # Stage24 测试
```

桌面运行验证（本地手测，CI 不强制）：`dotnet run --project src/LearnAvalonia/Avalonia_StageNN --c Debug`，窗口出现即过。

### 7.3 pre-commit 钩子

- `trailing-whitespace` / `end-of-file-fixer` / `mixed-line-ending`（LF）：所有 `.axaml`/`.cs`/`.md` 文件遵循 LF（`.gitattributes` 已设 `* text=auto eol=lf`）。
- `codespell`：英文拼写；中文注释不影响。
- `dotnet-format`（针对 `LearnCSharp.slnx`，非 CI.slnx）：确保 `.cs` 符合 `.editorconfig`（file_scoped namespace / 无 var / using 排序）。
- `check-added-large-files`（≤2MB）：演示用资源避免大文件。
- `check-yaml` / `check-toml` / `check-json`：不涉及新增。
- **关键：** `.editorconfig` 规定 `csharp_style_namespace_declarations = file_scoped:warning`、`csharp_style_var_* = false:warning`——所有新代码必须用 file-scoped namespace、显式类型（不用 `var`）。

## 8 · 工作流程

### 8.1 git worktree

```pwsh
git -C C:\MyFile\ArcForges\LearnCSharp worktree add -b learnavalonia-fill .worktree/learnavalonia-fill main
```

所有改动在 worktree 分支 `learnavalonia-fill` 上进行。

### 8.2 提交策略

按里程碑分批提交（每里程碑一次提交 + 推送 CI 验证）：

| 提交 | 范围 | 预计文件数 |
|---|---|---|
| 1 | CPM + Stage24 测试项目骨架 + 导航壳模板 + Stage01–05 | ~80 |
| 2 | Stage06–09 + Capstone1 | ~80 |
| 3 | Stage10–14 | ~90 |
| 4 | Stage15–17 + Capstone3 改造 | ~70 |
| 5 | Stage21–22 + Capstone2 | ~70 |
| 6 | Stage23–28 | ~90 |
| 7 | Stage29–31 + Capstone4 + 跳过项目 README | ~80 |
| 8 | 收尾：CI.slnx 加测试条目、README 汇总、pre-commit 全绿 | ~5 |

每次提交前：`dotnet build LearnCSharp.CI.slnx -c Release` 通过 + `pre-commit run --all-files` 通过（本地）。

### 8.3 PR

push 后开 PR：
```pwsh
gh pr create --base main --head learnavalonia-fill `
  --title "feat: fill LearnAvalonia 32 projects (Avalonia 12 desktop mainline)" `
  --body "..."
```

## 9 · 风险与缓解

| 风险 | 缓解 |
|---|---|
| 524 个 demo 页工作量巨大，单会话难全填 | 按里程碑提交；每里程碑自包含可编译；必要时分多 PR（但本次目标是单 PR 收口） |
| 演示页质量参差 | 统一模板 + 头部注释；每页含「演示区 + 文档引用 + 验收要点」三要素；CI 编译兜底 |
| Avalonia 12 API 细节错误 | 严格按路线图校准表（§1.3.2 包版本、§1.4 能力矩阵、附录 B breaking）；不写 `Avalonia.ReactiveUI`/`UseDirect2D1`/旧 FileDialog |
| pre-commit 钩子改文件 | 提交前本地跑 `pre-commit run --all-files`；遵循 `.editorconfig`（file-scoped namespace / 显式类型 / LF） |
| WebView/ItemsRepeater 版本落后核心 | 单独 pin（已在 §6.1） |
| Stage24 Headless 测试需 xUnit v3 | CPM 已有 `xunit.v3 3.2.2`；Headless.XUnit 12.1.0 兼容 |
| 大文件触发 `check-added-large-files` | 演示不引外部资源；用 Avalonia 内置图形/几何生成 |

## 10 · 验收

- [ ] `dotnet build LearnCSharp.CI.slnx -c Release` 0 警告 0 错误
- [ ] `dotnet test LearnCSharp.CI.slnx --filter Avalonia_Stage24` 通过
- [ ] `pre-commit run --all-files` 全绿
- [ ] 32 个项目每个含 README + Docs/StageNN.md + N 个 demo 页（N = 知识点数）
- [ ] 每个 demo 页头部含 `Doc:` / `Accept:` 注释
- [ ] 跳过的 3 个项目含 README 注明跳过原因
- [ ] PR 开启、CI 绿

## 11 · 文档引用

- 大纲：`C:\MyFile\ArcForges\ArchitectureDesign\AvaloniaStudy\Avalonia-12-全平台应用完整学习路线图.md`
- 阶段/Capstone 文档：`C:\MyFile\ArcForges\ArchitectureDesign\AvaloniaStudy\第N部分-阶段M-<名>.md`（35 份）
- 仓内 README：`C:\MyFile\ArcForges\LearnCSharp\README.md`（LearnCSharp 整体说明）
- 仓内惯例：`src/LearnBlazor/Blazor_Part02_HtmlModernCss/`（每项目独立可跑的范例）