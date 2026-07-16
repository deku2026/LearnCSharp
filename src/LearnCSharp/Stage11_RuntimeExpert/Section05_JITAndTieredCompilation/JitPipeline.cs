// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第5部分-JIT编译与分层编译.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section05_JITAndTieredCompilation
// Item     : JitPipeline
// Topic id : stage11/section05/jit_pipeline
//
// Lesson: first call triggers JIT; pipeline import → morph → optimize → emit.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section05;

internal static class JitPipeline
{
    [LearnTopic("stage11/section05/jit_pipeline")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== JitPipeline ===");
        DemoFirstCallVsSteady();
        DemoMultiRunTiming();
        DemoNoInlineBoundary();
        DemoIsReferenceOrContainsReferences();
        return 0;
    }

    private static void DemoFirstCallVsSteady()
    {
        Console.WriteLine("-- first invocation may include JIT cost --");
        // Use a never-before-called method
        long t0 = Stopwatch.GetTimestamp();
        int a = FreshMethodA(1);
        TimeSpan first = Stopwatch.GetElapsedTime(t0);

        t0 = Stopwatch.GetTimestamp();
        int b = FreshMethodA(2);
        TimeSpan second = Stopwatch.GetElapsedTime(t0);

        Debug.Assert(a == 2 && b == 4);
        Console.WriteLine($"  FreshMethodA first≈{first.TotalMicroseconds:F1}µs, second≈{second.TotalMicroseconds:F1}µs");
        Console.WriteLine("  Pipeline: Import IL→IR → morph/inline → opts → lower/RA → emit native+GC info");
    }

    private static void DemoMultiRunTiming()
    {
        Console.WriteLine("-- multi-run steady-state timing (educational) --");
        // Warmup
        long warm = 0;
        for (int i = 0; i < 10_000; i++)
            warm += SteadyWork(i);
        Debug.Assert(warm != 0);

        const int runs = 7;
        double[] ms = new double[runs];
        long sink = 0;
        for (int r = 0; r < runs; r++)
        {
            long t0 = Stopwatch.GetTimestamp();
            for (int i = 0; i < 50_000; i++)
                sink += SteadyWork(i);
            ms[r] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
        }

        Array.Sort(ms);
        double median = ms[runs / 2];
        Console.WriteLine($"  50k SteadyWork × {runs} runs: median={median:F3}ms, min={ms[0]:F3}, max={ms[^1]:F3}, sink={sink}");
        Debug.Assert(median > 0 || sink != 0);
        Debug.Assert(ms[^1] >= ms[0]);
    }

    private static void DemoNoInlineBoundary()
    {
        Console.WriteLine("-- NoInlining forces a real call boundary --");
        int x = NoInlineAdd(10, 5);
        Debug.Assert(x == 15);
        Console.WriteLine($"  NoInlineAdd(10,5)={x}");
        Console.WriteLine("  Useful when measuring call overhead or blocking constant folding.");
    }

    private static void DemoIsReferenceOrContainsReferences()
    {
        Console.WriteLine("-- RuntimeHelpers.IsReferenceOrContainsReferences (JIT/GC layout) --");
        bool i = RuntimeHelpers.IsReferenceOrContainsReferences<int>();
        bool s = RuntimeHelpers.IsReferenceOrContainsReferences<string>();
        bool spanLike = RuntimeHelpers.IsReferenceOrContainsReferences<ValueTuple<int, string>>();
        Console.WriteLine($"  int={i}, string={s}, (int,string)={spanLike}");
        Debug.Assert(!i && s && spanLike);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int FreshMethodA(int n) => n * 2;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int SteadyWork(int i) => (i * 17) ^ (i >> 3);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NoInlineAdd(int a, int b) => a + b;
}
