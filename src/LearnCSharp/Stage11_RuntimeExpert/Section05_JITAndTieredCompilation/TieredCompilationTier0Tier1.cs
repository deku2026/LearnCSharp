// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第5部分-JIT编译与分层编译.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section05_JITAndTieredCompilation
// Item     : TieredCompilationTier0Tier1
// Topic id : stage11/section05/tiered_compilation_tier0_tier1
//
// Lesson: Tier 0 quick code → call counting → Tier 1 optimized (and PGO).

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section05;

internal static class TieredCompilationTier0Tier1
{
    [LearnTopic("stage11/section05/tiered_compilation_tier0_tier1")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TieredCompilationTier0Tier1 ===");
        DemoExplainTiers();
        DemoHotMethod();
        DemoEnvKnobs();
        return 0;
    }

    private static void DemoExplainTiers()
    {
        Console.WriteLine("-- tiered compilation --");
        Console.WriteLine("  Tier 0: fast JIT, fewer opts, instrumented call counters");
        Console.WriteLine("  Tier 1: re-JIT with full opts after hotness threshold");
        Console.WriteLine("  Dynamic PGO: profile edges/types at Tier 0 → better Tier 1");
        Console.WriteLine("  Enabled by default on modern .NET Core / .NET 5+");
    }

    private static void DemoHotMethod()
    {
        Console.WriteLine("-- warm a method (may promote tiers asynchronously) --");
        long sum = 0;
        long t0 = Stopwatch.GetTimestamp();
        for (int i = 0; i < 50_000; i++)
            sum += HotWork(i);
        TimeSpan elapsed = Stopwatch.GetElapsedTime(t0);
        Debug.Assert(sum != 0);
        Console.WriteLine($"  50k calls HotWork sum={sum}, elapsed={elapsed.TotalMilliseconds:F2}ms");
        Console.WriteLine("  Promotion is async; short demos may not show Tier1 flip.");
    }

    private static void DemoEnvKnobs()
    {
        Console.WriteLine("-- configuration knobs (env / runtimeconfig) --");
        Console.WriteLine("  DOTNET_TieredCompilation=0|1");
        Console.WriteLine("  DOTNET_TC_QuickJit=0|1");
        Console.WriteLine("  DOTNET_TieredPGO=0|1");
        string? tc = Environment.GetEnvironmentVariable("DOTNET_TieredCompilation");
        Console.WriteLine($"  current DOTNET_TieredCompilation={tc ?? "(default on)"}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int HotWork(int i) => (i * 17) ^ (i >> 3);
}
