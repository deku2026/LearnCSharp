# LearnBlazor 填充设计（Part02–16，纯 Web 前端）

> 日期：2026-07-26
> 源文档：`C:\MyFile\ArcForges\ArchitectureDesign\BlazorStudy\第2-16部分*.md`
> 目标工程：`src/LearnBlazor/Blazor_Part02..16_*`（15 个占位工程，均为 `Microsoft.NET.Sdk.Web` + Blazor Server Interactive）

## 1 · 目标与边界

### 1.1 目标
把 BlazorStudy 文档（第 2–16 部分）对应的 **15 个 LearnBlazor 占位工程** 填充为 **中型可编译演示**，覆盖从入门到专家级核心机制与验收点，本地编译通过后提交 push。

### 1.2 宿主模型（已与用户确认）
- **纯 Blazor Server Web 前端**（`Microsoft.NET.Sdk.Web` + `AddInteractiveServerComponents`），不引入 MAUI/Hybrid。
- 文档里 MAUI/Hybrid 专属内容适配为 Web 语境；纯 Web 不可实现的部分 **跳过**，工程保持占位可编译。
- **Part11（原生平台桥接）跳过**：相机/分享/SecureStorage/MAUI 权限/`BlazorWebView.TryDispatchAsync` 全是 MAUI 专属，Web 无对应"原生桥接"概念。保留现有占位文件不填。
- "最多有后端的结合"：Part12 需要后端时建一个最小 ASP.NET Core Minimal API 后端，Blazor 前端用 `HttpClient` 调用。

### 1.3 非目标
- 不做 MAUI / iOS / Android 打包。
- 不做 WebAssembly 托管切换（保持 Server）。
- 不做 Part17（源码阅读专家）/ Part18（Capstone 1-3）——LearnBlazor 没有对应占位工程。
- 不做真实生产业务，演示数据均为内存假数据。

## 2 · Part 映射表（文档 → 实现方案）

| Part | 文档主题 | Web 实现 | 新增子项目 |
|---:|---|---|---|
| 02 | HTML/CSS/移动视觉地基 | 静态演示页：tokens.css + App Shell（顶栏/内容/底栏）+ 暗色 + safe-area + reduced-motion + `@layer` + container query + drawer | 无 |
| 03 | Razor Components 核心 | 参数/级联/片段/事件/生命周期/CSS isolation/`@bind`/`DynamicComponent`/`SectionOutlet` | 无 |
| 04 | 前端状态架构 | Scoped 服务/级联/状态容器+订阅/`CascadeType`/状态流心智图页 | 无 |
| 05 | 导航 App Shell | `Router`/`NavMenu`/`MainLayout`/相对+查询参数/404/`NavigateTo`/`FocusOnNavigate` | 无 |
| 06 | 表单/输入/键盘/触控/文件 | `EditForm`/`EditContext`/验证/`InputFile`+防伪/触控目标/`InputText/Select/Checkbox/TextArea` | 无 |
| 07 | 列表/媒体/消费级 UI | `Virtualize` 虚拟化/卡片网格/图标系统/媒体占位/`@key`/`@ref` | 无 |
| 08 | 无障碍/i18n | ARIA landmark/focus trap/skip link + `IStringLocalizer`+`CultureSelector`+`RequestLocalization` | 无 |
| 09 | 组件库/样式方案定案 | **RCL 类库** `Blazor_Part09_ComponentLibStyling.RazorLib`（`Microsoft.NET.Sdk.Razor`） + 主 Web 项目消费 + CSS isolation + token 共享 + 静态资产 `_content` 路径 | **+1 RCL** |
| 10 | JS 互操作/WebView | `IJSRuntime`/ES module import/`DotNetObjectReference`/Dispose/`JSDisconnectedException`/CSP/`MarkupString` 边界（跳过 WebView 配置） | 无 |
| 11 | 原生平台桥接 | **⛔ 跳过**（MAUI 专属）保留占位 | 无 |
| 12 | Code-First RPC 客户端 | 适配为 **HTTP 客户端**：`HttpClient` factory + 重试/超时/`IHttpClientFactory`+`Microsoft.Extensions.Http.Resilience` + 错误映射 + UI 状态机（loading/content/empty/error） | **+1 迷你后端** `Blazor_Part12_CodeFirstRpcClient.Api`（Minimal API + 假数据） |
| 13 | 认证安全边界 | `AuthenticationStateProvider` 自定义 + `[Authorize]`/`AuthorizeRouteView`/`AuthorizeView` + JWT 内存 + `CascadingAuthenticationState` + 路由守卫 | 无 |
| 14 | 测试调试质量 | **bUnit** 组件单测项目（xUnit + bUnit）演示渲染/事件/输入/快照；主项目加日志/错误边界演示 | **+1 测试项目** `Blazor_Part14_TestingDebugging.Tests` |
| 15 | 性能诊断 | 流式渲染 `[StreamRendering]`/虚拟化重用/`RevalidatingServerTypeState`/渲染减少/`IComponent` 手写/诊断中间件 | 无 |
| 16 | 打包发布工程化 | Web 发布工程：`dotnet publish` 配置（Release/静态资产/Trimming）+ Dockerfile + `appsettings` 环境分层 + 静态资产缓存头 + 健康检查 | 无 |

**新增子项目合计**：3 个（RCL、迷你后端、测试项目），均加入 `LearnCSharp.CI.slnx`。

## 3 · 工程约定（对齐仓库）

### 3.1 csproj 模板
所有 Web 项目沿用占位模板（`Microsoft.NET.Sdk.Web` + `AssemblyName`/`RootNamespace`），不显式写 `TargetFramework`/`LangVersion`（由 `Directory.Build.props` 统一：`net10.0`、`LangVersion 14.0`、`Nullable enable`、`ImplicitUsings enable`、CPVM）。

### 3.2 命名
- 项目名：`Blazor_Part{NN}_{Topic}`（已存在，不改）
- RootNamespace：`LearnBlazor.Blazor_Part{NN}_{Topic}`
- 测试项目：`Blazor_Part14_TestingDebugging.Tests`（RootNamespace `LearnBlazor.Blazor_Part14_Tests`）
- RCL：`Blazor_Part09_ComponentLibStyling.RazorLib`
- 后端：`Blazor_Part12_CodeFirstRpcClient.Api`

### 3.3 中央包版本（`Directory.Packages.props` 新增）
```
<PackageVersion Include="bunit.web" Version="1.40.0" />
<PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="10.8.0" />  // 已存在
<PackageVersion Include="Microsoft.AspNetCore.Components.WebAssembly.Server" ... >  // 不需要，Server 模式
```
（具体版本以编写时 NuGet 最新稳定为准，CPVM 必填，否则 `dotnet restore` 报 NU1007。）

### 3.4 静态资产
- 全局壳/token CSS 放 `wwwroot/css/`。
- 组件私有样式用 CSS isolation（`Component.razor.css`）。
- RCL 静态资产 `_content/{PackageId}/...`。

## 4 · 每个 Part 的具体交付物（中型演示）

> 每个填的 Part：多 Razor 页面 + 组件 + 必要服务/CSS + 一个"导读页"列出本部分机制点与验收清单。所有页面可从主菜单导航。每个 Part 目录 `Components/Pages/` 下 3-6 个 `.razor`。

### Part02 HTML/CSS 移动视觉地基
- `Components/Pages/ShellDemo.razor`：静态 HTML+CSS 移动壳（顶栏/内容/底栏三 Tab）。
- `wwwroot/css/tokens.css`：CSS 变量 token 层（亮/暗 `prefers-color-scheme` + `html[data-theme]`）。
- `wwwroot/css/shell.css`：`@layer reset/tokens/base/components` + flex 壳 + drawer + safe-area + `:focus-visible` + `prefers-reduced-motion`。
- `Components/Pages/TokensDemo.razor`、`SafeAreaDemo.razor`、`DrawerDemo.razor`、`ReducedMotionDemo.razor`。
- **无 Blazor 逻辑依赖**，纯 HTML/CSS 演示页，可双击 `wwwroot` 看。

### Part03 Razor Components 核心
- `Components/`：`Greeting.razor`（参数）、`Child.razor`（级联）、`Fragments.razor`（`RenderFragment`/`SectionOutlet`）、`Lifecycle.razor`（`OnInitialized`/`OnParametersSet`/`OnAfterRender`+`IDisposable`）、`TwoWay.razor`（`@bind`）、`Dynamic.razor`（`DynamicComponent`）。
- `Components/Pages/`：导读页 + 各组件演示页。
- `Components/Lifecycle.razor.css` 演示 CSS isolation。

### Part04 前端状态架构
- `Services/`：`CounterState.cs`（事件订阅 `event Action`）、`CartState.cs`、`ThemeState.cs`。
- `Components/`：`Counter.razor`（订阅刷新）、`Cart.razor`、`ThemeProvider.razor`（级联 `CascadingValue`）。
- `Program.cs`：`AddScoped<CounterState>()` 等。
- `Components/Pages/StateFlow.razor`：状态流心智图（单向流演示）。

### Part05 导航 App Shell
- `Components/App.razor`：`Routes` 用 `AuthorizeRouteView`（Part13 复用）。
- `Components/Layouts/MainLayout.razor` + `.razor.css`（顶栏 + NavMenu + `<main>`）。
- `Components/Layouts/NavMenu.razor`。
- `Components/Pages/`：`Home`、`About?query=...`（查询参数）、`Items/{Id:int}`（路由参数）、`NotFound`（404）。
- `FocusOnNavigate` 演示。

### Part06 表单/输入/键盘/触控/文件
- `Models/Contact.cs`（`[Required]`/`[EmailAddress]`/`[StringLength]`）。
- `Components/Pages/ContactForm.razor`（`EditForm`+`EditContext`+`DataAnnotationsValidator`+`ValidationSummary`）。
- `Components/Pages/FileUpload.razor`（`InputFile` + 防伪 + 大小/类型校验）。
- `Components/Pages/TouchTargets.razor`（≥44px 触控演示）。
- 自定义 `InputComponent` 子类（`CustomInputText.razor`：继承 `InputBase<string>`）。

### Part07 列表/媒体/消费级 UI
- `Services/ItemService.cs`（内存 1 万条假数据）。
- `Components/Pages/VirtualList.razor`（`Virtualize` + `ItemsProvider`）。
- `Components/Pages/CardGrid.razor`（grid auto-fill + `@key`）。
- `Components/Icon.razor`（内联 SVG + `currentColor`）。
- `Components/Pages/MediaPlaceholder.razor`（懒加载占位）。

### Part08 无障碍/i18n
- `Components/Layouts/SkipLink.razor`、`Components/FocusTrap.razor`（modal 焦点陷阱 via JS interop）。
- `Resources/Pages.Home.resx` + `Resources/Pages.Home.zh-CN.resx`（`IStringLocalizer`）。
- `Components/CultureSelector.razor`（cookie/localStorage 切换 culture）。
- `Program.cs`：`AddLocalization()` + `RequestLocalizationOptions`（`SupportedCultures`）。
- `Components/Pages/A11yDemo.razor`（landmark/aria-current/`sr-only`）。

### Part09 组件库/样式方案定案
- **新增 RCL 子项目** `Blazor_Part09_ComponentLibStyling.RazorLib`（`Microsoft.NET.Sdk.Razor`）：
  - `wwwroot/css/tokens.css`（token 权威，复用 Part02）。
  - `Components/Button.razor`/`.razor.css`、`Card.razor`/`.razor.css`、`Badge.razor`/`.razor.css`。
  - `_Imports.razor`。
- 主 Web 项目 `Blazor_Part09_ComponentLibStyling` 引用 RCL，`Components/Pages/ConsumeLib.razor` 演示消费 + 静态资产 `_content/.../tokens.css`。

### Part10 JS 互操作/WebView
- `wwwroot/js/interop.js`（ES module：`export function focusTrap(...)`/`observeViewport(...)`）。
- `Components/JsInterop.razor`（`IJSRuntime.ImportAsync` → `IJSObjectReference`，`OnDispose` 释放）。
- `Components/JsCallback.razor`（`DotNetObjectReference` + `[JSInvokable]`，IntersectionObserver 回调）。
- `Components/Pages/CspDemo.razor`（CSP meta 演示 + `MarkupString` XSS 边界警示）。
- `Program.cs`：CSP 头中间件演示（可选 `X-Content-Security-Policy`）。

### Part11 ⛔ 跳过
- 保留占位：`Program.cs`/`App.razor`/`Routes.razor`/`Home.razor`/`_Imports.razor`/`app.css` 不动。
- `Home.razor` 注释一行：`Part11 - 原生平台桥接（MAUI 专属，纯 Web 跳过）。`

### Part12 Code-First RPC 客户端 → HTTP 客户端
- **新增迷你后端** `Blazor_Part12_CodeFirstRpcClient.Api`（`Microsoft.NET.Sdk.Web` Minimal API）：
  - `Program.cs`：`/api/items` GET/POST 假数据 + `/api/items/{id}` + 延迟/错误端点演示。
  - `appsettings.json` + CORS 允许前端端口。
- 前端 `Blazor_Part12_CodeFirstRpcClient`：
  - `Services/ItemsClient.cs`（`HttpClient` + `IHttpClientFactory` + `Microsoft.Extensions.Http.Resilience` 重试/超时/熔断）。
  - `Models/Item.cs`、`Contracts/`（错误码/失败类型）。
  - `Components/Pages/ItemsPage.razor`（UI 状态机：loading/content/empty/error，`@inject ItemsClient`）。
  - `Program.cs`：`AddHttpClient<ItemsClient>` + resilience handler + 配置后端 URL。
- 后端启动端口在 `appsettings`，前端 `appsettings.Development` 指向 `https://localhost:5xxx`。

### Part13 认证安全边界
- `Auth/`：`SimpleAuthStateProvider.cs`（`AuthenticationStateProvider`，内存用户+JWT）、`AuthService.cs`（登录/登出/`ClaimsPrincipal`）。
- `Models/LoginModel.cs`。
- `Components/Pages/Login.razor`、`Account.razor`（`[Authorize]`）、`Admin.razor`（`[Authorize(Roles="admin")]`）。
- `Components/AuthorizeView.razor` 演示 `Authorized`/`NotAuthorized`/`Authorizing`。
- `Program.cs`：`AddAuthentication`/`AddAuthorization` + `CascadingAuthenticationState`。
- `App.razor` 用 `AuthorizeRouteView`。

### Part14 测试调试质量
- **新增测试项目** `Blazor_Part14_TestingDebugging.Tests`（`Microsoft.NET.Sdk` + xUnit + bUnit）：
  - `CounterTest.cs`（渲染/点击/断言）、`FormTest.cs`（输入/提交/验证）、`NavTest.cs`（`FakeNavigationManager`）。
  - 引用 `Blazor_Part03_RazorComponents`（被测组件来源）或自建 `FixtureComponents/`。
- 主项目 `Blazor_Part14_TestingDebugging`：
  - `Components/Pages/ErrorBoundary.razor`（`ErrorBoundary` 演示）、`LoggingDemo.razor`（`ILogger`+`ILoggerProvider` 简易）、`Diagnostics.razor`（渲染计数/`ComponentBase` 手写）。

### Part15 性能诊断
- `Components/Pages/StreamRendering.razor`（`[StreamRendering]` + `IAsyncEnumerable`）。
- `Components/Pages/VirtualReuse.razor`（大列表虚拟化复用 Part07）。
- `Components/Pages/ReduceRender.razor`（`ShouldRender` 重写、`@bind:after`、`Virtualize`）。
- `Components/ManualComponent.cs`（手写 `IComponent` 演示）。
- `Program.cs`：响应压缩 + 静态资产缓存头中间件。

### Part16 打包发布工程化
- `Blazor_Part16_PackagingEngineering.csproj`：`<PropertyGroup>` 加 Release 配置（`<Optimize>`/`<DebugType>none`/`<ServerGarbageCollection>`）。
- `Dockerfile`（多阶段构建：sdk → aspnet runtime，发布 + 启动）。
- `appsettings.Production.json`（环境分层）。
- `Components/Pages/PublishNotes.razor`（讲解 `dotnet publish -c Release -o publish`、Trimming/AOT 取舍、静态资产）。
- `Program.cs`：健康检查 `AddHealthChecks()` + `MapHealthChecks("/healthz")` + 静态资产缓存头。
- `docs/` 子目录（项目内）：`PUBLISH.md`（发布步骤）——可选，若仓库惯例不放 docs 则进 `Components/Pages`。

## 5 · 编译验证

- `dotnet build LearnCSharp.CI.slnx -c Debug`（含所有新增项目，零警告目标；占位 Part11 仍编译通过）。
- `dotnet test` Part14 测试项目。
- 单独编译 Blazor 子集命令：`dotnet build LearnCSharp.CI.slnx -c Debug /p:Projects=src/LearnBlazor/**`（或直接 build slnx 后过滤）。
- 不引入新警告（仓库 `TreatWarningsAsErrors`? 查 `Directory.Build.props` 未设，但 `EnableNETAnalyzers=true` + `AnalysisLevel=latest`，保持零警告）。

## 6 · 提交与 push

- worktree：`C:\MyFile\ArcForges\.worktrees\LearnCSharp-blazor`，分支 `feat/learnblazor-fill`。
- 提交信息（对齐仓库 `feat: ... (#NN)` 风格，但无 PR 号先本地提交）：
  `feat: fill LearnBlazor Part02-16 demos (Web-only, RCL + mini-api + bUnit tests)`
- push 到 origin `feat/learnblazor-fill`，提示用户可开 PR。

## 7 · 风险与对策

| 风险 | 对策 |
|---|---|
| CPVM 漏配 bUnit 版本 → NU1007 | 编写前查 NuGet，先加 `PackageVersion` 再引用 |
| Part12 后端端口冲突 | 用 `appsettings` 固定端口，前端 `HttpClient` 配置 URL，CI slnx 不跑后端只 build |
| RCL 主项目引用循环 | RCL 不引用主项目，单向引用 |
| Part13 JWT 库版本 | 用框架自带 `Microsoft.AspNetCore.Authentication.JwtBearer`（已 CPVM 10.0.10）或简化 cookie |
| 静态资产路径 | RCL `_content/{PackageId}/`，注意 `PackageId` 默认=程序集名 |
| token 量 | 每个 Part 一次写入批次；先骨架后细节；用 `write` 大块写减少往返 |

## 8 · 实施顺序

1. 建 worktree + 基线 build 验证占位编译通过。
2. 加 CPVM 包版本（bUnit、resilience 已有）。
3. Part02（纯 CSS 基础，无依赖）。
4. Part03→08（组件/状态/导航/表单/列表/a11y，独立）。
5. Part09（RCL，需新子项目 + slnx）。
6. Part10（JS interop）。
7. Part12（后端 + 前端，需新子项目 + slnx）。
8. Part13（认证）。
9. Part14（测试项目 + slnx）。
10. Part15、Part16。
11. 整体 build slnx → 修警告 → 测试 → commit → push。
