// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第1部分-SDK项目与MSBuild构建.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section01_SdkProjectAndMSBuild
// Item     : DirectoryBuildPropsTargets
// Topic id : stage10/section01/directory_build_props_targets
//
// 目录级共享：Directory.Build.props / .targets + Packages.props — 读本仓库真实文件。

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
        DemoReadRealDirectoryBuildProps();
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

    private static void DemoReadRealDirectoryBuildProps()
    {
        Console.WriteLine("-- read real Directory.Build.props in this repo --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null, "repo root with Directory.Build.props not found");
        string path = Path.Join(root, "Directory.Build.props");
        string text = File.ReadAllText(path);
        Debug.Assert(text.Contains("TargetFramework", StringComparison.Ordinal));
        Debug.Assert(text.Contains("net10.0", StringComparison.Ordinal));
        Debug.Assert(text.Contains("LangVersion", StringComparison.Ordinal));
        Debug.Assert(text.Contains("Nullable", StringComparison.Ordinal));
        Debug.Assert(text.Contains("ManagePackageVersionsCentrally", StringComparison.Ordinal));
        Console.WriteLine($"  path={path}");
        Console.WriteLine($"  length={text.Length}; contains net10.0 + LangVersion + CPM flag");
        Console.WriteLine("  从项目目录向上查找最近的 Directory.Build.props 并 import");
    }

    private static void DemoDirectoryBuildTargets()
    {
        Console.WriteLine("-- Directory.Build.targets (auto-imported late) --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string path = Path.Join(root, "Directory.Build.targets");
        if (File.Exists(path))
        {
            string text = File.ReadAllText(path);
            Debug.Assert(text.Contains("Project", StringComparison.OrdinalIgnoreCase));
            Console.WriteLine($"  found Directory.Build.targets length={text.Length}");
        }
        else
        {
            Console.WriteLine("  (no Directory.Build.targets in this repo — optional file)");
        }

        Console.WriteLine("  props = 默认属性；targets = 默认目标/任务钩子");
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
    }

    private static void DemoPackagesPropsTeaser()
    {
        Console.WriteLine("-- Directory.Packages.props (CPM) — real file --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string path = Path.Join(root, "Directory.Packages.props");
        string text = File.ReadAllText(path);
        Debug.Assert(text.Contains("ManagePackageVersionsCentrally", StringComparison.Ordinal));
        Debug.Assert(text.Contains("PackageVersion", StringComparison.Ordinal));
        Console.WriteLine($"  PackageVersion entries present; length={text.Length}");
    }

    private static string? FindRepoRoot()
    {
        foreach (string start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? dir = new(start);
            while (dir is not null)
            {
                if (File.Exists(Path.Join(dir.FullName, "Directory.Build.props"))
                    && File.Exists(Path.Join(dir.FullName, "global.json")))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }

        return null;
    }
}
