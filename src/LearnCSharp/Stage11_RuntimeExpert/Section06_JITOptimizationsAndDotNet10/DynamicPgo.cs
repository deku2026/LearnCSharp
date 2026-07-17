// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第6部分-JIT优化与dotNET10专题.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section06_JITOptimizationsAndDotNet10
// Item     : DynamicPgo
// Topic id : stage11/section06/dynamic_pgo
//
// Lesson: Dynamic PGO collects edge/type profiles at Tier0 to guide Tier1 opts.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section06;

internal static class DynamicPgo
{
    [LearnTopic("stage11/section06/dynamic_pgo")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DynamicPgo ===");
        DemoWhatIsPgo();
        DemoBiasedBranchMultiRun();
        DemoConfig();
        return 0;
    }

    private static void DemoWhatIsPgo()
    {
        Console.WriteLine("-- Dynamic Profile-Guided Optimization --");
        Console.WriteLine("  Instrumentation: branch likelihood, virtual targets, block counts.");
        Console.WriteLine("  Tier1 uses profile for layout, GDV, inlining budget.");
        Console.WriteLine("  DOTNET_TieredPGO=1 (default on recent .NET)");
    }

    private static void DemoBiasedBranchMultiRun()
    {
        Console.WriteLine("-- biased branch multi-run timing --");
        // Warm
        int hits = 0;
        for (int i = 0; i < 50_000; i++)
        {
            if (Likely(i))
                hits++;
        }

        Debug.Assert(hits > 49_000);

        double[] samples = new double[6];
        long sink = 0;
        for (int r = 0; r < samples.Length; r++)
        {
            long t0 = Stopwatch.GetTimestamp();
            int h = 0;
            for (int i = 0; i < 200_000; i++)
            {
                if (Likely(i))
                    h++;
                sink += h;
            }

            samples[r] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            hits = h;
        }

        Array.Sort(samples);
        Console.WriteLine($"  Likely() hits={hits}/200000, median={samples[2]:F3}ms");
        Debug.Assert(hits > 199_000);
        Debug.Assert(sink != 0);
        Console.WriteLine("  After PGO, JIT can place hot path as fall-through.");
    }

    private static void DemoConfig()
    {
        Console.WriteLine("-- knobs + polymorphic-ish work --");
        string? pgo = Environment.GetEnvironmentVariable("DOTNET_TieredPGO");
        Console.WriteLine($"  DOTNET_TieredPGO={pgo ?? "(default)"}");
        long sum = 0;
        for (int i = 0; i < 20_000; i++)
            sum += Work(i % 2 == 0 ? "a" : "bb");
        Debug.Assert(sum > 0);
        Console.WriteLine($"  Work sum lengths={sum}");
        Console.WriteLine($"  IsReferenceOrContainsReferences<string>={RuntimeHelpers.IsReferenceOrContainsReferences<string>()}");
        Debug.Assert(RuntimeHelpers.IsReferenceOrContainsReferences<string>());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool Likely(int i) => i != 12345;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Work(string s) => s.Length;
}
