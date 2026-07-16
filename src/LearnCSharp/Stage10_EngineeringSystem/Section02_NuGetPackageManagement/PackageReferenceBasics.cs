// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第2部分-NuGet包管理.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section02_NuGetPackageManagement
// Item     : PackageReferenceBasics
// Topic id : stage10/section02/package_reference_basics
//
// PackageReference：声明依赖，restore 解析图；≠ 头文件复制。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section02;

internal static class PackageReferenceBasics
{
    [LearnTopic("stage10/section02/package_reference_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== PackageReferenceBasics ===");
        DemoDeclarePackage();
        DemoNotHeaderFiles();
        DemoRestoreGraph();
        DemoReadRealNuGetConfig();
        DemoProjectReferenceVsPackage();
        DemoCli();
        return 0;
    }

    private static void DemoDeclarePackage()
    {
        Console.WriteLine("-- declare a package --");
        string[] xml =
        [
            "<ItemGroup>",
            "  <PackageReference Include=\"Serilog\" Version=\"4.0.0\" />",
            "  <PackageReference Include=\"System.Text.Json\" Version=\"9.0.0\" />",
            "</ItemGroup>",
        ];
        foreach (string line in xml)
            Console.WriteLine($"  {line}");
        Debug.Assert(xml.Any(l => l.Contains("PackageReference", StringComparison.Ordinal)));
        Console.WriteLine("  Include=包 ID；Version=直接依赖版本（CPM 下可省略）");
    }

    private static void DemoReadRealNuGetConfig()
    {
        Console.WriteLine("-- real NuGet.config in this repo --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string path = Path.Combine(root, "NuGet.config");
        string text = File.ReadAllText(path);
        Debug.Assert(text.Contains("packageSources", StringComparison.OrdinalIgnoreCase));
        Debug.Assert(text.Contains("nuget.org", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"  path={path}; has nuget.org source");
    }

    private static string? FindRepoRoot()
    {
        foreach (string start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? dir = new(start);
            while (dir is not null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "NuGet.config"))
                    && File.Exists(Path.Combine(dir.FullName, "global.json")))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }

        return null;
    }

    private static void DemoNotHeaderFiles()
    {
        Console.WriteLine("-- packages are not header copies --");
        Console.WriteLine("  C++: #include 文本展开；链接还要 .lib/.a");
        Console.WriteLine("  C#: PackageReference → 引用程序集 + 传递依赖 + 可选分析器/原生资产");
        Console.WriteLine("  元数据在 DLL 里；编译器按引用解析类型，不粘贴源码");
        Assembly mscorlib = typeof(object).Assembly;
        Console.WriteLine($"  例: typeof(object).Assembly = {mscorlib.GetName().Name}");
        Debug.Assert(mscorlib.GetName().Name is not null);
    }

    private static void DemoRestoreGraph()
    {
        Console.WriteLine("-- restore resolves a graph --");
        // 微型依赖图演示
        var direct = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["App"] = "Serilog 4.0",
            ["Serilog"] = "Serilog.Sinks.Console 5.0 (transitive example)",
        };
        foreach (var kv in direct)
            Console.WriteLine($"  {kv.Key} → {kv.Value}");
        Console.WriteLine("  产物: project.assets.json（obj/）描述闭合依赖图");
        Console.WriteLine("  全局缓存: ~/.nuget/packages（可复用，不进 Git）");
        Debug.Assert(direct.ContainsKey("App"));
    }

    private static void DemoProjectReferenceVsPackage()
    {
        Console.WriteLine("-- ProjectReference vs PackageReference --");
        Console.WriteLine("  ProjectReference: 同源仓库项目，一起 build");
        Console.WriteLine("  PackageReference: 已打包的 nupkg，按版本还原");
        Console.WriteLine("  FrameworkReference: 共享框架（如 Microsoft.AspNetCore.App）");
        string[] kinds = ["ProjectReference", "PackageReference", "FrameworkReference"];
        Debug.Assert(kinds.Length == 3);
    }

    private static void DemoCli()
    {
        Console.WriteLine("-- CLI --");
        string[] cmds =
        [
            "dotnet add package Serilog",
            "dotnet remove package Serilog",
            "dotnet list package",
            "dotnet list package --outdated",
            "dotnet restore",
        ];
        foreach (string c in cmds)
            Console.WriteLine($"  $ {c}");
        Debug.Assert(cmds[0].StartsWith("dotnet add", StringComparison.Ordinal));
    }
}
