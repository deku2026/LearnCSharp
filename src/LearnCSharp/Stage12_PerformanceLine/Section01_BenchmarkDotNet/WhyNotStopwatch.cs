// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第1部分-BenchmarkDotNet.md
// Stage    : Stage12_PerformanceLine
// Section  : Section01_BenchmarkDotNet
// Item     : WhyNotStopwatch
// Topic id : stage12/section01/why_not_stopwatch
//
// Lesson: ad-hoc Stopwatch micro-benchmarks lie (Debug, JIT tiers, DCE, GC, stats).

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section01;

internal static class WhyNotStopwatch
{
    [LearnTopic("stage12/section01/why_not_stopwatch")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WhyNotStopwatch ===");
        DemoNaiveStopwatch();
        DemoPitfallsCatalog();
        DemoDeadCodeRisk();
        return 0;
    }

    private static void DemoNaiveStopwatch()
    {
        Console.WriteLine("-- naive Stopwatch loop (educational only; not a real benchmark) --");
        Stopwatch sw = Stopwatch.StartNew();
        long acc = 0;
        for (int i = 0; i < 10_000; i++)
            acc += Work(i);
        sw.Stop();
        Debug.Assert(acc > 0);
        Console.WriteLine($"  10k Work calls: {sw.Elapsed.TotalMilliseconds:F3} ms, acc={acc}");
        Console.WriteLine("  One shot + no warmup + no stats → noisy and environment-biased.");
    }

    private static void DemoPitfallsCatalog()
    {
        Console.WriteLine("-- why Stopwatch alone misleads --");
        Console.WriteLine("  1) Debug builds: little optimization → different numbers than Release.");
        Console.WriteLine("  2) JIT + tiered compilation: early calls may be Tier0; later Tier1.");
        Console.WriteLine("  3) Dead-code elimination: unused results can be optimized away.");
        Console.WriteLine("  4) GC pauses: random stalls fold into the measured interval.");
        Console.WriteLine("  5) No stats: one sample has no Mean/Error/StdDev/outliers.");
        Console.WriteLine("  BDN automates: Release process, warmup, overhead subtract, multi-iter stats.");
        Console.WriteLine($"  IsDebug={Debugger.IsAttached}, ProcessorCount={Environment.ProcessorCount}");
    }

    private static void DemoDeadCodeRisk()
    {
        Console.WriteLine("-- DCE risk sketch --");
        // If result is unused, a real optimizing benchmark harness must keep it alive.
        // BDN: return the value or Consumer.Consume(...). Google Benchmark: DoNotOptimize.
        int kept = 0;
        for (int i = 0; i < 1000; i++)
            kept ^= Work(i);
        Debug.Assert(kept != int.MinValue || kept == int.MinValue);
        Console.WriteLine($"  kept side-effect xor={kept} (forces work to matter)");
        Console.WriteLine("  Rule: measure only after you can prove the code ran and results survived.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Work(int x) => (x * 17) ^ (x + 3);
}
