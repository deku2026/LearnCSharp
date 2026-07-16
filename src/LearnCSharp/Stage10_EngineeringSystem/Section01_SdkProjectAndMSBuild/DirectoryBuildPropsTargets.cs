// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第1部分-SDK项目与MSBuild构建.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section01_SdkProjectAndMSBuild
// Item     : DirectoryBuildPropsTargets
// Topic id : stage10/section01/directory_build_props_targets
//
// 目录级共享：Directory.Build.props / .targets + Packages.props 预告。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section01;

internal static class DirectoryBuildPropsTargets
{
    [LearnTopic("stage10/section01/directory_build_props_targets")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DirectoryBuildPropsTargets ===");
        DemoProblemDuplication();
        DemoDirectoryBuildProps();
        DemoDirectoryBuildTargets();
        DemoImportSearchOrder();
        DemoPackagesPropsTeaser();
        return 0;
    }

    private static void DemoProblemDuplication()
    {
        Console.WriteLine("-- problem: copy-paste properties across projects --");
        string[] duplicated =
        [
            "<Nullable>enable</Nullable>",
            "<ImplicitUsings>enable</ImplicitUsings>",
            "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>",
            "<LangVersion>14.0</LangVersion>",
        ];
        Console.WriteLine($"  若 20 个项目各写一遍 → 20 处漂移风险: {duplicated.Length} 项共性");
        Debug.Assert(duplicated.Length == 4);
        Console.WriteLine("  目标: 仓库根定义一次，子项目自动继承");
    }

    private static void DemoDirectoryBuildProps()
    {
        Console.WriteLine("-- Directory.Build.props (auto-imported early) --");
        string[] sample =
        [
            "<Project>",
            "  <PropertyGroup>",
            "    <Nullable>enable</Nullable>",
            "    <ImplicitUsings>enable</ImplicitUsings>",
            "    <LangVersion>14.0</LangVersion>",
            "    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>",
            "  </PropertyGroup>",
            "</Project>",
        ];
        foreach (string line in sample)
            Console.WriteLine($"  {line}");
        Debug.Assert(sample[0].Contains("Project", StringComparison.Ordinal));
        Console.WriteLine("  从项目目录向上查找最近的 Directory.Build.props 并 import");
        Console.WriteLine("  放在求值早期 → 项目内同名属性可覆盖仓库默认");
    }

    private static void DemoDirectoryBuildTargets()
    {
        Console.WriteLine("-- Directory.Build.targets (auto-imported late) --");
        Console.WriteLine("  适合: 共享 Target、AfterBuild 钩子、统一打包后处理");
        Console.WriteLine("  props = 默认属性；targets = 默认目标/任务钩子");
        string[] roles =
        [
            "props → PropertyGroup 默认值",
            "targets → Target 扩展构建图",
        ];
        foreach (string r in roles)
            Console.WriteLine($"  {r}");
        Debug.Assert(roles.Length == 2);
    }

    private static void DemoImportSearchOrder()
    {
        Console.WriteLine("-- import mental model --");
        string[] order =
        [
            "1. 向上找 Directory.Build.props",
            "2. 加载 SDK + 本项目 .csproj 主体",
            "3. NuGet 注入的 build/*.props|targets",
            "4. 向上找 Directory.Build.targets",
        ];
        foreach (string step in order)
            Console.WriteLine($"  {step}");
        Debug.Assert(order.Length == 4);
        Console.WriteLine("  覆盖规则: 后写属性可覆盖先写（项目可覆盖仓库 props）");
    }

    private static void DemoPackagesPropsTeaser()
    {
        Console.WriteLine("-- Directory.Packages.props teaser (CPM, part 2) --");
        Console.WriteLine("  集中 PackageVersion，项目里 PackageReference 不再写 Version");
        Console.WriteLine("  <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>");
        string marker = "Directory.Packages.props";
        Debug.Assert(marker.EndsWith(".props", StringComparison.Ordinal));
        Console.WriteLine($"  详见: {marker} + Central Package Management");
    }
}
