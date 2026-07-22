// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第4部分-测试.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section04_Testing
// Item     : DotnetTestWorkflow
// Topic id : stage10/section04/dotnet_test_workflow
//
// dotnet test 工作流 + 可执行 assert 演示（不强制跑真实测试宿主）。

using System.Diagnostics;
using System.Reflection;
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
        DemoCiSnippetFromRepo();
        DemoEducationalRunnerExitCodes();
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
        Debug.Assert(cmds.All(c => c.StartsWith("dotnet test", StringComparison.Ordinal)));
    }

    private static void DemoFiltering()
    {
        Console.WriteLine("-- filtering --");
        string[] samples =
        [
            "--filter FullyQualifiedName~OrderService",
            "--filter Category=Unit",
            "--filter Name=Add_EmptyCart_ReturnsZero",
        ];
        foreach (string s in samples)
            Console.WriteLine($"  $ dotnet test {s}");
        Debug.Assert(samples.All(s => s.Contains("filter", StringComparison.Ordinal)));
    }

    private static void DemoCoverageConcept()
    {
        Console.WriteLine("-- coverage (concept) --");
        int branches = 3;
        int hit = 2;
        double pct = 100.0 * hit / branches;
        Debug.Assert(pct is > 60 and < 70);
        Console.WriteLine($"  demo branch coverage: {hit}/{branches} = {pct:0}%");
        Console.WriteLine("  ⚠ 高覆盖 ≠ 好测试；关注关键路径与断言质量");
    }

    private static void DemoCiSnippetFromRepo()
    {
        Console.WriteLine("-- CI workflow files in this repo --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string workflows = Path.Join(root, ".github", "workflows");
        Debug.Assert(Directory.Exists(workflows));
        string[] yml = Directory.GetFiles(workflows, "*.yml");
        Debug.Assert(yml.Length >= 1);
        string any = File.ReadAllText(yml[0]);
        Debug.Assert(any.Length > 0);
        Console.WriteLine($"  workflow count={yml.Length}; sample={Path.GetFileName(yml[0])}");
        bool mentionsTest = yml.Select(File.ReadAllText)
            .Any(t => t.Contains("dotnet test", StringComparison.OrdinalIgnoreCase)
                      || t.Contains("dotnet build", StringComparison.OrdinalIgnoreCase));
        Debug.Assert(mentionsTest);
        Console.WriteLine($"  mentions dotnet build/test: {mentionsTest}");
    }

    private static void DemoEducationalRunnerExitCodes()
    {
        Console.WriteLine("-- educational runner exit codes --");
        (string, bool)[] results = new[] { ("pass", true), ("pass", true), ("fail", false) };
        int failed = results.Count(r => !r.Item2);
        int exit = failed == 0 ? 0 : 1;
        Debug.Assert(exit == 1);
        Console.WriteLine($"  {results.Length} tests, {failed} failed → process exit {exit}");

        // This process itself is a "suite" of demos returning 0 when healthy
        Assembly asm = typeof(DotnetTestWorkflow).Assembly;
        Debug.Assert(asm.GetName().Name == "LearnCSharp");
        Console.WriteLine($"  host assembly={asm.GetName().Name}; Environment.ProcessId={Environment.ProcessId}");
    }

    private static string? FindRepoRoot()
    {
        foreach (string start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? dir = new(start);
            while (dir is not null)
            {
                if (Directory.Exists(Path.Join(dir.FullName, ".github", "workflows"))
                    && File.Exists(Path.Join(dir.FullName, "global.json")))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }

        return null;
    }
}
