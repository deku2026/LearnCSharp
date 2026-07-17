// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第5部分-JIT编译与分层编译.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section05_JITAndTieredCompilation
// Item     : QuickJitOsr
// Topic id : stage11/section05/quick_jit_osr
//
// Lesson: QuickJIT for Tier0 loops; OSR replaces long-running loop mid-execution.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section05;

internal static class QuickJitOsr
{
    [LearnTopic("stage11/section05/quick_jit_osr")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== QuickJitOsr ===");
        DemoQuickJitAndOsrNotes();
        DemoLongLoopMultiRun();
        return 0;
    }

    private static void DemoQuickJitAndOsrNotes()
    {
        Console.WriteLine("-- Quick JIT + OSR --");
        Console.WriteLine("  Tier0: faster, less-optimized codegen (DOTNET_TC_QuickJitForLoops).");
        Console.WriteLine("  OSR: long-running loop can be replaced with optimized code mid-method.");
        Console.WriteLine("  Critical for benchmarks/servers with long-lived hot loops.");
    }

    private static void DemoLongLoopMultiRun()
    {
        Console.WriteLine("-- long loop multi-run (OSR candidate) --");
        // Warm
        long warm = TightLoop(50_000);
        Debug.Assert(warm > 0);

        double[] samples = new double[5];
        long acc = 0;
        for (int r = 0; r < samples.Length; r++)
        {
            long t0 = Stopwatch.GetTimestamp();
            acc = TightLoop(300_000);
            samples[r] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
        }

        Array.Sort(samples);
        Console.WriteLine($"  TightLoop(300000) acc={acc}, median={samples[2]:F3}ms");
        Debug.Assert(acc > 0);
        Debug.Assert(samples[2] > 0 || samples[2] == 0);
        Console.WriteLine("  Observe OSR with PerfView / DOTNET_JitDisasm in real labs.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long TightLoop(int n)
    {
        long acc = 0;
        for (int i = 0; i < n; i++)
            acc += i ^ (i << 1);
        return acc;
    }
}
