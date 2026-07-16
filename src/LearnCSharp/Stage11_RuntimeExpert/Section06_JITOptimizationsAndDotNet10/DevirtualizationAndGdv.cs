// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第6部分-JIT优化与dotNET10专题.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section06_JITOptimizationsAndDotNet10
// Item     : DevirtualizationAndGdv
// Topic id : stage11/section06/devirtualization_and_gdv
//
// Lesson: guarded devirtualization (GDV) specializes monomorphic/hot virtual sites.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section06;

internal static class DevirtualizationAndGdv
{
    [LearnTopic("stage11/section06/devirtualization_and_gdv")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DevirtualizationAndGdv ===");
        DemoDevirtSealed();
        DemoMonomorphicVsPolymorphicTiming();
        return 0;
    }

    private static void DemoDevirtSealed()
    {
        Console.WriteLine("-- classic devirtualization --");
        Animal a = new Dog();
        string s = a.Speak();
        Debug.Assert(s == "dog");
        Console.WriteLine($"  Animal a = new Dog(); a.Speak() → {s}");
        Console.WriteLine("  When exact type known (sealed/newobj SSA), virtual → direct/inline.");
    }

    private static void DemoMonomorphicVsPolymorphicTiming()
    {
        Console.WriteLine("-- monomorphic vs polymorphic interface calls (multi-run) --");
        IShape mono = new Circle();
        IShape[] poly = [new Circle(), new Square(), new Circle(), new Square()];

        // Warmup
        double w = 0;
        for (int i = 0; i < 20_000; i++)
        {
            w += Measure(mono);
            w += Measure(poly[i % poly.Length]);
        }

        Debug.Assert(w > 0);

        const int N = 200_000;
        double[] monoMs = new double[5];
        double[] polyMs = new double[5];
        double monoSum = 0, polySum = 0;
        for (int r = 0; r < 5; r++)
        {
            long t0 = Stopwatch.GetTimestamp();
            double s = 0;
            for (int i = 0; i < N; i++)
                s += Measure(mono);
            monoMs[r] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            monoSum = s;

            t0 = Stopwatch.GetTimestamp();
            s = 0;
            for (int i = 0; i < N; i++)
                s += Measure(poly[i % poly.Length]);
            polyMs[r] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            polySum = s;
        }

        Array.Sort(monoMs);
        Array.Sort(polyMs);
        Console.WriteLine($"  monomorphic median={monoMs[2]:F3}ms sum≈{monoSum:F0}");
        Console.WriteLine($"  polymorphic  median={polyMs[2]:F3}ms sum≈{polySum:F0}");
        Debug.Assert(monoSum > 0 && polySum > 0);
        Debug.Assert(monoMs[2] >= 0 && polyMs[2] >= 0);
        Console.WriteLine("  GDV: if (obj.MT == expected) direct; else virtual. PGO picks expected type.");
        Console.WriteLine("  Megamorphic sites resist single-type GDV.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static double Measure(IShape s) => s.Area();

    private abstract class Animal
    {
        public abstract string Speak();
    }

    private sealed class Dog : Animal
    {
        public override string Speak() => "dog";
    }

    private interface IShape
    {
        double Area();
    }

    private sealed class Circle : IShape
    {
        public double Area() => Math.PI;
    }

    private sealed class Square : IShape
    {
        public double Area() => 1.0;
    }
}
