// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第3部分-质量门禁.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section03_QualityGate
// Item     : RoslynAnalyzers
// Topic id : stage10/section03/roslyn_analyzers
//
// Roslyn 分析器：编译管线内静态检查；内置 CA 与包分析器。

using System.Collections.Immutable;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section03;

internal static class RoslynAnalyzers
{
    [LearnTopic("stage10/section03/roslyn_analyzers")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== RoslynAnalyzers ===");
        DemoWhatIsAnalyzer();
        DemoEnableNetAnalyzers();
        DemoHighValueCaRules();
        DemoThirdPartyAnalyzers();
        DemoHowAnalyzerThinks();
        return 0;
    }

    private static void DemoWhatIsAnalyzer()
    {
        Console.WriteLine("-- analyzer = Roslyn extension in the compiler pipeline --");
        Console.WriteLine("  输入: 语法树 + 语义模型（不是跑你的 Main）");
        Console.WriteLine("  输出: Diagnostic（ID/严重性/位置/消息）");
        Console.WriteLine("  可带 CodeFix 提供一键修复");
        string[] pipeline = ["parse", "bind", "analyze", "emit"];
        Debug.Assert(pipeline.Contains("analyze"));
        Console.WriteLine($"  pipeline: {string.Join(" → ", pipeline)}");
    }

    private static void DemoEnableNetAnalyzers()
    {
        Console.WriteLine("-- enable .NET analyzers --");
        Console.WriteLine("  SDK 已内置 Microsoft.CodeAnalysis.NetAnalyzers");
        Console.WriteLine("  <EnableNETAnalyzers>true</EnableNETAnalyzers>");
        Console.WriteLine("  <AnalysisLevel>latest-recommended</AnalysisLevel>");
        Console.WriteLine("  AnalysisLevel 也可: 8.0 / latest / preview 等");
        string[] props = ["EnableNETAnalyzers", "AnalysisLevel", "AnalysisMode"];
        Debug.Assert(props.Length == 3);
    }

    private static void DemoHighValueCaRules()
    {
        Console.WriteLine("-- high-value CA examples --");
        (string Id, string Theme)[] rules =
        [
            ("CA1062", "公共 API 参数 null 检查"),
            ("CA2000", "释放 IDisposable"),
            ("CA2016", "转发 CancellationToken"),
            ("CA1848", "LoggerMessage 源生成（性能）"),
            ("CA1859", "具体类型优于接口（性能提示）"),
            ("CA1860", "优先 Count/Length 而非 Any()"),
            ("CA1822", "可标记 static 的成员"),
        ];
        foreach ((string? id, string? theme) in rules)
            Console.WriteLine($"  {id}: {theme}");
        Debug.Assert(rules.All(r => r.Id.StartsWith("CA", StringComparison.Ordinal)));
    }

    private static void DemoThirdPartyAnalyzers()
    {
        Console.WriteLine("-- third-party analyzer packages --");
        Console.WriteLine("  以 PackageReference 引入，PrivateAssets=all 防传递");
        string[] pkgs = ["StyleCop.Analyzers", "Meziantou.Analyzer", "Roslynator.Analyzers", "SonarAnalyzer.CSharp"];
        foreach (string p in pkgs)
            Console.WriteLine($"  • {p}");
        Debug.Assert(pkgs.Length >= 3);
        Console.WriteLine("  规则过多会吵：先 recommended，再逐步升 error");
    }

    private static void DemoHowAnalyzerThinks()
    {
        Console.WriteLine("-- educational: rule-like check without Roslyn API --");
        // 模拟一条“公共方法参数不应为 null”的运行时断言风格检查
        string input = "ada";
        string result = PublicApi(input);
        Debug.Assert(result == "ADA");
        try
        {
            _ = PublicApi(null!);
            Debug.Fail("should throw");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"  guard fired: {ex.ParamName}");
            Debug.Assert(ex.ParamName == "value");
        }

        ImmutableArray<string> ids = ["CA1062", "CA2000"];
        Debug.Assert(ids.Length == 2);
        Console.WriteLine("  真分析器在编译期报告，不必等单测跑到这条路径");
    }

    private static string PublicApi(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.ToUpperInvariant();
    }
}
