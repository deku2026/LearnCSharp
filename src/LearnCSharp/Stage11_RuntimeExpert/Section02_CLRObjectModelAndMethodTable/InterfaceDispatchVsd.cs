// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第2部分-CLR对象模型与方法表.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section02_CLRObjectModelAndMethodTable
// Item     : InterfaceDispatchVsd
// Topic id : stage11/section02/interface_dispatch_vsd
//
// Lesson: interface calls use interface map / VSD stubs; multiple interfaces share object.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section02;

internal static class InterfaceDispatchVsd
{
    [LearnTopic("stage11/section02/interface_dispatch_vsd")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== InterfaceDispatchVsd ===");
        DemoInterfaceMap();
        DemoDiamondLikeExplicitImpl();
        DemoVsdConcept();
        return 0;
    }

    private static void DemoInterfaceMap()
    {
        Console.WriteLine("-- interface dispatch --");
        IShape circle = new Circle(2);
        IShape square = new Square(3);
        Console.WriteLine($"  Circle area={circle.Area():F2}, Square area={square.Area():F2}");
        Debug.Assert(Math.Abs(circle.Area() - (Math.PI * 4)) < 1e-6);
        Debug.Assert(Math.Abs(square.Area() - 9) < 1e-9);
        Console.WriteLine("  Call site is IShape::Area; runtime finds impl via interface map on MT.");
    }

    private static void DemoDiamondLikeExplicitImpl()
    {
        Console.WriteLine("-- explicit interface implementation --");
        var dual = new Dual();
        IAlpha a = dual;
        IBeta b = dual;
        Console.WriteLine($"  IAlpha.Name={a.Name}, IBeta.Name={b.Name}");
        Debug.Assert(a.Name == "alpha");
        Debug.Assert(b.Name == "beta");
        Console.WriteLine("  Two interface slots can point at different methods on same object.");
    }

    private static void DemoVsdConcept()
    {
        Console.WriteLine("-- Virtual Stub Dispatch (VSD) idea --");
        Console.WriteLine("  Monomorphic sites: stub caches one target MT → fast path.");
        Console.WriteLine("  Polymorphic sites: fall back to lookup / dispatch map.");
        IShape[] shapes = [new Circle(1), new Square(1), new Circle(1)];
        double sum = 0;
        foreach (IShape s in shapes)
            sum += s.Area();
        Console.WriteLine($"  polymorphic sum areas={sum:F2}");
        Debug.Assert(sum > 0);
        Console.WriteLine("  Dynamic PGO + GDV can specialize hot interface call sites (see Stage11 §6).");
    }

    private interface IShape
    {
        double Area();
    }

    private sealed class Circle(double r) : IShape
    {
        public double Area() => Math.PI * r * r;
    }

    private sealed class Square(double side) : IShape
    {
        public double Area() => side * side;
    }

    private interface IAlpha
    {
        string Name { get; }
    }

    private interface IBeta
    {
        string Name { get; }
    }

    private sealed class Dual : IAlpha, IBeta
    {
        string IAlpha.Name => "alpha";
        string IBeta.Name => "beta";
    }
}
