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
        DemoAttributesAndCorrectness();
        DemoMultiRunMeasure();
        return 0;
    }

    private static void DemoWhyInline()
    {
        Console.WriteLine("-- why inlining matters --");
        Console.WriteLine("  Removes call/ret, enables constant prop, CSE, better registers.");
        int x = Aggressive(3) + NoInline(3);
        Debug.Assert(x == 10);
        Console.WriteLine($"  Aggressive(3)+NoInline(3)={x} (same IL body, different MethodImpl)");
    }

    private static void DemoAttributesAndCorrectness()
    {
        Console.WriteLine("-- MethodImpl options (observable correctness) --");
        Console.WriteLine("  AggressiveInlining | NoInlining | AggressiveOptimization | NoOptimization");
        int a = Aggressive(100);
        int n = NoInline(100);
        int o = NoOpt(100);
        Debug.Assert(a == n && n == o);
        Console.WriteLine($"  Aggressive/NoInline/NoOpt(100) all={a}");
        Console.WriteLine($"  IsReferenceOrContainsReferences<int>={RuntimeHelpers.IsReferenceOrContainsReferences<int>()}");
        Debug.Assert(!RuntimeHelpers.IsReferenceOrContainsReferences<int>());
    }

    private static void DemoMultiRunMeasure()
    {
        Console.WriteLine("-- multi-run micro timing AggressiveInlining vs NoInlining --");
        const int N = 300_000;
        // Warmup both
        int w = 0;
        for (int i = 0; i < 50_000; i++)
        {
            w += Aggressive(i);
            w += NoInline(i);
        }

        Debug.Assert(w != 0);

        double[] ag = new double[5];
        double[] ni = new double[5];
        long sAg = 0, sNi = 0;
        for (int r = 0; r < 5; r++)
        {
            long t0 = Stopwatch.GetTimestamp();
            for (int i = 0; i < N; i++)
                sAg += Aggressive(i);
            ag[r] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;

            t0 = Stopwatch.GetTimestamp();
            for (int i = 0; i < N; i++)
                sNi += NoInline(i);
            ni[r] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
        }

        Array.Sort(ag);
        Array.Sort(ni);
        Console.WriteLine($"  Aggressive median={ag[2]:F3}ms, NoInline median={ni[2]:F3}ms, N={N}");
        Console.WriteLine($"  sums equal: {sAg == sNi} (sAg={sAg})");
        Debug.Assert(sAg == sNi);
        // Structure assert: both produced positive times; relative speed is environment-dependent
        Debug.Assert(ag[2] >= 0 && ni[2] >= 0);
        Console.WriteLine("  Prefer BenchmarkDotNet for publishable numbers; call overhead often visible here.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Aggressive(int x) => x ^ (x << 1);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NoInline(int x) => x ^ (x << 1);

    [MethodImpl(MethodImplOptions.NoOptimization)]
    private static int NoOpt(int x) => x ^ (x << 1);
}
