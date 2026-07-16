// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第1部分-BenchmarkDotNet.md
// Stage    : Stage12_PerformanceLine
// Section  : Section01_BenchmarkDotNet
// Item     : BenchmarkDotNetWorkings
// Topic id : stage12/section01/benchmarkdotnet_workings
//
// Lesson: BDN pipeline (Pilot/Overhead/Warmup/Workload, unroll, overhead subtract).
// Note: BenchmarkDotNet package is NOT referenced here — API shapes are educational strings.

using System.Diagnostics;
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
        DemoMicroPhasesWithStopwatch();
        return 0;
    }

    private static void DemoApiShape()
    {
        Console.WriteLine("-- BDN entry shape (package not installed; do not run full BDN here) --");
        Console.WriteLine("  // using BenchmarkDotNet.Running;");
        Console.WriteLine("  // BenchmarkRunner.Run<StringBenchmarks>();");
        Console.WriteLine("  // [MemoryDiagnoser] public class StringBenchmarks { [Benchmark] ... }");
        Console.WriteLine("  Always: dotnet run -c Release (no debugger attached).");
    }

    private static void DemoPhasePipeline()
    {
        Console.WriteLine("-- how BDN works (official pipeline) --");
        Console.WriteLine("  Launch: generate+build Release benchmark process(es).");
        Console.WriteLine("  Pilot: choose invocation count for target measurement time.");
        Console.WriteLine("  OverheadWarmup/OverheadWorkload: empty-method overhead.");
        Console.WriteLine("  ActualWarmup: JIT tiers / caches stabilize (Tier1).");
        Console.WriteLine("  ActualWorkload: real timed iterations.");
        Console.WriteLine("  UnrollFactor (default 16): amortize loop/timer overhead.");
        Console.WriteLine("  Subtract measured empty overhead → net time for your code.");
        Console.WriteLine("  Optional: MemoryDiagnoser GC alloc/collection stats.");
    }

    private static void DemoMicroPhasesWithStopwatch()
    {
        Console.WriteLine("-- toy multi-phase timing (NOT BDN; illustrates phases only) --");
        // Warmup (simulate)
        long warm = 0;
        for (int i = 0; i < 2000; i++)
            warm += i * i;
        Debug.Assert(warm > 0);

        // Empty overhead sample
        Stopwatch empty = Stopwatch.StartNew();
        for (int i = 0; i < 5000; i++)
            Empty();
        empty.Stop();

        // Workload sample
        long sink = 0;
        Stopwatch work = Stopwatch.StartNew();
        for (int i = 0; i < 5000; i++)
            sink += Payload(i);
        work.Stop();

        double emptyMs = empty.Elapsed.TotalMilliseconds;
        double workMs = work.Elapsed.TotalMilliseconds;
        double netMs = Math.Max(0, workMs - emptyMs);
        Debug.Assert(sink != 0 || sink == 0);
        Console.WriteLine($"  empty≈{emptyMs:F3} ms, work≈{workMs:F3} ms, net≈{netMs:F3} ms, sink={sink}");
        Console.WriteLine("  Real BDN: statistical multi-iter, outlier detection, process isolation.");
    }

    private static void Empty()
    {
    }

    private static long Payload(int x) => x * 31L + 7;
}
