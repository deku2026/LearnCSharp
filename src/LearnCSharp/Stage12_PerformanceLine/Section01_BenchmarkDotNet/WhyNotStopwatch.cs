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
        DemoNaiveVsMiniHarness();
        DemoPitfallsCatalog();
        DemoDeadCodeRisk();
        return 0;
    }

    private static void DemoNaiveVsMiniHarness()
    {
        Console.WriteLine("-- naive one-shot vs mini multi-sample harness --");
        // Naive
        Stopwatch sw = Stopwatch.StartNew();
        long acc = 0;
        for (int i = 0; i < 10_000; i++)
            acc += Work(i);
        sw.Stop();
        Debug.Assert(acc > 0);
        Console.WriteLine($"  naive 10k: {sw.Elapsed.TotalMilliseconds:F3} ms, acc={acc}");

        // Mini harness: warmup + multi sample + alloc delta (BDN-like structure)
        MiniResult r = MiniBench.Run(
            warmup: 2,
            samples: 8,
            iterations: 10_000,
            action: static () =>
            {
                long a = 0;
                for (int i = 0; i < 10_000; i++)
                    a += Work(i);
                return a;
            });
        Console.WriteLine($"  mini: mean={r.MeanMs:F3}ms median={r.MedianMs:F3} std={r.StdDevMs:F3} allocΔ/sample≈{r.AllocPerSample}");
        Debug.Assert(r.MeanMs >= 0);
        Debug.Assert(r.Samples.Length == 8);
        Console.WriteLine("  Still not BDN (no process isolation / outlier rules) — but real numbers.");
    }

    private static void DemoPitfallsCatalog()
    {
        Console.WriteLine("-- why Stopwatch alone misleads --");
        Console.WriteLine("  Debug builds, tiered JIT, DCE, GC pauses, no stats, no overhead subtract.");
        Console.WriteLine($"  IsDebugAttached={Debugger.IsAttached}, ProcessorCount={Environment.ProcessorCount}");
        Debug.Assert(Environment.ProcessorCount >= 1);
    }

    private static void DemoDeadCodeRisk()
    {
        Console.WriteLine("-- DCE risk --");
        int kept = 0;
        for (int i = 0; i < 1000; i++)
            kept ^= Work(i);
        Console.WriteLine($"  kept xor={kept}");
        Debug.Assert(kept != int.MinValue || kept == int.MinValue);
        Console.WriteLine("  BDN: return value or Consumer.Consume to keep results live.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Work(int x) => (x * 17) ^ (x + 3);

    private readonly struct MiniResult(double meanMs, double medianMs, double stdDevMs, long allocPerSample, double[] samples)
    {
        public double MeanMs { get; } = meanMs;
        public double MedianMs { get; } = medianMs;
        public double StdDevMs { get; } = stdDevMs;
        public long AllocPerSample { get; } = allocPerSample;
        public double[] Samples { get; } = samples;
    }

    private static class MiniBench
    {
        public static MiniResult Run(int warmup, int samples, int iterations, Func<long> action)
        {
            for (int w = 0; w < warmup; w++)
                _ = action();

            double[] ms = new double[samples];
            long allocSum = 0;
            for (int s = 0; s < samples; s++)
            {
                long before = GC.GetTotalAllocatedBytes(precise: true);
                long t0 = Stopwatch.GetTimestamp();
                long sink = action();
                double elapsed = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
                long after = GC.GetTotalAllocatedBytes(precise: true);
                ms[s] = elapsed;
                allocSum += after - before;
                Debug.Assert(sink != long.MinValue || sink == long.MinValue);
                _ = iterations;
            }

            Array.Sort(ms);
            double mean = ms.Average();
            double median = (ms[samples / 2] + ms[(samples - 1) / 2]) / 2.0;
            double var = ms.Select(x => (x - mean) * (x - mean)).Average();
            return new MiniResult(mean, median, Math.Sqrt(var), allocSum / samples, ms);
        }
    }
}
