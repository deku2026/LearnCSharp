// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第6部分-JIT优化与dotNET10专题.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section06_JITOptimizationsAndDotNet10
// Item     : EscapeAnalysisStackAlloc
// Topic id : stage11/section06/escape_analysis_stack_alloc
//
// Lesson: objects that do not escape may be stack-allocated; stackalloc / Span explicit.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section06;

internal static class EscapeAnalysisStackAlloc
{
    [LearnTopic("stage11/section06/escape_analysis_stack_alloc")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== EscapeAnalysisStackAlloc ===");
        DemoEscapeVsNoEscapeAlloc();
        DemoStackallocZeroHeap();
        DemoStructVsClass();
        return 0;
    }

    private static void DemoEscapeVsNoEscapeAlloc()
    {
        Console.WriteLine("-- escape vs no-escape allocation measurement --");
        // Warm
        _ = NoEscapeSum();
        _ = Escape();

        long beforeNoEsc = GC.GetAllocatedBytesForCurrentThread();
        int s = 0;
        for (int i = 0; i < 10_000; i++)
            s += NoEscapeSum();
        long afterNoEsc = GC.GetAllocatedBytesForCurrentThread();

        long beforeEsc = GC.GetAllocatedBytesForCurrentThread();
        object last = null!;
        for (int i = 0; i < 10_000; i++)
            last = Escape();
        long afterEsc = GC.GetAllocatedBytesForCurrentThread();

        long noEscDelta = afterNoEsc - beforeNoEsc;
        long escDelta = afterEsc - beforeEsc;
        Console.WriteLine($"  10k NoEscapeSum Δalloc={noEscDelta}, sum={s}");
        Console.WriteLine($"  10k Escape()    Δalloc={escDelta}, last={last.GetType().Name}");
        Debug.Assert(s == 30_000);
        Debug.Assert(escDelta > noEscDelta, "escaping allocations should cost more heap");
        Debug.Assert(escDelta > 0);
        Console.WriteLine("  JIT may scalar-replace / stack-alloc non-escaping small objects.");
    }

    private static void DemoStackallocZeroHeap()
    {
        Console.WriteLine("-- explicit stackalloc + Span (guaranteed no buffer heap) --");
        long before = GC.GetAllocatedBytesForCurrentThread();
        int sum = StackSum();
        long after = GC.GetAllocatedBytesForCurrentThread();
        Debug.Assert(sum == 10);
        Console.WriteLine($"  stackalloc sum={sum}, Δalloc={after - before}");
        Debug.Assert(after - before == 0);
    }

    private static void DemoStructVsClass()
    {
        Console.WriteLine("-- struct Pair vs class Tiny --");
        long b0 = GC.GetAllocatedBytesForCurrentThread();
        int h = 0;
        for (int i = 0; i < 5_000; i++)
            h ^= HashPair(i, i + 1);
        long b1 = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 5_000; i++)
            h ^= new Tiny(i, i + 1).GetHashCode();
        long b2 = GC.GetAllocatedBytesForCurrentThread();
        Console.WriteLine($"  5k struct HashPair Δ={b1 - b0}, 5k new Tiny Δ={b2 - b1}, h={h}");
        Debug.Assert(b2 - b1 > b1 - b0);
        Console.WriteLine($"  IsReferenceOrContainsReferences<Pair>={RuntimeHelpers.IsReferenceOrContainsReferences<Pair>()}");
        Debug.Assert(!RuntimeHelpers.IsReferenceOrContainsReferences<Pair>());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NoEscapeSum()
    {
        var t = new Tiny(1, 2);
        return t.A + t.B;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static object Escape()
    {
        int[] arr = [1, 2, 3];
        return arr;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int StackSum()
    {
        Span<int> buf = stackalloc int[4];
        buf[0] = 1;
        buf[1] = 2;
        buf[2] = 3;
        buf[3] = 4;
        int sum = 0;
        foreach (int v in buf)
            sum += v;
        return sum;
    }

    private static int HashPair(int a, int b) => new Pair(a, b).GetHashCode();

    private sealed class Tiny(int a, int b)
    {
        public int A { get; } = a;
        public int B { get; } = b;
        public override int GetHashCode() => HashCode.Combine(A, B);
    }

    private readonly struct Pair(int a, int b)
    {
        public int A { get; } = a;
        public int B { get; } = b;
        public override int GetHashCode() => HashCode.Combine(A, B);
    }
}
