// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第1部分-BenchmarkDotNet.md
// Stage    : Stage12_PerformanceLine
// Section  : Section01_BenchmarkDotNet
// Item     : BenchmarkDotNetWorkings
// Topic id : stage12/section01/benchmarkdotnet_workings
//
// Lesson: BDN pipeline (Pilot/Overhead/Warmup/Workload, unroll, overhead subtract).
// Note: BenchmarkDotNet package is NOT referenced — mini harness mirrors concepts with real numbers.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section01;

internal static class BenchmarkDotNetWorkings
{
    [LearnTopic("stage12/section01/benchmarkdotnet_workings")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== BenchmarkDotNetWorkings ===");
        DemoApiShape();
        DemoPhasePipeline();
        DemoMiniHarnessWithOverheadSubtract();
        return 0;
    }

    private static void DemoApiShape()
    {
        Console.WriteLine("-- BDN entry shape (package not installed) --");
        Console.WriteLine("  BenchmarkRunner.Run<T>(); [MemoryDiagnoser] [Benchmark] ...");
        Console.WriteLine("  Always: dotnet run -c Release (no debugger).");
    }

    private static void DemoPhasePipeline()
    {
        Console.WriteLine("-- how BDN works --");
        Console.WriteLine("  Launch → Pilot → OverheadWarmup/Workload → ActualWarmup → ActualWorkload");
        Console.WriteLine("  UnrollFactor amortizes loop/timer; subtract empty overhead → net time.");
    }

    private static void DemoMiniHarnessWithOverheadSubtract()
    {
        Console.WriteLine("-- mini harness: warmup + empty overhead + workload + alloc (BDN concepts) --");
        const int iters = 8_000;
        const int samples = 6;

        // Warmup
        long warm = 0;
        for (int i = 0; i < 2_000; i++)
        {
            Empty();
            warm += Payload(i);
        }

        Debug.Assert(warm != 0 || warm == 0);

        double[] emptyMs = new double[samples];
        double[] workMs = new double[samples];
        long allocEmpty = 0, allocWork = 0;
        long sink = 0;

        for (int s = 0; s < samples; s++)
        {
            long b0 = GC.GetTotalAllocatedBytes(precise: true);
            long t0 = Stopwatch.GetTimestamp();
            for (int i = 0; i < iters; i++)
                Empty();
            emptyMs[s] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            allocEmpty += GC.GetTotalAllocatedBytes(precise: true) - b0;

            b0 = GC.GetTotalAllocatedBytes(precise: true);
            t0 = Stopwatch.GetTimestamp();
            for (int i = 0; i < iters; i++)
                sink += Payload(i);
            workMs[s] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            allocWork += GC.GetTotalAllocatedBytes(precise: true) - b0;
        }

        Array.Sort(emptyMs);
        Array.Sort(workMs);
        double emptyMed = emptyMs[samples / 2];
        double workMed = workMs[samples / 2];
        double net = Math.Max(0, workMed - emptyMed);
        Console.WriteLine($"  empty median={emptyMed:F4}ms, work median={workMed:F4}ms, net≈{net:F4}ms");
        Console.WriteLine($"  alloc empty/sample≈{allocEmpty / samples}, work/sample≈{allocWork / samples}, sink={sink}");
        Debug.Assert(workMed >= 0);
        Debug.Assert(sink != 0 || sink == 0);
        Console.WriteLine("  Real BDN: stats, outliers, process isolation, MemoryDiagnoser columns.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Empty()
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long Payload(int x) => x * 31L + 7;
}
