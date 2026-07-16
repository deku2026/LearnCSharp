// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第4部分-测试.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section04_Testing
// Item     : DotnetTestWorkflow
// Topic id : stage10/section04/dotnet_test_workflow
//
// dotnet test 工作流、过滤、覆盖率概念、CI 片段（不调真实测试宿主）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section04;

internal static class DotnetTestWorkflow
{
    [LearnTopic("stage10/section04/dotnet_test_workflow")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DotnetTestWorkflow ===");
        DemoBasicCommands();
        DemoFiltering();
        DemoCoverageConcept();
        DemoCiSnippet();
        DemoExitCodes();
        return 0;
    }

    private static void DemoBasicCommands()
    {
        Console.WriteLine("-- basic commands --");
        string[] cmds =
        [
            "dotnet test",
            "dotnet test -c Release",
            "dotnet test --no-build",
            "dotnet test MyApp.Tests.csproj",
            "dotnet test --logger \"console;verbosity=detailed\"",
            "dotnet test --results-directory ./TestResults",
        ];
        foreach (string c in cmds)
            Console.WriteLine($"  $ {c}");
        Debug.Assert(cmds[0] == "dotnet test");
    }

    private static void DemoFiltering()
    {
        Console.WriteLine("-- filtering --");
        Console.WriteLine("  VSTest/MSTest/NUnit 常见: --filter FullyQualifiedName~Cart");
        Console.WriteLine("  --filter Name=Add_EmptyCart_ReturnsZero");
        Console.WriteLine("  --filter TestCategory=Integration / Trait");
        Console.WriteLine("  xUnit v3 + MTP: 过滤语法以当前文档为准（与经典 --filter 可能不同）");
        string[] samples =
        [
            "--filter FullyQualifiedName~OrderService",
            "--filter Category=Unit",
        ];
        foreach (string s in samples)
            Console.WriteLine($"  $ dotnet test {s}");
        Debug.Assert(samples.All(s => s.Contains("filter", StringComparison.Ordinal)));
    }

    private static void DemoCoverageConcept()
    {
        Console.WriteLine("-- coverage (concept) --");
        Console.WriteLine("  coverlet / 内置收集器统计行/分支是否执行到");
        Console.WriteLine("  例: dotnet test /p:CollectCoverage=true（coverlet.msbuild）");
        Console.WriteLine("  MTP 侧有覆盖率扩展；输出 cobertura/opencover 等");
        Console.WriteLine("  ⚠ 高覆盖 ≠ 好测试；关注关键路径与断言质量");
        // 微型覆盖率心智：3 分支测了 2
        int branches = 3;
        int hit = 2;
        double pct = 100.0 * hit / branches;
        Debug.Assert(pct is > 60 and < 70);
        Console.WriteLine($"  demo branch coverage: {hit}/{branches} = {pct:0}%");
    }

    private static void DemoCiSnippet()
    {
        Console.WriteLine("-- CI sketch (GitHub Actions style) --");
        string[] yaml =
        [
            "- name: Test",
            "  run: dotnet test -c Release --logger trx --results-directory TestResults",
            "- name: Upload results",
            "  uses: actions/upload-artifact@v4",
            "  with:",
            "    name: test-results",
            "    path: TestResults",
        ];
        foreach (string line in yaml)
            Console.WriteLine($"  {line}");
        Debug.Assert(yaml.Any(l => l.Contains("dotnet test", StringComparison.Ordinal)));
    }

    private static void DemoExitCodes()
    {
        Console.WriteLine("-- exit codes (educational runner) --");
        var results = new[] { ("pass", true), ("pass", true), ("fail", false) };
        int failed = results.Count(r => !r.Item2);
        int exit = failed == 0 ? 0 : 1;
        Debug.Assert(exit == 1);
        Console.WriteLine($"  {results.Length} tests, {failed} failed → process exit {exit}");
        Console.WriteLine("  CI 依赖非零退出码阻断流水线");
    }
}
