// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第1部分-SDK项目与MSBuild构建.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section01_SdkProjectAndMSBuild
// Item     : SdkStyleProjectStructure
// Topic id : stage10/section01/sdk_style_project_structure
//
// SDK-style .csproj：声明式 + 默认 glob，替代旧式显式 Compile 列表。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section01;

internal static class SdkStyleProjectStructure
{
    [LearnTopic("stage10/section01/sdk_style_project_structure")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SdkStyleProjectStructure ===");
        DemoMinimalCsproj();
        DemoSdkAttributeMeaning();
        DemoGlobVsExplicitCompile();
        DemoAvailableSdks();
        DemoRuntimeAssemblyEvidence();
        return 0;
    }

    private static void DemoMinimalCsproj()
    {
        Console.WriteLine("-- minimal modern .csproj --");
        string[] lines =
        [
            """<Project Sdk="Microsoft.NET.Sdk">""",
            "  <PropertyGroup>",
            "    <OutputType>Exe</OutputType>",
            "    <TargetFramework>net10.0</TargetFramework>",
            "    <LangVersion>14.0</LangVersion>",
            "    <Nullable>enable</Nullable>",
            "    <ImplicitUsings>enable</ImplicitUsings>",
            "  </PropertyGroup>",
            "</Project>",
        ];
        foreach (string line in lines)
            Console.WriteLine($"  {line}");
        Debug.Assert(lines.Any(l => l.Contains("Microsoft.NET.Sdk", StringComparison.Ordinal)));
        Debug.Assert(lines.Any(l => l.Contains("net10.0", StringComparison.Ordinal)));
        Console.WriteLine("  要点: 无 <Compile Include>；SDK 默认把项目目录 **/*.cs 通配进来");
    }

    private static void DemoSdkAttributeMeaning()
    {
        Console.WriteLine("-- Sdk= attribute injects MSBuild logic --");
        (string Sdk, string Role)[] sdks =
        [
            ("Microsoft.NET.Sdk", "console / classlib 默认"),
            ("Microsoft.NET.Sdk.Web", "ASP.NET Core"),
            ("Microsoft.NET.Sdk.Worker", "后台 Worker"),
            ("Microsoft.NET.Sdk.Razor", "Razor 类库"),
        ];
        foreach (var (sdk, role) in sdks)
            Console.WriteLine($"  {sdk} → {role}");
        Debug.Assert(sdks[0].Sdk.EndsWith(".Sdk", StringComparison.Ordinal));
        Console.WriteLine("  一句 Sdk 属性 ≈ 导入一整套默认 targets（编译/还原/发布）");
    }

    private static void DemoGlobVsExplicitCompile()
    {
        Console.WriteLine("-- SDK-style vs legacy explicit Compile --");
        Console.WriteLine("  legacy: 每个 .cs 都要 <Compile Include=\"File.cs\" /> → 合并冲突多");
        Console.WriteLine("  SDK: 自动 glob；可用 <Compile Remove=\"Generated/*.cs\" /> 排除");
        Console.WriteLine("  包: packages.config → PackageReference；引用 HintPath 大幅消失");
        string[] painPoints = ["explicit Compile list", "HintPath hell", "packages.config dual source"];
        Debug.Assert(painPoints.Length == 3);
        Console.WriteLine($"  SDK-style 解决的痛点数: {painPoints.Length}");
    }

    private static void DemoAvailableSdks()
    {
        Console.WriteLine("-- 🔶 C++ 对照 --");
        Console.WriteLine("  .csproj ≈ CMakeLists，但 MSBuild 直接执行脚本（无 generate 一步）");
        Console.WriteLine("  默认 glob 源文件：CMake 官方不推荐 file(GLOB)，.NET SDK 则推荐");
        Console.WriteLine("  无头文件：assembly 自带 metadata，不用 .h/.cpp 双轨");
        Debug.Assert("assembly".Length > 0);
    }

    private static void DemoRuntimeAssemblyEvidence()
    {
        Console.WriteLine("-- 本进程程序集证据（SDK 产物） --");
        Assembly asm = typeof(SdkStyleProjectStructure).Assembly;
        Console.WriteLine($"  AssemblyName={asm.GetName().Name}");
        Console.WriteLine($"  Location={(string.IsNullOrEmpty(asm.Location) ? "(empty/single-file)" : Path.GetFileName(asm.Location))}");
        Console.WriteLine($"  Framework={System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        Debug.Assert(asm.GetName().Name is not null);
    }
}
