# LearnBlazor Fill Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fill 14 of 15 LearnBlazor placeholder projects (Part02–16, skip Part11) with mid-size Blazor Server Web demos derived from BlazorStudy docs, add 3 new sub-projects (RCL, mini-api, bUnit tests), and have LearnCSharp.CI.slnx build clean.

**Architecture:** Each Part keeps its existing `Microsoft.NET.Sdk.Web` csproj (Blazor Server Interactive). Shared conventions via `Directory.Build.props` (net10.0/LangVersion 14/Nullable/CPVM). New sub-projects added to CI.slnx. No MAUI/Hybrid.

**Tech Stack:** .NET 10, Blazor Server (`AddInteractiveServerComponents`), bUnit (xUnit), `Microsoft.Extensions.Http.Resilience`, RCL (`Microsoft.NET.Sdk.Razor`).

## Global Constraints

- TargetFramework: `net10.0` (from `Directory.Build.props`); LangVersion 14; Nullable enable; ImplicitUsings enable.
- CPVM (`ManagePackageVersionsCentrally=true`): all package refs must have `<PackageVersion>` in `Directory.Packages.props`.
- Naming: project `Blazor_Part{NN}_{Topic}`, RootNamespace `LearnBlazor.Blazor_Part{NN}_{Topic}`.
- Worktree path: `C:\MyFile\ArcForges\.worktrees\LearnCSharp-blazor` (branch `feat/learnblazor-fill`).
- All builds must yield **0 warnings 0 errors** (`EnableNETAnalyzers=true` + `AnalysisLevel=latest`).
- Part11 untouched (skip MAUI-only content).
- All work happens in the worktree; verify with `dotnet build LearnCSharp.CI.slnx -c Debug`.

## File Structure

Per existing placeholder scaffold, each Part has:
- `Blazor_Part{NN}_{Topic}/Blazor_Part{NN}_{Topic}.csproj` (existing)
- `Program.cs` (existing — modify to add services/middleware per Part)
- `Components/App.razor`, `Routes.razor`, `_Imports.razor` (existing — extend imports)
- `Components/Pages/Home.razor` (existing — becomes index with links)
- `wwwroot/app.css` (existing)

New shared per-Part directories: `Components/` (non-page components), `Services/`, `Models/`, `Auth/`, `Resources/`, `wwwroot/css/`, `wwwroot/js/`.

New sub-projects (all in `src/LearnBlazor/`):
- `Blazor_Part09_ComponentLibStyling.RazorLib/` (RCL)
- `Blazor_Part12_CodeFirstRpcClient.Api/` (minimal API backend)
- `Blazor_Part14_TestingDebugging.Tests/` (xUnit+bUnit)

Each Part's `Components/Pages/Home.razor` becomes a导读 index listing demo links + 验收 checklist summary.

---

## Task 0: Setup & baseline

**Files:** none modified

- [ ] **Step 1:** Confirm worktree exists at `C:\MyFile\ArcForges\.worktrees\LearnCSharp-blazor` on branch `feat/learnblazor-fill`.
- [ ] **Step 2:** Baseline build clean: `dotnet build LearnCSharp.CI.slnx -c Debug -v q` → 0 warnings/0 errors (already verified).
- [ ] **Step 3:** Add CPVM package versions to `Directory.Packages.props`:
```xml
    <!-- LearnBlazor -->
    <PackageVersion Include="bunit.web" Version="1.40.0" />
    <PackageVersion Include="bunit.core" Version="1.40.0" />
```
(Resilience package `Microsoft.Extensions.Http.Resilience` 10.8.0 already present.)
- [ ] **Step 4:** Build again to confirm CPVM still valid.
- [ ] **Step 5:** Commit: `chore: add bUnit CPVM entries for LearnBlazor`.

---

## Task 1: Part02 — HTML/CSS mobile visual foundation

**Files (all in `src/LearnBlazor/Blazor_Part02_HtmlModernCss/`):**
- Create: `wwwroot/css/tokens.css`, `wwwroot/css/shell.css`, `wwwroot/css/shell.html` (static demo)
- Create: `Components/Pages/ShellDemo.razor`, `TokensDemo.razor`, `SafeAreaDemo.razor`, `DrawerDemo.razor`, `ReducedMotionDemo.razor`
- Modify: `Components/App.razor` (link tokens.css + shell.css), `Components/Pages/Home.razor` (index links)
- Modify: `Components/_Imports.razor` (no change needed — already web)

**Produces:** A pure-CSS mobile shell demo reachable from Home; `tokens.css`/`shell.css` reusable by later Parts.

- [ ] **Step 1:** Write `wwwroot/css/tokens.css` with palette + semantic tokens + dark via `prefers-color-scheme` + `html[data-theme]`.
- [ ] **Step 2:** Write `wwwroot/css/shell.css` with `@layer reset,tokens,base,components,utilities`; flex app shell (top-bar/content/bottom-nav); `.sr-only`; safe-area; `:focus-visible`; `prefers-reduced-motion`; drawer.
- [ ] **Step 3:** Write `Components/Pages/ShellDemo.razor` — full mobile shell with 30 fake rows + bottom nav + drawer toggle (CSS-driven via `@onclick` toggling a class).
- [ ] **Step 4:** Write `TokensDemo.razor`/`SafeAreaDemo.razor`/`DrawerDemo.razor`/`ReducedMotionDemo.razor` — focused demos each explaining one mechanism with prose.
- [ ] **Step 5:** Modify `App.razor` to include `tokens.css` + `shell.css` (or keep per-page via `<HeadOutlet>`; choose global include for simplicity).
- [ ] **Step 6:** Modify `Home.razor` to list the 5 demo links + 验收 checklist.
- [ ] **Step 7:** `dotnet build LearnCSharp.CI.slnx -c Debug -v q` → 0/0.
- [ ] **Step 8:** Commit: `feat(Part02): HTML/CSS mobile shell + tokens + safe-area demos`.

---

## Task 2: Part03 — Razor Components core

**Files (in `src/LearnBlazor/Blazor_Part03_RazorComponents/`):**
- Create: `Components/Greeting.razor`, `Child.razor`, `Fragments.razor`, `Lifecycle.razor` (+`.razor.css`), `TwoWay.razor`, `DynamicHost.razor`
- Create: `Components/Pages/Index.razor`, `ParamsDemo.razor`, `CascadeDemo.razor`, `FragmentsDemo.razor`, `LifecycleDemo.razor`, `BindDemo.razor`, `DynamicDemo.razor`
- Modify: `Components/_Imports.razor` (add `@using LearnBlazor.Blazor_Part03_RazorComponents.Components`)
- Modify: `Components/Pages/Home.razor` (index)

- [ ] **Step 1:** Write `Greeting.razor` (`[Parameter] string Name`, `[Parameter] RenderFragment? Header`).
- [ ] **Step 2:** Write `Child.razor` with `[CascadingParameter] Theme` + `[Parameter] ChildContent`.
- [ ] **Step 3:** Write `Fragments.razor` using `<SectionOutlet SectionId="..." />` + `SectionContent`.
- [ ] **Step 4:** Write `Lifecycle.razor` logging `OnInitialized{Async}`/`OnParametersSet`/`OnAfterRender{Async}` + `IDisposable`. Add `Lifecycle.razor.css` (scoped).
- [ ] **Step 5:** Write `TwoWay.razor` with `@bind` + `@bind:after` + custom `bind:value`.
- [ ] **Step 6:** Write `DynamicHost.razor` using `DynamicComponent Type="@selected" Parameters="@params"`.
- [ ] **Step 7:** Write all `Components/Pages/*Demo.razor` consuming each component.
- [ ] **Step 8:** `Home.razor` index links.
- [ ] **Step 9:** Build → 0/0.
- [ ] **Step 10:** Commit: `feat(Part03): Razor Components params/cascade/lifecycle/fragments/bind/dynamic demos`.

---

## Task 3: Part04 — Frontend state architecture

**Files (in `src/LearnBlazor/Blazor_Part04_FrontendStateArchitecture/`):**
- Create: `Services/CounterState.cs`, `Services/CartState.cs`, `Services/ThemeState.cs`
- Create: `Components/CounterView.razor`, `Components/CartView.razor`, `Components/ThemeProvider.razor`
- Create: `Components/Pages/StateFlow.razor`, `ScopedServiceDemo.razor`, `CascadeDemo.razor`
- Modify: `Program.cs` (register scoped states), `_Imports.razor`

**Produces:** `CounterState` with `event Action? Changed` + `OnStateChanged()` pattern reused by Part13-style states.

- [ ] **Step 1:** Write `CounterState.cs` — `int Count`, `void Increment()`, `event Action? Changed`, `IDisposable`.
- [ ] **Step 2:** Write `CartState.cs` — `List<CartItem>`, add/remove/total, `event Action? Changed`.
- [ ] **Step 3:** Write `ThemeState.cs` — `string Theme`, toggle, `event Action? Changed`.
- [ ] **Step 4:** Write `CounterView.razor` — inject + subscribe + `StateHasChanged` on event (use `@implements IDisposable`).
- [ ] **Step 5:** Write `CartView.razor`, `ThemeProvider.razor` (cascade `Theme`).
- [ ] **Step 6:** Modify `Program.cs`: `builder.Services.AddScoped<CounterState>();` etc.
- [ ] **Step 7:** Write `StateFlow.razor` page with prose diagram of the flow.
- [ ] **Step 8:** Build → 0/0.
- [ ] **Step 9:** Commit: `feat(Part04): scoped state container + cascade + subscribe pattern`.

---

## Task 4: Part05 — Navigation App Shell

**Files (in `src/LearnBlazor/Blazor_Part05_NavigationAppShell/`):**
- Create: `Components/Layouts/MainLayout.razor` (+`.razor.css`), `Components/Layouts/NavMenu.razor`
- Create: `Components/Pages/About.razor`, `Items.razor`, `ItemDetail.razor`, `NotFound.razor`
- Modify: `Components/App.razor` (use `MainLayout` via `Routes`/`RouteView DefaultLayout`), `Routes.razor` (NotFound content), `Home.razor`

- [ ] **Step 1:** Write `MainLayout.razor` — `@inherits LayoutComponentBase`, `<header>`+`<NavMenu/>`+`<main>@Body</main>`.
- [ ] **Step 2:** Write `NavMenu.razor` — `NavLink` with `ActiveClass`, aria-current.
- [ ] **Step 3:** Write `About.razor` (`@page "/about"` + `[SupplyParameterFromQuery] string? q`), `Items.razor`/`ItemDetail.razor` (`@page "/items/{Id:int}"`), `NotFound.razor`.
- [ ] **Step 4:** Modify `Routes.razor` to add `<NotFound>` with `Home` link + `<FocusOnNavigate>`.
- [ ] **Step 5:** Modify `Home.razor` links.
- [ ] **Step 6:** Build → 0/0.
- [ ] **Step 7:** Commit: `feat(Part05): router + MainLayout + NavMenu + query/route params + 404`.

---

## Task 5: Part06 — Forms / input / keyboard / touch / files

**Files (in `src/LearnBlazor/Blazor_Part06_FormsInputTouch/`):**
- Create: `Models/Contact.cs` (validation attributes), `Models/UploadResult.cs`
- Create: `Components/CustomInputText.razor` (inherits `InputBase<string>`)
- Create: `Components/Pages/ContactForm.razor`, `FileUpload.razor`, `TouchTargets.razor`
- Modify: `_Imports.razor` (add `Microsoft.AspNetCore.Components.Forms`)

- [ ] **Step 1:** Write `Models/Contact.cs` with `[Required]`/`[EmailAddress]`/`[StringLength(100, MinimumLength=2)]`.
- [ ] **Step 2:** Write `ContactForm.razor` — `EditForm Model=@contact OnValidSubmit`, `DataAnnotationsValidator`, `ValidationSummary`, `InputText`/`InputTextArea`/`InputSelect`, success message.
- [ ] **Step 3:** Write `FileUpload.razor` — `InputFile` multiple, antiforgery (`@attribute [AntiforgeryValidation]` or form), size/type checks, preview list.
- [ ] **Step 4:** Write `CustomInputText.razor` — `@inherits InputBase<string>`, override `BuildRenderAttributes`/`TryParseValueFromString`.
- [ ] **Step 5:** Write `TouchTargets.razor` — buttons with `min-width/height: 44px` prose.
- [ ] **Step 6:** `Home.razor` index.
- [ ] **Step 7:** Build → 0/0.
- [ ] **Step 8:** Commit: `feat(Part06): EditForm validation + InputFile + custom input + touch targets`.

---

## Task 6: Part07 — Lists / media / consumer UI

**Files (in `src/LearnBlazor/Blazor_Part07_ListsMediaUI/`):**
- Create: `Services/ItemService.cs` (10k fake items via `Enumerable.Range`)
- Create: `Components/Icon.razor` (inline SVG with `[Parameter] string Name`)
- Create: `Components/Pages/VirtualList.razor`, `CardGrid.razor`, `MediaPlaceholder.razor`

- [ ] **Step 1:** Write `ItemService.cs` — `GetItems(int count)` returning `List<Item>`; `Item` record `Id`/`Title`/`Sub`/`Icon`.
- [ ] **Step 2:** Write `Icon.razor` — switch on Name returning inline `<svg>` with `currentColor`.
- [ ] **Step 3:** Write `VirtualList.razor` — `<Virtualize Context="item" ItemsProvider="@provider" OverscanCount="10">` with placeholder + `@key`.
- [ ] **Step 4:** Write `CardGrid.razor` — `display:grid; grid-template-columns:repeat(auto-fill,minmax(9.5rem,1fr))` + `@key`.
- [ ] **Step 5:** Write `MediaPlaceholder.razor` — `<img loading="lazy" />` + CSS blur-up placeholder.
- [ ] **Step 6:** `Program.cs` register `ItemService` scoped.
- [ ] **Step 7:** `Home.razor` index. Build → 0/0.
- [ ] **Step 8:** Commit: `feat(Part07): Virtualize + card grid + Icon + lazy media`.

---

## Task 7: Part08 — Accessibility & i18n

**Files (in `src/LearnBlazor/Blazor_Part08_AccessibilityI18n/`):**
- Create: `Resources/Pages/Hello.resx`, `Resources/Pages/Hello.zh-CN.resx` (or `.en-US`/`.zh-CN`)
- Create: `Components/CultureSelector.razor`, `Components/SkipLink.razor`, `Components/FocusTrap.razor`
- Create: `Components/Pages/A11yDemo.razor`, `I18nDemo.razor`
- Modify: `Program.cs` (`AddLocalization` + `RequestLocalizationOptions`)

- [ ] **Step 1:** Create `Resources/Pages/Hello.resx` (default en) with `Title`/`Greeting`/`Button`; create `Hello.zh-CN.resx` with Chinese values.
- [ ] **Step 2:** Write `CultureSelector.razor` — `IStringLocalizer<Hello>` inject, select culture, store in cookie via `HttpContext`/`NavigationManager`+JS (use cookie middleware).
- [ ] **Step 3:** Modify `Program.cs`: `builder.Services.AddLocalization();` + `app.UseRequestLocalization("en","zh-CN")`.
- [ ] **Step 4:** Write `SkipLink.razor` (`<a href="#main" class="sr-only sr-only-focusable">Skip to content</a>`).
- [ ] **Step 5:** Write `FocusTrap.razor` — modal with `@keydown` Tab/Shift+Tab cycling (no JS needed for simple trap).
- [ ] **Step 6:** Write `A11yDemo.razor` (landmark/aria-current/sr-only prose) + `I18nDemo.razor` (`@inject IStringLocalizer<Hello> L` + `@L["Greeting"]`).
- [ ] **Step 7:** `Home.razor` index. Build → 0/0 (note: resx files need `<EmbeddedResource>` — Razor SDK auto-includes).
- [ ] **Step 8:** Commit: `feat(Part08): ARIA + focus trap + IStringLocalizer + culture selector`.

---

## Task 8: Part09 — Component library & styling (RCL)

**Files:**
- Create new RCL project: `src/LearnBlazor/Blazor_Part09_ComponentLibStyling.RazorLib/Blazor_Part09_ComponentLibStyling.RazorLib.csproj`, `Components/Button.razor`(+`.css`), `Components/Card.razor`(+`.css`), `Components/Badge.razor`(+`.css`), `Components/ThemeProvider.razor`, `wwwroot/css/tokens.css`, `_Imports.razor`
- Modify: `src/LearnBlazor/Blazor_Part09_ComponentLibStyling/Blazor_Part09_ComponentLibStyling.csproj` (`<ProjectReference>` to RCL)
- Create: `src/LearnBlazor/Blazor_Part09_ComponentLibStyling/Components/Pages/ConsumeLib.razor`
- Modify: `LearnCSharp.CI.slnx` (add RCL project), `_Imports.razor`, `Home.razor`

- [ ] **Step 1:** Create `Blazor_Part09_ComponentLibStyling.RazorLib.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <AssemblyName>Blazor_Part09_ComponentLibStyling.RazorLib</AssemblyName>
    <RootNamespace>LearnBlazor.Blazor_Part09_ComponentLibStyling.RazorLib</RootNamespace>
    <IsPackable>true</IsPackable>
  </PropertyGroup>
</Project>
```
- [ ] **Step 2:** Write `wwwroot/css/tokens.css` (copy of Part02 tokens — authoritative for the lib).
- [ ] **Step 3:** Write `Components/Button.razor` + `.razor.css` — `[Parameter] Variant`, `Disabled`, `ChildContent`; uses `var(--color-*)`.
- [ ] **Step 4:** Write `Components/Card.razor`+`.css`, `Components/Badge.razor`+`.css`, `Components/ThemeProvider.razor` (cascade).
- [ ] **Step 5:** Write `_Imports.razor` in RCL with `@using LearnBlazor.Blazor_Part09_ComponentLibStyling.RazorLib.Components`.
- [ ] **Step 6:** Add `<ProjectReference Include="..\Blazor_Part09_ComponentLibStyling.RazorLib\...">` to main Part09 csproj.
- [ ] **Step 7:** Write `ConsumeLib.razor` — `<link href="_content/Blazor_Part09_ComponentLibStyling.RazorLib/css/tokens.css" />` + consume `<Button/>`/`<Card/>`/`<Badge/>`.
- [ ] **Step 8:** Add to `LearnCSharp.CI.slnx` under `/src/LearnBlazor/` folder:
```xml
    <Project Path="src/LearnBlazor/Blazor_Part09_ComponentLibStyling.RazorLib/Blazor_Part09_ComponentLibStyling.RazorLib.csproj" />
```
- [ ] **Step 9:** `_Imports.razor` add RCL namespace. `Home.razor` index. Build → 0/0.
- [ ] **Step 10:** Commit: `feat(Part09): RCL component library + CSS isolation + token sharing`.

---

## Task 9: Part10 — JS interop (WebView content skipped)

**Files (in `src/LearnBlazor/Blazor_Part10_JsInteropWebView/`):**
- Create: `wwwroot/js/interop.js` (ES module)
- Create: `Components/JsInterop.razor`, `Components/JsCallback.razor`, `Components/Pages/InteropDemo.razor`, `CspDemo.razor`
- Modify: `Components/_Imports.razor`, `Program.cs` (CSP header middleware)

- [ ] **Step 1:** Write `wwwroot/js/interop.js`:
```js
export function focusTrap(el) { /* keydown handler for Tab cycling */ }
export function observeViewport(dotnet, el) { /* IntersectionObserver -> dotnet.invokeMethodAsync */ }
export function getElementHeight(selector) { return document.querySelector(selector)?.offsetHeight ?? 0; }
```
- [ ] **Step 2:** Write `JsInterop.razor` — `@inject IJSRuntime JS`, `OnAfterRenderAsync` import `./js/interop.js`, call `getElementHeight`, dispose `IJSObjectReference`.
- [ ] **Step 3:** Write `JsCallback.razor` — `DotNetObjectReference.Create(this)`, `[JSInvokable] public void OnVisible()`, dispose.
- [ ] **Step 4:** Write `InteropDemo.razor` combining + prose on disposal / `JSDisconnectedException`.
- [ ] **Step 5:** Write `CspDemo.razor` — prose + `MarkupString` XSS warning example + a CSP meta tag.
- [ ] **Step 6:** `Program.cs` add `app.Use(async (ctx,next)=>{ ctx.Response.Headers.ContentSecurityPolicy = "default-src 'self'"; await next(); });` (dev: report-only to not break Blazor framework scripts — use `Content-Security-Policy-Report-Only`).
- [ ] **Step 7:** `Home.razor` index. Build → 0/0.
- [ ] **Step 8:** Commit: `feat(Part10): IJSRuntime/ES module/DotNetObjectReference/Dispose/CSP demos`.

---

## Task 10: Part11 — Skip (MAUI only)

**Files:** none modified beyond `Home.razor` note.

- [ ] **Step 1:** Modify `src/LearnBlazor/Blazor_Part11_NativePlatformBridge/Components/Pages/Home.razor` to:
```razor
@page "/"
<PageTitle>Part11 - Native Platform Bridge (skipped)</PageTitle>
<h1>Part11_NativePlatformBridge</h1>
<p>第 11 部分（原生平台桥接）为 MAUI Hybrid 专属：相机/分享/SecureStorage/MAUI 权限/
<code>BlazorWebView.TryDispatchAsync</code> 在纯 Web 前端无对应概念，按设计跳过。占位工程保持可编译。</p>
```
- [ ] **Step 2:** Build → 0/0.
- [ ] **Step 3:** Commit: `docs(Part11): mark as skipped (MAUI-only, pure-Web n/a)`.

---

## Task 11: Part12 — HTTP client (RPC adapted)

**Files:**
- Create new backend: `src/LearnBlazor/Blazor_Part12_CodeFirstRpcClient.Api/Blazor_Part12_CodeFirstRpcClient.Api.csproj`, `Program.cs` (Minimal API), `appsettings.json`, `Models/Item.cs`, `Services/ItemStore.cs`
- Create in frontend (`Blazor_Part12_CodeFirstRpcClient/`): `Services/ItemsClient.cs`, `Models/Item.cs`, `Contracts/ApiError.cs`, `Components/Pages/ItemsPage.razor`
- Modify: frontend `Program.cs`, `Blazor_Part12_CodeFirstRpcClient.csproj` (no project ref — backend runs separately), `LearnCSharp.CI.slnx`, `Home.razor`

- [ ] **Step 1:** Create backend csproj (`Microsoft.NET.Sdk.Web`, RootNamespace `LearnBlazor.Blazor_Part12_Api`). Add `appsettings.json` with fixed port `http://localhost:5199` + CORS AllowAny.
- [ ] **Step 2:** Write backend `Models/Item.cs` (record `Item(int Id, string Title, bool Done)`), `Services/ItemStore.cs` (in-memory list + seed 50 items).
- [ ] **Step 3:** Write backend `Program.cs`: `GET /api/items`, `GET /api/items/{id}`, `POST /api/items`, `GET /api/items/error` (throws for resilience demo), `GET /api/items/slow` (Task.Delay 2s); `AddCors` AllowAnyOrigin; `MapHealthChecks`.
- [ ] **Step 4:** Write frontend `Models/Item.cs` (mirror record), `Contracts/ApiError.cs` (enum `None`/`Network`/`Timeout`/`NotFound`/`Server`).
- [ ] **Step 5:** Write frontend `Services/ItemsClient.cs` — `HttpClient _http`; `GetAllAsync(CancellationToken)` returning `Result<IReadOnlyList<Item>, ApiError>`; try/catch `HttpRequestException`/`TaskCanceledException` mapping to `ApiError`.
- [ ] **Step 6:** Modify frontend `Program.cs`: `builder.Services.AddHttpClient<ItemsClient>(c => c.BaseAddress = new(builder.Configuration["Api:Base"]!)) .AddStandardResilienceHandler();` + `AddStandardResilienceHandler` from `Microsoft.Extensions.Http.Resilience`.
- [ ] **Step 7:** Write `ItemsPage.razor` — UI state machine (`enum State { Loading, Content, Empty, Error }`), `@inject ItemsClient`, fetch on `OnInitializedAsync`, retry button on Error.
- [ ] **Step 8:** Add backend to `LearnCSharp.CI.slnx`:
```xml
    <Project Path="src/LearnBlazor/Blazor_Part12_CodeFirstRpcClient.Api/Blazor_Part12_CodeFirstRpcClient.Api.csproj" />
```
- [ ] **Step 9:** `Home.razor` index. Build slnx → 0/0.
- [ ] **Step 10:** Commit: `feat(Part12): HTTP client + resilience + error mapping + UI state machine + minimal API backend`.

---

## Task 12: Part13 — Auth security boundary

**Files (in `src/LearnBlazor/Blazor_Part13_AuthSecurityBoundary/`):**
- Create: `Auth/SimpleAuthStateProvider.cs`, `Auth/AuthService.cs`, `Auth/User.cs`, `Models/LoginModel.cs`
- Create: `Components/Pages/Login.razor`, `Account.razor` (`[Authorize]`), `Admin.razor` (`[Authorize(Roles="admin")]`), `AuthStatus.razor`
- Modify: `Program.cs` (cookie auth), `Components/App.razor` (`AuthorizeRouteView`), `Routes.razor`, `Components/_Imports.razor`

- [ ] **Step 1:** Write `User.cs` (record `User(string Name, string[] Roles, ClaimsPrincipal ToPrincipal())`), `LoginModel.cs` (`[Required]` username/password).
- [ ] **Step 2:** Write `AuthService.cs` — in-memory users (`alice/admin`, `bob/user`), `LoginAsync`/`LogoutAsync` using `HttpContext.SignInAsync` cookie scheme; `bool IsAdmin`.
- [ ] **Step 3:** Write `SimpleAuthStateProvider.cs` — extends `AuthenticationStateProvider`, `GetAuthenticationStateAsync` reads `HttpContext.User`, raises `NotifyAuthenticationStateChanged` on change.
- [ ] **Step 4:** Modify `Program.cs`: `AddAuthentication(Cookie) AddAuthorization`, `AddScoped<AuthenticationStateProvider, SimpleAuthStateProvider>`, `app.UseAuthentication UseAuthorization`.
- [ ] **Step 5:** Write `Login.razor` (`EditForm` posting to `Login` endpoint-style — use `AuthService.LoginAsync`), `Account.razor` (`@attribute [Authorize]`, show username/roles), `Admin.razor` (`@attribute [Authorize(Roles="admin")]`), `AuthStatus.razor` (`<AuthorizeView>`).
- [ ] **Step 6:** Modify `App.razor`/`Routes.razor` to use `<AuthorizeRouteView>` + `<NotFound>`.
- [ ] **Step 7:** `_Imports.razor` add `Microsoft.AspNetCore.Components.Authorization`. `Home.razor` index.
- [ ] **Step 8:** Build → 0/0.
- [ ] **Step 9:** Commit: `feat(Part13): AuthStateProvider + AuthorizeRouteView + cookie auth + route guard`.

---

## Task 13: Part14 — Testing & debugging (bUnit)

**Files:**
- Create new test project: `src/LearnBlazor/Blazor_Part14_TestingDebugging.Tests/Blazor_Part14_TestingDebugging.Tests.csproj`, `CounterTest.cs`, `FormTest.cs`, `NavTest.cs`, `_Imports.razor` (global usings)
- Create in main `Blazor_Part14_TestingDebugging/`: `Components/Counter.razor` (fixture component), `Components/SimpleForm.razor`, `Components/Pages/ErrorBoundaryDemo.razor`, `LoggingDemo.razor`, `DiagnosticsDemo.razor`
- Modify: `LearnCSharp.CI.slnx` (add test project), main `_Imports.razor`, `Home.razor`

- [ ] **Step 1:** Create test csproj:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <RootNamespace>LearnBlazor.blazor_Part14_Tests</RootNamespace>
    <AssemblyName>Blazor_Part14_Tests</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit.v3" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="coverlet.collector" />
    <PackageReference Include="bunit.web" />
    <PackageReference Include="bunit.core" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\Blazor_Part03_RazorComponents\Blazor_Part03_RazorComponents.csproj" />
    <ProjectReference Include="..\Blazor_Part14_TestingDebugging\Blazor_Part14_TestingDebugging.csproj" />
  </ItemGroup>
</Project>
```
(Note: CPVM has these versions. bUnit version added in Task 0.)
- [ ] **Step 2:** Write `CounterTest.cs` using `using var ctx = new TestContext(); ctx.RenderComponent<Counter>(p => p.Add(c => c.Start, 5))` → find button → click → markup contains "6".
- [ ] **Step 3:** Write `FormTest.cs` — render `SimpleForm`, input + submit, assert success message.
- [ ] **Step 4:** Write `NavTest.cs` — `ctx.Services.AddSingleton<FakeNavigationManager>()`? Use bUnit `NavigationManager` — assert `NavigateTo` called.
- [ ] **Step 5:** In main Part14 project write `Counter.razor` (`[Parameter] int Start`), `SimpleForm.razor`, `ErrorBoundaryDemo.razor` (`<ErrorBoundary>`), `LoggingDemo.razor` (`@inject ILogger<...>`), `DiagnosticsDemo.razor` (render count + `ShouldRender` override).
- [ ] **Step 6:** Add test project to `LearnCSharp.CI.slnx`:
```xml
    <Project Path="src/LearnBlazor/Blazor_Part14_TestingDebugging.Tests/Blazor_Part14_TestingDebugging.Tests.csproj" />
```
- [ ] **Step 7:** `Home.razor` index. Build slnx → 0/0.
- [ ] **Step 8:** `dotnet test src/LearnBlazor/Blazor_Part14_TestingDebugging.Tests` → pass.
- [ ] **Step 9:** Commit: `feat(Part14): bUnit tests + ErrorBoundary + logging + diagnostics demos`.

---

## Task 14: Part15 — Performance & diagnostics

**Files (in `src/LearnBlazor/Blazor_Part15_PerformanceDiagnostics/`):**
- Create: `Services/StreamService.cs` (`IAsyncEnumerable<string>`), `Components/ManualComponent.cs` (implements `IComponent`)
- Create: `Components/Pages/StreamRenderingDemo.razor` (`[StreamRendering]`), `VirtualReuseDemo.razor`, `ReduceRenderDemo.razor`, `ManualRenderDemo.razor`
- Modify: `Program.cs` (response compression + static asset cache headers)

- [ ] **Step 1:** Write `StreamService.cs` — `async IAsyncEnumerable<string> GetItemsAsync([EnumeratorCancellation] Ct)` yielding 20 strings with 200ms delay.
- [ ] **Step 2:** Write `StreamRenderingDemo.razor` — `@attribute [StreamRendering(true)]`, `await foreach` in `OnInitializedAsync`? No — use `@inject StreamService` + render placeholder + yield via `IAsyncEnumerable`.
- [ ] **Step 3:** Write `VirtualReuseDemo.razor` — `<Virtualize>` over 10k items.
- [ ] **Step 4:** Write `ReduceRenderDemo.razor` — `ShouldRender` override with throttling, `@bind:after`, render count display.
- [ ] **Step 5:** Write `ManualComponent.cs` — implement `IComponent` (`Attach RenderHandle`, `SetParametersAsync`, `BuildRenderTree`).
- [ ] **Step 6:** Modify `Program.cs`: `app.UseResponseCompression()` + static files cache header middleware.
- [ ] **Step 7:** `Home.razor` index. Build → 0/0.
- [ ] **Step 8:** Commit: `feat(Part15): stream rendering + virtualization reuse + ShouldRender + manual IComponent`.

---

## Task 15: Part16 — Packaging & engineering (Web publish)

**Files (in `src/LearnBlazor/Blazor_Part16_PackagingEngineering/`):**
- Create: `Dockerfile`, `appsettings.Production.json`, `docs/PUBLISH.md` (project-local)
- Create: `Components/Pages/PublishNotes.razor`
- Modify: `Blazor_Part16_PackagingEngineering.csproj` (Release props), `Program.cs` (health check + cache headers + `UseExceptionHandler`), `appsettings.json`

- [ ] **Step 1:** Modify csproj add Release-specific `<PropertyGroup Condition="'$(Configuration)'=='Release'">` with `<Optimize>true</Optimize>` `<DebugType>none</DebugType>` `<ServerGarbageCollection>true</ServerGarbageCollection>` (as runtimeconfig).
- [ ] **Step 2:** Write `appsettings.Production.json` (logging Warning, allowed hosts).
- [ ] **Step 3:** Write `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Blazor_Part16_PackagingEngineering.csproj -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Blazor_Part16_PackagingEngineering.dll"]
```
- [ ] **Step 4:** Modify `Program.cs`: `AddHealthChecks()` + `MapHealthChecks("/healthz")` + `app.UseExceptionHandler("/error")` + static files with `OnPrepareResponse` cache headers.
- [ ] **Step 5:** Write `PublishNotes.razor` — prose on `dotnet publish -c Release -o publish`, Trimming/AOT tradeoffs, static assets, Docker.
- [ ] **Step 6:** Write `docs/PUBLISH.md` with `dotnet publish` + `docker build` + env vars.
- [ ] **Step 7:** `Home.razor` index. Build → 0/0.
- [ ] **Step 8:** Commit: `feat(Part16): publish config + Dockerfile + health checks + cache headers + publish notes`.

---

## Task 16: Final verification & push

- [ ] **Step 1:** Full build: `dotnet build LearnCSharp.CI.slnx -c Debug` → 0 warnings 0 errors.
- [ ] **Step 2:** Release build sanity: `dotnet build LearnCSharp.CI.slnx -c Release -v q` → 0/0.
- [ ] **Step 3:** Run Part14 tests: `dotnet test src/LearnBlazor/Blazor_Part14_TestingDebugging.Tests` → all pass.
- [ ] **Step 4:** Verify slnx includes new 3 sub-projects (RCL, API, Tests).
- [ ] **Step 5:** Squash check — `git log --oneline feat/learnblazor-fill ^main` shows per-part commits (keep, don't squash — repo style uses single squash-merge at PR time).
- [ ] **Step 6:** Push: `git push -u origin feat/learnblazor-fill`.
- [ ] **Step 7:** Report PR URL via `gh pr create --base main --fill` (or just give user branch/URL).

---

## Self-Review

**Spec coverage:** All 14 filled Parts (02-10,12-16) mapped to Tasks 1-15; Part11 skip = Task 10; 3 new sub-projects = Tasks 8/11/13. ✓
**Placeholders:** None — every code step has concrete code or explicit reference to spec section. ✓
**Type consistency:** `CounterState.Changed` event, `Item(Id,Title,Done)` record, `ApiError` enum used consistently. ✓