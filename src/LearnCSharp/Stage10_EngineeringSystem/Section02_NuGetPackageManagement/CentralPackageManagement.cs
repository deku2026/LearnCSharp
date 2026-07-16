// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第2部分-NuGet包管理.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section02_NuGetPackageManagement
// Item     : CentralPackageManagement
// Topic id : stage10/section02/central_package_management
//
// CPM：Directory.Packages.props 集中版本 + 传递固定。

using System.Diagnostics;
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
        DemoDirectoryPackagesProps();
        DemoProjectSideReference();
        DemoTransitivePinning();
        DemoRules();
        return 0;
    }

    private static void DemoVersionScatterProblem()
    {
        Console.WriteLine("-- problem: versions scattered in every csproj --");
        var scatter = new Dictionary<string, string>
        {
            ["App.csproj"] = "Newtonsoft.Json 13.0.1",
            ["Tests.csproj"] = "Newtonsoft.Json 12.0.3",
            ["Worker.csproj"] = "Newtonsoft.Json 13.0.3",
        };
        foreach (var kv in scatter)
            Console.WriteLine($"  {kv.Key}: {kv.Value}");
        int distinct = scatter.Values.Distinct(StringComparer.Ordinal).Count();
        Debug.Assert(distinct == 3);
        Console.WriteLine($"  distinct versions in repo: {distinct} → 升级噩梦");
    }

    private static void DemoDirectoryPackagesProps()
    {
        Console.WriteLine("-- Directory.Packages.props --");
        string[] lines =
        [
            "<Project>",
            "  <PropertyGroup>",
            "    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>",
            "    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>",
            "  </PropertyGroup>",
            "  <ItemGroup>",
            "    <PackageVersion Include=\"Newtonsoft.Json\" Version=\"13.0.3\" />",
            "    <PackageVersion Include=\"xunit\" Version=\"2.9.0\" />",
            "  </ItemGroup>",
            "</Project>",
        ];
        foreach (string line in lines)
            Console.WriteLine($"  {line}");
        Debug.Assert(lines.Any(l => l.Contains("PackageVersion", StringComparison.Ordinal)));
    }

    private static void DemoProjectSideReference()
    {
        Console.WriteLine("-- project only declares id (no Version) --");
        Console.WriteLine("  <PackageReference Include=\"Newtonsoft.Json\" />");
        Console.WriteLine("  版本来自中央 PackageVersion；本地写 Version 会报错（除非覆盖策略允许）");
        string refLine = """<PackageReference Include="Newtonsoft.Json" />""";
        Debug.Assert(!refLine.Contains("Version=", StringComparison.Ordinal));
    }

    private static void DemoTransitivePinning()
    {
        Console.WriteLine("-- transitive pinning --");
        Console.WriteLine("  直接依赖 A 拉进传递包 T 的“浮动”版本 → 难审计");
        Console.WriteLine("  CentralPackageTransitivePinningEnabled: 也可在 Packages.props 钉 T");
        Console.WriteLine("  效果: 传递依赖版本进入中央清单，升级可感知");
        var central = new HashSet<string>(StringComparer.Ordinal)
        {
            "Newtonsoft.Json",
            "System.Text.Json", // also pin transitive if needed
        };
        Debug.Assert(central.Count >= 2);
        Console.WriteLine($"  pinned package ids (demo): {string.Join(", ", central)}");
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
}
