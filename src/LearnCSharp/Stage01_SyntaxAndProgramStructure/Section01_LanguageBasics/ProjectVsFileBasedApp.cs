// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : ProjectVsFileBasedApp
// Topic id : stage01/section01/project_vs_file_based_app
//
// 步骤 1：.csproj 项目 vs .NET 10 单文件 file-based app。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class ProjectVsFileBasedApp
{
    [LearnTopic("stage01/section01/project_vs_file_based_app")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ProjectVsFileBasedApp ===");
        DemoProjectBasedModel();
        DemoFileBasedAppModel();
        DemoCppMentalModel();
        DemoCliCommands();
        return 0;
    }

    private static void DemoProjectBasedModel()
    {
        Console.WriteLine("-- project-based (.csproj) --");
        // SDK-style 项目：声明式，不写编译/链接命令
        string[] csprojLines =
        [
            """<Project Sdk="Microsoft.NET.Sdk">""",
            "  <PropertyGroup>",
            "    <OutputType>Exe</OutputType>",
            "    <TargetFramework>net10.0</TargetFramework>",
            "    <ImplicitUsings>enable</ImplicitUsings>",
            "    <Nullable>enable</Nullable>",
            "  </PropertyGroup>",
            "</Project>",
        ];

        foreach (string line in csprojLines)
            Console.WriteLine($"  {line}");

        Debug.Assert(csprojLines.Any(l => l.Contains("Microsoft.NET.Sdk")));
        Debug.Assert(csprojLines.Any(l => l.Contains("net10.0")));
        Console.WriteLine("  要点: 一组 .cs → 一个程序集(assembly)，元数据自带，无需头文件");
    }

    private static void DemoFileBasedAppModel()
    {
        Console.WriteLine("-- file-based app (.NET 10) --");
        // 无 .csproj，dotnet run app.cs；顶部 #: 指令声明依赖/属性
        string[] directives =
        [
            "#:package Spectre.Console@*",
            "#:property TargetFramework=net10.0",
            "#:sdk Microsoft.NET.Sdk.Web",
        ];

        foreach (string d in directives)
            Console.WriteLine($"  {d}");

        Console.WriteLine("  源码本体可只有: Console.WriteLine(\"hi\");");
        Console.WriteLine("  转换: dotnet project convert app.cs → 正式 .csproj 项目");
        Console.WriteLine("  ⚠ #:package 省略版本需 Directory.Packages.props，否则写版本或 @*");

        Debug.Assert(directives[0].StartsWith("#:package", StringComparison.Ordinal));
        Debug.Assert(directives.All(d => d.StartsWith("#:", StringComparison.Ordinal)));
    }

    private static void DemoCppMentalModel()
    {
        Console.WriteLine("-- 🔶 C++ 对照 --");
        Console.WriteLine("  .csproj ≈ CMakeLists.txt / .vcxproj，但更声明式");
        Console.WriteLine("  无头文件、无独立链接步骤；程序集自带 metadata");
        Console.WriteLine("  PackageReference + NuGet ≈ vcpkg/Conan，深度集成进 dotnet CLI");
        Console.WriteLine("  file-based app ≈ g++ a.cpp && ./a.out，但还能声明 NuGet / 换 SDK");

        string assemblyUnit = "assembly";
        Debug.Assert(assemblyUnit is "assembly");
    }

    private static void DemoCliCommands()
    {
        Console.WriteLine("-- 常用 CLI --");
        string[] commands =
        [
            "dotnet new console -n Hello",
            "dotnet run",
            "dotnet run app.cs",
            "echo 'Console.WriteLine(\"hi\");' | dotnet run -",
            "dotnet project convert app.cs",
            "dotnet build / dotnet publish",
        ];

        foreach (string cmd in commands)
            Console.WriteLine($"  $ {cmd}");

        Debug.Assert(commands.Length >= 5);
        Debug.Assert(commands.Any(c => c.Contains("file-based") || c.Contains("app.cs")));
    }
}
