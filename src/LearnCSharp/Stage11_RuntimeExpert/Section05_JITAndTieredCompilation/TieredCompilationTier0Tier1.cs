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
        DemoHotMethodMultiPhase();
        DemoEnvKnobs();
        return 0;
    }

    private static void DemoExplainTiers()
    {
        Console.WriteLine("-- tiered compilation --");
        Console.WriteLine("  Tier 0: fast JIT, fewer opts, instrumented call counters");
        Console.WriteLine("  Tier 1: re-JIT with full opts after hotness threshold");
        Console.WriteLine("  Dynamic PGO: profile edges/types at Tier 0 → better Tier 1");
    }

    private static void DemoHotMethodMultiPhase()
    {
        Console.WriteLine("-- multi-phase timing of a hot method --");
        // Phase 1: cold-ish batch
        long sink = 0;
        long t0 = Stopwatch.GetTimestamp();
        for (int i = 0; i < 5_000; i++)
            sink += HotWork(i);
        double coldMs = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;

        // Phase 2: warm many calls (tier promotion may happen async)
        t0 = Stopwatch.GetTimestamp();
        for (int i = 0; i < 200_000; i++)
            sink += HotWork(i);
        double warmMs = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;

        // Phase 3: measure again
        double[] samples = new double[5];
        for (int s = 0; s < samples.Length; s++)
        {
            t0 = Stopwatch.GetTimestamp();
            for (int i = 0; i < 50_000; i++)
                sink += HotWork(i);
            samples[s] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
        }

        Array.Sort(samples);
        Console.WriteLine($"  first 5k≈{coldMs:F3}ms, next 200k≈{warmMs:F3}ms");
        Console.WriteLine($"  post-warm 50k median={samples[2]:F3}ms (min={samples[0]:F3}, max={samples[^1]:F3})");
        Console.WriteLine($"  sink={sink}");
        Debug.Assert(sink != 0);
        Debug.Assert(samples[2] >= 0);
        Console.WriteLine("  Promotion is async; short demos may not show Tier1 flip — use BDN/tools.");
    }

    private static void DemoEnvKnobs()
    {
        Console.WriteLine("-- configuration knobs --");
        Console.WriteLine("  DOTNET_TieredCompilation=0|1  DOTNET_TC_QuickJit=0|1  DOTNET_TieredPGO=0|1");
        string? tc = Environment.GetEnvironmentVariable("DOTNET_TieredCompilation");
        Console.WriteLine($"  DOTNET_TieredCompilation={tc ?? "(default on)"}");
        Console.WriteLine($"  ProcessorCount={Environment.ProcessorCount}");
        Debug.Assert(Environment.ProcessorCount >= 1);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int HotWork(int i) => (i * 17) ^ (i >> 3);
}
