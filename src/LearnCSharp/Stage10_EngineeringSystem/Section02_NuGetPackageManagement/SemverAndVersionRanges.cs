// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第2部分-NuGet包管理.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section02_NuGetPackageManagement
// Item     : SemverAndVersionRanges
// Topic id : stage10/section02/semver_and_version_ranges
//
// SemVer、版本范围、浮出版本、同一包只解析一个版本。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section02;

internal static class SemverAndVersionRanges
{
    [LearnTopic("stage10/section02/semver_and_version_ranges")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SemverAndVersionRanges ===");
        DemoSemVerParts();
        DemoVersionRanges();
        DemoFloatingVersions();
        DemoOneVersionRule();
        DemoParseVersion();
        return 0;
    }

    private static void DemoSemVerParts()
    {
        Console.WriteLine("-- SemVer MAJOR.MINOR.PATCH --");
        Console.WriteLine("  MAJOR: 破坏性变更");
        Console.WriteLine("  MINOR: 向后兼容功能");
        Console.WriteLine("  PATCH: 向后兼容修复");
        Console.WriteLine("  预发布: 1.0.0-beta.1；构建元数据: 1.0.0+gitsha");
        var v = new Version(4, 2, 1);
        Debug.Assert(v.Major == 4 && v.Minor == 2 && v.Build == 1);
        Console.WriteLine($"  System.Version sample: {v}");
    }

    private static void DemoVersionRanges()
    {
        Console.WriteLine("-- NuGet version ranges --");
        (string Spec, string Meaning)[] ranges =
        [
            ("1.2.3", "最低 1.2.3（常用写法，实际是 >= 在解析规则下）"),
            ("[1.2.3]", "精确 1.2.3"),
            ("[1.0,2.0)", ">=1.0 且 <2.0"),
            ("(,1.0]", "<=1.0"),
            ("[1.0,)", ">=1.0"),
        ];
        foreach (var (spec, meaning) in ranges)
            Console.WriteLine($"  {spec,-12} {meaning}");
        Debug.Assert(ranges.Any(r => r.Spec.StartsWith('[')));
    }

    private static void DemoFloatingVersions()
    {
        Console.WriteLine("-- floating versions (use carefully) --");
        Console.WriteLine("  1.0.* → 最新 1.0.x patch");
        Console.WriteLine("  1.*   → 最新 1.x");
        Console.WriteLine("  *     → 最新（极不推荐于库）");
        Console.WriteLine("  应用可偶尔 float；库与 CI 更宜锁死或 CPM + lockfile");
        string floatSpec = "1.2.*";
        Debug.Assert(floatSpec.EndsWith('*'));
    }

    private static void DemoOneVersionRule()
    {
        Console.WriteLine("-- one version per package id in a graph --");
        // 模拟：A 要 Newtonsoft 12，B 要 13 → 提升到满足约束的单一版本
        var requests = new Dictionary<string, Version>
        {
            ["LibA"] = new Version(12, 0, 0),
            ["LibB"] = new Version(13, 0, 1),
        };
        Version resolved = requests.Values.Max()!;
        Debug.Assert(resolved == new Version(13, 0, 1));
        Console.WriteLine($"  requests 12.0 and 13.0.1 → graph unifies to {resolved}");
        Console.WriteLine("  冲突无法满足时 restore 失败；可用绑定重定向（旧 Framework）或升级约束");
    }

    private static void DemoParseVersion()
    {
        Console.WriteLine("-- parse & compare --");
        bool ok1 = Version.TryParse("9.0.1", out Version? a);
        bool ok2 = Version.TryParse("9.0.10", out Version? b);
        Debug.Assert(ok1 && ok2 && a is not null && b is not null);
        Debug.Assert(b > a);
        Console.WriteLine($"  {a} < {b} ? {a < b}");
        Console.WriteLine("  NuGet 完整 SemVer 比 System.Version 更丰富（预发布排序）");
    }
}
