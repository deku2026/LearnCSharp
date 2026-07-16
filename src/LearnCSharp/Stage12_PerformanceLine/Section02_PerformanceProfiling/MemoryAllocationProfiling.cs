// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第2部分-性能分析与诊断.md
// Stage    : Stage12_PerformanceLine
// Section  : Section02_PerformanceProfiling
// Item     : MemoryAllocationProfiling
// Topic id : stage12/section02/memory_allocation_profiling
//
// Lesson: counters/gcdump/allocation tracing; observe alloc rate with managed APIs.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section02;

internal static class MemoryAllocationProfiling
{
    [LearnTopic("stage12/section02/memory_allocation_profiling")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MemoryAllocationProfiling ===");
        DemoToolCommands();
        DemoObserveAllocations();
        DemoLeakDiffIdea();
        return 0;
    }

    private static void DemoToolCommands()
    {
        Console.WriteLine("-- memory tools --");
        Console.WriteLine("  dotnet-counters monitor -p <pid>");
        Console.WriteLine("    watch: allocation-rate, gen-0/1/2-gc-count, gc-heap-size, working-set");
        Console.WriteLine("  dotnet-gcdump collect -p <pid>  // two dumps → diff growing types");
        Console.WriteLine("  Allocation traces: VS .NET Object Allocation / trace alloc providers");
        Console.WriteLine("  BDN [MemoryDiagnoser] for micro-level Allocated/op (Stage12 §1).");
    }

    private static void DemoObserveAllocations()
    {
        Console.WriteLine("-- managed observation APIs --");
        long before = GC.GetTotalAllocatedBytes(precise: true);
        int gen0Before = GC.CollectionCount(0);
        List<byte[]> keep = new(32);
        for (int i = 0; i < 32; i++)
            keep.Add(new byte[4096]);
        long after = GC.GetTotalAllocatedBytes(precise: true);
        int gen0After = GC.CollectionCount(0);
        Debug.Assert(keep.Count == 32);
        Console.WriteLine($"  allocated Δ≈{after - before} bytes (expected ≥ 32*4096)");
        Console.WriteLine($"  Gen0 collections during demo: {gen0After - gen0Before}");
        Console.WriteLine($"  TotalMemory≈{GC.GetTotalMemory(false)}");
        // prevent DCE of keep
        Debug.Assert(keep[0].Length == 4096);
    }

    private static void DemoLeakDiffIdea()
    {
        Console.WriteLine("-- leak investigation pattern --");
        Console.WriteLine("  T1 gcdump → exercise → T2 gcdump → compare type counts/sizes.");
        Console.WriteLine("  Growing type + retention path → find who roots it (event, static, cache).");
        Console.WriteLine("  High allocation-rate without growth = churn (fix with pooling/Span).");
        WeakReference wr = new(new object());
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"  WeakReference.IsAlive after GC={wr.IsAlive} (illustrates reachability)");
    }
}
