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
        DemoQuickJit();
        DemoOsrConcept();
        DemoLongLoop();
        return 0;
    }

    private static void DemoQuickJit()
    {
        Console.WriteLine("-- Quick JIT --");
        Console.WriteLine("  Tier 0 uses faster, less-optimized codegen to reduce pause on first call.");
        Console.WriteLine("  Loops may get special handling so startup stays snappy.");
        Console.WriteLine("  DOTNET_TC_QuickJitForLoops controls loop QuickJIT.");
    }

    private static void DemoOsrConcept()
    {
        Console.WriteLine("-- On-Stack Replacement (OSR) --");
        Console.WriteLine("  A method stuck in a long loop can be recompiled optimized");
        Console.WriteLine("  and continue from a patch point without waiting for method exit.");
        Console.WriteLine("  Critical for benchmarks/servers with long-lived hot loops.");
    }

    private static void DemoLongLoop()
    {
        Console.WriteLine("-- long loop (OSR candidate on real runtimes) --");
        long t0 = Stopwatch.GetTimestamp();
        long acc = TightLoop(200_000);
        TimeSpan e = Stopwatch.GetElapsedTime(t0);
        Debug.Assert(acc > 0);
        Console.WriteLine($"  TightLoop(200000) acc={acc}, {e.TotalMilliseconds:F2}ms");
        Console.WriteLine("  Use tools (perfview / jitdisasm) to confirm OSR in production diagnosis.");
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
