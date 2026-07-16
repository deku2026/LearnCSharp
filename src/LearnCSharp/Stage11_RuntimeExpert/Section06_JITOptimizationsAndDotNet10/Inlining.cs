// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第6部分-JIT优化与dotNET10专题.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section06_JITOptimizationsAndDotNet10
// Item     : Inlining
// Topic id : stage11/section06/inlining
//
// Lesson: inlining removes call overhead and enables further opts; attributes guide JIT.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section06;

internal static class Inlining
{
    [LearnTopic("stage11/section06/inlining")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Inlining ===");
        DemoWhyInline();
        DemoAttributes();
        DemoMeasure();
        return 0;
    }

    private static void DemoWhyInline()
    {
        Console.WriteLine("-- why inlining matters --");
        Console.WriteLine("  Removes call/ret, enables constant prop, CSE, better register use.");
        Console.WriteLine("  Enables devirtualization of callees after type is known.");
        // f(x) = x ^ (x << 1); f(3)=3^6=5; sum=10
        int x = Aggressive(3) + NoInline(3);
        Debug.Assert(x == 10);
        Console.WriteLine($"  Aggressive(3)+NoInline(3)={x}");
    }

    private static void DemoAttributes()
    {
        Console.WriteLine("-- MethodImpl attributes --");
        Console.WriteLine("  AggressiveInlining: hint to inline");
        Console.WriteLine("  NoInlining: force call boundary");
        Console.WriteLine("  AggressiveOptimization: prefer speed (less tiering friendliness historically)");
        Console.WriteLine("  JIT still decides based on size/budget/EH/virtual etc.");
    }

    private static void DemoMeasure()
    {
        Console.WriteLine("-- micro timing (illustrative only) --");
        const int N = 200_000;
        long t0 = Stopwatch.GetTimestamp();
        int s1 = 0;
        for (int i = 0; i < N; i++)
            s1 += Aggressive(i);
        TimeSpan a = Stopwatch.GetElapsedTime(t0);

        t0 = Stopwatch.GetTimestamp();
        int s2 = 0;
        for (int i = 0; i < N; i++)
            s2 += NoInline(i);
        TimeSpan b = Stopwatch.GetElapsedTime(t0);

        Debug.Assert(s1 == s2);
        Console.WriteLine($"  Aggressive loop {a.TotalMilliseconds:F2}ms, NoInline {b.TotalMilliseconds:F2}ms, sum={s1}");
        Console.WriteLine("  Prefer BenchmarkDotNet for real numbers; this is educational.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Aggressive(int x) => x ^ (x << 1);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NoInline(int x) => x ^ (x << 1);
}
