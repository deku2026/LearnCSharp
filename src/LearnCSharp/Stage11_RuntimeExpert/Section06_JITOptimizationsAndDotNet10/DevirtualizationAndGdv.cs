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
        DemoGdvIdea();
        DemoPolymorphic();
        return 0;
    }

    private static void DemoDevirtSealed()
    {
        Console.WriteLine("-- classic devirtualization --");
        Console.WriteLine("  When exact type is known (sealed, newobj SSA), virtual → direct call/inline.");
        Animal a = new Dog(); // type often proven
        string s = a.Speak();
        Debug.Assert(s == "dog");
        Console.WriteLine($"  Animal a = new Dog(); a.Speak() → {s}");
    }

    private static void DemoGdvIdea()
    {
        Console.WriteLine("-- Guarded Devirtualization (GDV) --");
        Console.WriteLine("  if (obj.MT == expected) direct_call; else fallback virtual stub;");
        Console.WriteLine("  Profile (Dynamic PGO) picks the expected type for hot sites.");
        IShape shape = new Circle();
        double area = 0;
        for (int i = 0; i < 10_000; i++)
            area += Measure(shape);
        Debug.Assert(area > 0);
        Console.WriteLine($"  monomorphic IShape loop sum areas≈{area:F1}");
    }

    private static void DemoPolymorphic()
    {
        Console.WriteLine("-- polymorphic sites resist single-type GDV --");
        IShape[] shapes = [new Circle(), new Square(), new Circle()];
        double sum = 0;
        for (int i = 0; i < 3000; i++)
            sum += Measure(shapes[i % shapes.Length]);
        Debug.Assert(sum > 0);
        Console.WriteLine($"  mixed shapes sum≈{sum:F1}");
        Console.WriteLine("  Too many types → megamorphic; keep interfaces but reduce type churn if hot.");
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
