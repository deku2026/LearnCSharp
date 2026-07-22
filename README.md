# LearnCSharp · 现代 C# 14 / .NET 10 学习占位脚手架

按 `C:/MyFile/ArcForges/ArchitectureDesign/CSharpStudy/CSharp-dotNET-完整学习路线图.md`
逐阶段铺好的 **271 个空 `Run()` 占位**。后续往每个 `.cs` 里直接写真实代码 (`Console.WriteLine` /
`Debug.Assert` / 抛异常 / 玩 `args`，怎么舒服怎么写)，**没有测试框架的盒子约束**。

阶段 14 (Unity 接轨附录) 不铺占位 — 那是 Unity 仓自己的事。

## 工作原理

整仓一个可执行 `LearnCSharp`。每个占位 `.cs` 长这样：

```csharp
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class ProjectVsFileBasedApp
{
    [LearnTopic("stage01/section01/project_vs_file_based_app")]
    internal static int Run(string[] args)
    {
        _ = args;
        return 0;
    }
}
```

`[LearnTopic("id")]` 把方法标记为可调度 topic。`Program.cs` 启动时反射扫描当前 assembly，
把所有打了这个特性的 `static int Run(string[])` 方法收进 `Dictionary<string, Func<string[], int>>`。
`Main` 再把命令行参数转给 registry 调度 — **等同于直接进入那个 topic 的 `int Main(string[])`**。

```pwsh
# Debug build 无参 = 遍历全部 271 个 topic (F5 from IDE 用这条)
dotnet run --project src/LearnCSharp -c Debug

# Release/ci build 无参 = 列出全部 topic
dotnet run --project src/LearnCSharp -c Release

# 任意 build 跑某个 topic
dotnet run --project src/LearnCSharp -c Debug -- stage01/section01/project_vs_file_based_app

# 透传额外参数
dotnet run --project src/LearnCSharp -c Debug -- stage10/section04/dotnet_test_workflow extra args
```

「无参」行为按 `DEBUG` 宏分支:

- **Debug build**: 无参时 registry 遍历全 dict, 顺序调用每个 `Run()`. IDE 里 F5 不用配 args, 任何 topic 里下的断点都会命中.
- **Release**: 无参时只列出已注册 topic — 手动 pick.

调试时: IDE 的 launch profile 用 Debug build, 直接 F5; 或显式把 args 设成 topic id, F5 命中那一个 `Run()` 里的断点.

## 关键路径

```
.
├── .editorconfig / .gitattributes / .gitignore
├── Directory.Build.props          # 中央 net10.0 / LangVersion 14.0 / Nullable / CPM
├── Directory.Build.targets
├── Directory.Packages.props       # 中央包版本管理 (Central Package Management)
├── NuGet.config
├── global.json                    # 锁 SDK 版本
├── LearnCSharp.slnx               # SLNX (新式 XML solution)
├── LICENSE / README.md
└── src/
    └── LearnCSharp/
        ├── LearnCSharp.csproj
        ├── Program.cs             # 唯一 Main, 把 args 转给 registry
        ├── Topics/
        │   ├── LearnTopicAttribute.cs  # [LearnTopic("id")] 标记
        │   └── TopicRegistry.cs        # 反射扫描 + 字典 + Run / List
        ├── Stage01_SyntaxAndProgramStructure/        # 阶段 1
        ├── Stage02_TypeSystem/                       # 阶段 2 (4 部分)
        ├── Stage03_MembersAndOOP/                    # 阶段 3 (4 部分)
        ├── Stage04_ControlFlowAndPatterns/           # 阶段 4 (2 部分)
        ├── Stage05_CollectionsLINQIterators/         # 阶段 5 (3 部分)
        ├── Stage06_ExceptionsAndDiagnostics/         # 阶段 6 (2 部分)
        ├── Stage07_AsyncBasics/                      # 阶段 7 (2 部分)
        ├── Stage08_KeywordsAndCSharp14/              # 阶段 8 (2 部分)
        ├── Stage09_BCL/                              # 阶段 9 (6 部分)
        ├── Stage10_EngineeringSystem/                # 阶段 10 (5 部分)
        ├── Stage11_RuntimeExpert/                    # 阶段 11 (10 部分)
        ├── Stage12_PerformanceLine/                  # 阶段 12 (4 部分)
        └── Stage13_MetaprogrammingAndInterop/        # 阶段 13 (4 部分)
```

每个 stage 目录:

- 一份 `README.md` (该 stage 目标 + 学习节奏);
- 每个「部分」一个 `SectionNN_xxx/` 子目录, 对应路线图里的「第 N 部分」;
- 子目录内 N 个 `*.cs` 占位, 每份 = 独立类型 + 一个 `[LearnTopic]` 标注的 `Run`.

## 现状

- 语言/运行时: **C# 14 / .NET 10** (`<TargetFramework>net10.0</TargetFramework>` + `<LangVersion>14.0</LangVersion>`).
- SDK: `.NET 10 SDK 10.0.301` (锁在 `global.json`).
- 解决方案: **SLNX** (XML 新式 solution, 不再用旧 `.sln` 格式).
- 包版本: **CPM (Central Package Management)** — 所有 NuGet 版本在 `Directory.Packages.props` 集中管。
- 占位规模: **13 stage / 48 section / 271 个 `.cs`**, 每个一条 `[LearnTopic(id)] Run(string[])`.
- 不带测试框架 — 代码主体是赤裸 `int Run(string[] args)`, 用 `Console.WriteLine` / `Debug.Assert` / 抛异常 / 玩 `args` 自由发挥。

## 本地一把梭

```pwsh
# 在 worktree 里
cd C:\MyFile\LearnCSharp\.worktree\scaffold

# 编一遍
dotnet build LearnCSharp.slnx -c Debug

# 列出全部 topic
dotnet run --project src/LearnCSharp -c Release

# 跑某个 topic
dotnet run --project src/LearnCSharp -c Debug -- stage02/section01/value_vs_reference_types

# Debug 无参 = 遍历所有 topic (IDE F5 友好)
dotnet run --project src/LearnCSharp -c Debug
```

## 填一个占位

1. 选一个 `.cs` (例: `src/LearnCSharp/Stage02_TypeSystem/Section01_Foundations/BoxingAndUnboxing.cs`);
2. 顶部注释里有 Doc / Stage / Section / Item / Topic id, 翻 `C:/MyFile/ArcForges/ArchitectureDesign/CSharpStudy/<那个 md>` 看本节;
3. 配合 `https://learn.microsoft.com/dotnet/csharp/...` + `https://sharplab.io` (看 IL/lowered) + `BenchmarkDotNet` (量化) 进入循环;
4. 在 `Run(string[] args)` 里直接写代码 — `Console.WriteLine` / `Debug.Assert` / 抛异常 / 用 args 都可;
5. 跑: `dotnet run --project src/LearnCSharp -c Debug -- <topic id>`;

## 添加新占位

直接 `cp` 一个现成 `.cs` 改名, 改顶部 5 个注释字段 (Doc / Stage / Section / Item / Topic id),
改类名 + `[LearnTopic("<新 id>")]` 里的 id 字符串。下次 `dotnet build` 自动接上 — 没有 GLOB 配置,
SDK-style 项目默认把 `**/*.cs` 都编进去。

## Worktree 用法

主 checkout (`C:/MyFile/LearnCSharp`) 故意保持空 (只有 `LICENSE`), 所有占位放进 worktree 分支:

```pwsh
git -C C:\MyFile\LearnCSharp worktree list
# C:/MyFile/LearnCSharp                    [main]      <- 空, 只有 LICENSE
# C:/MyFile/LearnCSharp/.worktree/scaffold [scaffold]  <- 这里
```

要再起独立练习分支:

```pwsh
git -C C:\MyFile\LearnCSharp worktree add -b stage02-types .worktree/stage02-types scaffold
```

`.worktree/` 已加进 `.gitignore`, 所以从 scaffold branch 看 `.worktree/` 是 untracked + 被忽略,
git 自己也知道这是 worktree 路径不会误删。

## 与路线图的关系

- 本仓只放 **占位 + registry + 本地工具链**, **不在仓内**重写路线图。
- 路线图文档保留在原位 (`C:/MyFile/ArcForges/ArchitectureDesign/CSharpStudy/`), 仓内通过注释 / README 引用相对路径。

## LearnAsp · ASP.NET Core 10 实战模块

`src/LearnAsp/` 收录从 LearnAsp.Net 仓库并入的完整 ASP.NET Core 10 实战项目:
**48 个 src 项目 + 31 个测试项目 + benchmarks**, 统一加 `Asp_` 前缀, 与
`Avalonia_*` / `Blazor_*` / `Maui_*` 的平台前缀惯例一致。模块说明详见
[`src/LearnAsp/README.md`](src/LearnAsp/README.md)。

```pwsh
# CI 口径: 编译全部 (含 LearnAsp, 不含 Maui/Android)
dotnet build LearnCSharp.CI.slnx -c Release

# Aspire AppHost (Part10, 拉起 W7/W8 全套服务)
dotnet run --project src/LearnAsp/Asp_Part10_Aspire
```
## License

见 `LICENSE`。
