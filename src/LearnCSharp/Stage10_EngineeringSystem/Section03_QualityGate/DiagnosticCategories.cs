// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第3部分-质量门禁.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section03_QualityGate
// Item     : DiagnosticCategories
// Topic id : stage10/section03/diagnostic_categories
//
// 先分清：编译器诊断、分析器、IDE 建议、运行时错误。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section03;

internal static class DiagnosticCategories
{
    [LearnTopic("stage10/section03/diagnostic_categories")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DiagnosticCategories ===");
        DemoTaxonomy();
        DemoIdPrefixes();
        DemoSeverityLadder();
        DemoWhenEachFires();
        DemoLiveExample();
        return 0;
    }

    private static void DemoTaxonomy()
    {
        Console.WriteLine("-- diagnostic taxonomy --");
        (string Kind, string When, string Example)[] rows =
        [
            ("Compiler", "编译期语言/类型规则", "CS0161 并非所有路径返回值"),
            ("Analyzer (CA/IDE…)", "静态分析规则（Roslyn）", "CA1062 验证公共方法参数"),
            ("Code style (IDE)", "风格偏好", "IDE0007 使用 var"),
            ("Build props", "TreatWarningsAsErrors 等", "警告提升为错误"),
            ("Runtime", "跑起来才发现", "NullReferenceException"),
        ];
        foreach (var (kind, when, example) in rows)
            Console.WriteLine($"  {kind,-22} | {when,-24} | {example}");
        Debug.Assert(rows.Length == 5);
    }

    private static void DemoIdPrefixes()
    {
        Console.WriteLine("-- common ID prefixes --");
        (string Prefix, string Source)[] prefixes =
        [
            ("CS", "C# 编译器"),
            ("CA", "Microsoft 代码分析（.NET 分析器）"),
            ("IDE", "Visual Studio / Roslyn 代码风格"),
            ("SYSLIB", "BCL 过时/安全相关"),
            ("NU", "NuGet 还原/审计"),
        ];
        foreach (var (prefix, source) in prefixes)
            Console.WriteLine($"  {prefix,-8} {source}");
        Debug.Assert(prefixes.All(p => p.Prefix.Length >= 2));
    }

    private static void DemoSeverityLadder()
    {
        Console.WriteLine("-- severity ladder --");
        string[] levels = ["silent", "suggestion", "warning", "error"];
        for (int i = 0; i < levels.Length; i++)
            Console.WriteLine($"  {i}: {levels[i]}");
        Debug.Assert(levels[^1] == "error");
        Console.WriteLine("  .editorconfig 可把某规则从 suggestion 提到 error");
    }

    private static void DemoWhenEachFires()
    {
        Console.WriteLine("-- when it fires --");
        Console.WriteLine("  IDE 波浪线: 编辑时分析器 + 编译器后台");
        Console.WriteLine("  dotnet build: 编译器 + 分析器（若启用）");
        Console.WriteLine("  CI: 与本地相同命令；门禁靠 TreatWarningsAsErrors / 规则 error");
        Console.WriteLine("  运行时: 测试与监控，不是 diagnostic ID");
        Debug.Assert(true);
    }

    private static void DemoLiveExample()
    {
        Console.WriteLine("-- micro compile-time vs runtime --");
        // 编译期安全：可空注解帮助分析器/编译器
        string? maybe = GetMaybe(false);
        string safe = maybe ?? "(null)";
        Debug.Assert(safe == "(null)");
        Console.WriteLine($"  nullable-aware path: {safe}");
        Console.WriteLine("  质量门禁目标: 尽量把缺陷左移到 CS/CA，而不是生产 NRE");
    }

    private static string? GetMaybe(bool flag) => flag ? "ok" : null;
}
