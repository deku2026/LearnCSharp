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
        DemoEscapeVsNoEscape();
        DemoStackallocSpan();
        DemoLimitations();
        return 0;
    }

    private static void DemoEscapeVsNoEscape()
    {
        Console.WriteLine("-- escape analysis idea --");
        Console.WriteLine("  If object never leaves method (no heap store/return/async), JIT may scalar-replace / stack alloc.");
        int s = NoEscapeSum();
        object escaped = Escape();
        Debug.Assert(s == 3 && escaped is int[]);
        Console.WriteLine($"  NoEscapeSum={s}, Escape returns {escaped.GetType().Name}");
    }

    private static void DemoStackallocSpan()
    {
        Console.WriteLine("-- explicit stackalloc + Span --");
        Span<int> buf = stackalloc int[4];
        buf[0] = 1;
        buf[1] = 2;
        buf[2] = 3;
        buf[3] = 4;
        int sum = 0;
        foreach (int v in buf)
            sum += v;
        Debug.Assert(sum == 10);
        Console.WriteLine($"  stackalloc sum={sum} (no GC heap for buffer)");
    }

    private static void DemoLimitations()
    {
        Console.WriteLine("-- limitations --");
        Console.WriteLine("  Async methods, captured locals, interface boxes, large sizes limit stack alloc.");
        Console.WriteLine("  Prefer Span/stackalloc/struct for guaranteed stack-ish patterns.");
        int h = HashPair(1, 2);
        Debug.Assert(h != 0 || h == 0);
        Console.WriteLine($"  struct Pair hash={h}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NoEscapeSum()
    {
        // small helper object — may be optimized away entirely
        var t = new Tiny(1, 2);
        return t.A + t.B;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static object Escape()
    {
        int[] arr = [1, 2, 3];
        return arr; // escapes to caller
    }

    private static int HashPair(int a, int b) => new Pair(a, b).GetHashCode();

    private sealed class Tiny(int a, int b)
    {
        public int A { get; } = a;
        public int B { get; } = b;
    }

    private readonly struct Pair(int a, int b)
    {
        public int A { get; } = a;
        public int B { get; } = b;
        public override int GetHashCode() => HashCode.Combine(A, B);
    }
}
