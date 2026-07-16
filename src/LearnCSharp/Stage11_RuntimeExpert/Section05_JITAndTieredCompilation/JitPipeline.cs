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
        DemoFirstCallCompile();
        DemoPipelineStages();
        DemoNoInlineBoundary();
        return 0;
    }

    private static void DemoFirstCallCompile()
    {
        Console.WriteLine("-- first invocation pays JIT cost --");
        long t0 = Stopwatch.GetTimestamp();
        int a = ColdMethod(1);
        TimeSpan first = Stopwatch.GetElapsedTime(t0);

        t0 = Stopwatch.GetTimestamp();
        int b = ColdMethod(2);
        TimeSpan second = Stopwatch.GetElapsedTime(t0);

        Debug.Assert(a == 2 && b == 4);
        Console.WriteLine($"  ColdMethod first≈{first.TotalMicroseconds:F1}µs, second≈{second.TotalMicroseconds:F1}µs");
        Console.WriteLine("  (noisy on short methods; tiering may re-JIT later)");
    }

    private static void DemoPipelineStages()
    {
        Console.WriteLine("-- RyuJIT conceptual pipeline --");
        Console.WriteLine("  1) Importer: IL → IR (trees/nodes)");
        Console.WriteLine("  2) Morph / inlining decisions");
        Console.WriteLine("  3) Optimization passes (CSE, range check, etc.)");
        Console.WriteLine("  4) Lowering + register allocation");
        Console.WriteLine("  5) Emit native code + GC info + EH tables");
        Console.WriteLine("  Observe with: DOTNET_JitDisasm, PerfView, BenchmarkDotNet DisassemblyDiagnoser");
    }

    private static void DemoNoInlineBoundary()
    {
        Console.WriteLine("-- NoInlining forces a real call boundary --");
        int x = NoInlineAdd(10, 5);
        Debug.Assert(x == 15);
        Console.WriteLine($"  NoInlineAdd(10,5)={x}");
        Console.WriteLine("  Useful when measuring call overhead or preventing constant folding across methods.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ColdMethod(int n) => n * 2;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NoInlineAdd(int a, int b) => a + b;
}
