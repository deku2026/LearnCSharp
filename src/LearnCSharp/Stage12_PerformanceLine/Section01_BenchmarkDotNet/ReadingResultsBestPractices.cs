// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第1部分-BenchmarkDotNet.md
// Stage    : Stage12_PerformanceLine
// Section  : Section01_BenchmarkDotNet
// Item     : ReadingResultsBestPractices
// Topic id : stage12/section01/reading_results_best_practices
//
// Lesson: Mean/Error/StdDev/Median/Ratio, outliers, micro-bench vs real bottlenecks.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section01;

internal static class ReadingResultsBestPractices
{
    [LearnTopic("stage12/section01/reading_results_best_practices")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReadingResultsBestPractices ===");
        DemoResultColumns();
        DemoTinyStats();
        DemoBestPractices();
        return 0;
    }

    private static void DemoResultColumns()
    {
        Console.WriteLine("-- reading BDN summary columns --");
        Console.WriteLine("  Mean: average time per op after warmup/overhead handling.");
        Console.WriteLine("  Error: half-width of confidence interval (roughly).");
        Console.WriteLine("  StdDev: spread; high StdDev → noisy machine or non-stable code.");
        Console.WriteLine("  Median: robust center when outliers exist.");
        Console.WriteLine("  Ratio: vs Baseline (0.5 ≈ twice as fast as baseline).");
        Console.WriteLine("  Allocated: bytes/op from MemoryDiagnoser (0 is a strong claim).");
        Console.WriteLine("  Warnings: outliers, multimodality, high variance — re-run / quiet machine.");
    }

    private static void DemoTinyStats()
    {
        Console.WriteLine("-- micro multi-sample (toy; not BDN quality) --");
        double[] samples = new double[12];
        for (int s = 0; s < samples.Length; s++)
        {
            long sink = 0;
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 2000; i++)
                sink += i * 3L;
            sw.Stop();
            samples[s] = sw.Elapsed.TotalMilliseconds;
            Debug.Assert(sink > 0);
        }

        Array.Sort(samples);
        double mean = samples.Average();
        double median = (samples[5] + samples[6]) / 2.0;
        double variance = samples.Select(x => (x - mean) * (x - mean)).Average();
        double std = Math.Sqrt(variance);
        Console.WriteLine($"  n={samples.Length} mean={mean:F4} ms median={median:F4} std={std:F4}");
        Console.WriteLine("  BDN adds outlier rules, more iters, process isolation, better CI.");
    }

    private static void DemoBestPractices()
    {
        Console.WriteLine("-- best practices --");
        Console.WriteLine("  1) Profile real app first (Stage12 §2); BDN the hot path, not random code.");
        Console.WriteLine("  2) Release, quiet machine, fixed power plan, close browsers when possible.");
        Console.WriteLine("  3) Return values / Consume to defeat DCE; GlobalSetup outside timing.");
        Console.WriteLine("  4) Parameterize N; algorithms can invert ranking as size grows.");
        Console.WriteLine("  5) Micro-bench ≠ production: IO, locks, GC, cache, network still dominate.");
        Console.WriteLine("  6) Prefer statistical significance + effect size over 1% noise.");
    }
}
