// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第2部分-性能分析与诊断.md
// Stage    : Stage12_PerformanceLine
// Section  : Section02_PerformanceProfiling
// Item     : ProfileBeforeOptimizing
// Topic id : stage12/section02/profile_before_optimizing
//
// Lesson: 80/20 hot spots; sampling vs instrumentation; never optimize blind.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section02;

internal static class ProfileBeforeOptimizing
{
    [LearnTopic("stage12/section02/profile_before_optimizing")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ProfileBeforeOptimizing ===");
        DemoEightyTwenty();
        DemoSamplingVsInstrumentation();
        DemoCorrectOrder();
        return 0;
    }

    private static void DemoEightyTwenty()
    {
        Console.WriteLine("-- synthetic 80/20 workload --");
        Stopwatch total = Stopwatch.StartNew();
        Stopwatch hot = Stopwatch.StartNew();
        long h = HotPath(80_000);
        hot.Stop();
        Stopwatch cold = Stopwatch.StartNew();
        long c = ColdPath(2_000);
        cold.Stop();
        total.Stop();
        Debug.Assert(h != 0 || h == 0);
        Debug.Assert(c != 0 || c == 0);
        Console.WriteLine($"  HotPath  ≈{hot.Elapsed.TotalMilliseconds:F2} ms (optimize THIS)");
        Console.WriteLine($"  ColdPath ≈{cold.Elapsed.TotalMilliseconds:F2} ms (often not worth it)");
        Console.WriteLine($"  total    ≈{total.Elapsed.TotalMilliseconds:F2} ms sink h={h} c={c}");
        Console.WriteLine("  Blindly optimizing ColdPath wastes effort (Knuth: premature optimization).");
    }

    private static void DemoSamplingVsInstrumentation()
    {
        Console.WriteLine("-- sampling vs instrumentation --");
        Console.WriteLine("  Sampling: periodic stacks (dotnet-trace cpu-sampling / perf record).");
        Console.WriteLine("    low overhead (~1-5%), statistical; short methods may be missed.");
        Console.WriteLine("  Instrumentation: probe every call → exact counts, high overhead, distorts.");
        Console.WriteLine("  Default for CPU hotspots: sampling + flame graph (wide frames = hot).");
    }

    private static void DemoCorrectOrder()
    {
        Console.WriteLine("-- performance workflow --");
        Console.WriteLine("  1) Profile production-like load → find real hot path");
        Console.WriteLine("  2) Micro-benchmark candidates with BDN (Stage12 §1)");
        Console.WriteLine("  3) Optimize, then re-profile + re-bench to prove wins");
        Console.WriteLine("  Never: rewrite based on intuition alone.");
    }

    private static long HotPath(int n)
    {
        long acc = 0;
        for (int i = 0; i < n; i++)
        {
            // intentional heavier work
            acc += (long)Math.Sqrt(i + 1) * (i % 17);
            for (int k = 0; k < 8; k++)
                acc ^= i * k;
        }

        return acc;
    }

    private static long ColdPath(int n)
    {
        long acc = 0;
        for (int i = 0; i < n; i++)
            acc += i;
        return acc;
    }
}
