// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第2部分-NuGet包管理.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section02_NuGetPackageManagement
// Item     : CentralPackageManagement
// Topic id : stage10/section02/central_package_management
//
// CPM：Directory.Packages.props 集中版本 — 读本仓库真实文件。

using System.Diagnostics;
using System.Text.RegularExpressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section02;

internal static class CentralPackageManagement
{
    [LearnTopic("stage10/section02/central_package_management")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CentralPackageManagement ===");
        DemoVersionScatterProblem();
        DemoReadRealPackagesProps();
        DemoProjectSideReference();
        DemoTransitivePinning();
        DemoRules();
        return 0;
    }

    private static void DemoVersionScatterProblem()
    {
        Console.WriteLine("-- problem: versions scattered in every csproj --");
        Dictionary<string, string> scatter = new Dictionary<string, string>
        {
            ["App.csproj"] = "Newtonsoft.Json 13.0.1",
            ["Tests.csproj"] = "Newtonsoft.Json 12.0.3",
            ["Worker.csproj"] = "Newtonsoft.Json 13.0.3",
        };
        foreach (KeyValuePair<string, string> kv in scatter)
            Console.WriteLine($"  {kv.Key}: {kv.Value}");
        int distinct = scatter.Values.Distinct(StringComparer.Ordinal).Count();
        Debug.Assert(distinct == 3);
        Console.WriteLine($"  distinct versions in repo: {distinct} → 升级噩梦");
    }

    private static void DemoReadRealPackagesProps()
    {
        Console.WriteLine("-- real Directory.Packages.props in this repo --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string path = Path.Join(root, "Directory.Packages.props");
        string text = File.ReadAllText(path);
        Debug.Assert(text.Contains("ManagePackageVersionsCentrally", StringComparison.Ordinal));
        MatchCollection versions = Regex.Matches(text, @"PackageVersion\s+Include=""([^""]+)""");
        Debug.Assert(versions.Count >= 1);
        Console.WriteLine($"  path={path}");
        Console.WriteLine($"  PackageVersion count={versions.Count}");
        foreach (Match m in versions.Take(5))
            Console.WriteLine($"    - {m.Groups[1].Value}");
    }

    private static void DemoProjectSideReference()
    {
        Console.WriteLine("-- project PackageReference without Version (CPM) --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string csproj = Path.Join(root, "src", "LearnCSharp", "LearnCSharp.csproj");
        string text = File.ReadAllText(csproj);
        Debug.Assert(text.Contains("PackageReference", StringComparison.Ordinal));
        Debug.Assert(text.Contains("Microsoft.Extensions.DependencyInjection", StringComparison.Ordinal));
        // CPM: PackageReference lines should not carry Version=
        MatchCollection refs = Regex.Matches(text, @"<PackageReference\s+Include=""[^""]+""\s*/>");
        Debug.Assert(refs.Count >= 1);
        Console.WriteLine($"  LearnCSharp.csproj PackageReference (no Version) count={refs.Count}");
    }

    private static void DemoTransitivePinning()
    {
        Console.WriteLine("-- transitive pinning concept --");
        Console.WriteLine("  CentralPackageTransitivePinningEnabled: 也可在 Packages.props 钉传递包");
        HashSet<string> central = new HashSet<string>(StringComparer.Ordinal)
        {
            "Microsoft.Extensions.Hosting",
            "Microsoft.Extensions.Http",
        };
        Debug.Assert(central.Count >= 2);
        Console.WriteLine($"  direct pins in this lesson: {string.Join(", ", central)}");
    }

    private static void DemoRules()
    {
        Console.WriteLine("-- practical rules --");
        string[] rules =
        [
            "仓库根一份 Directory.Packages.props",
            "业务 csproj 不写 Version 属性",
            "升级包只改中央文件 → 一个 PR 全仓一致",
            "与 packages.lock.json 组合 → 可复现还原",
        ];
        foreach (string r in rules)
            Console.WriteLine($"  • {r}");
        Debug.Assert(rules.Length == 4);
    }

    private static string? FindRepoRoot()
    {
        foreach (string start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? dir = new(start);
            while (dir is not null)
            {
                if (File.Exists(Path.Join(dir.FullName, "Directory.Packages.props"))
                    && File.Exists(Path.Join(dir.FullName, "global.json")))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }

        return null;
    }
}
