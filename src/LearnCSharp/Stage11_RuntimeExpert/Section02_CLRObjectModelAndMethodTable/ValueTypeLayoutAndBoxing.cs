// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第2部分-CLR对象模型与方法表.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section02_CLRObjectModelAndMethodTable
// Item     : ValueTypeLayoutAndBoxing
// Topic id : stage11/section02/value_type_layout_and_boxing
//
// Lesson: value types inline on stack/fields; boxing allocates object with MT + payload.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section02;

internal static class ValueTypeLayoutAndBoxing
{
    [LearnTopic("stage11/section02/value_type_layout_and_boxing")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ValueTypeLayoutAndBoxing ===");
        DemoLayout();
        DemoBoxing();
        DemoNullableBox();
        return 0;
    }

    private static void DemoLayout()
    {
        Console.WriteLine("-- value type layout --");
        Console.WriteLine($"  sizeof(Point) via Unsafe={Unsafe.SizeOf<Point>()}");
        Console.WriteLine($"  Marshal.SizeOf<Point>()={Marshal.SizeOf<Point>()}");
        Debug.Assert(Unsafe.SizeOf<Point>() >= 8);
        Point p = new(3, 4);
        Console.WriteLine($"  Point on stack: ({p.X},{p.Y})");
        Debug.Assert(p.X == 3 && p.Y == 4);
    }

    private static void DemoBoxing()
    {
        Console.WriteLine("-- boxing allocates on GC heap --");
        long before = GC.GetTotalMemory(forceFullCollection: false);
        object boxed = new Point(1, 2); // box
        long after = GC.GetTotalMemory(forceFullCollection: false);
        Console.WriteLine($"  boxed type={boxed.GetType().FullName}");
        Debug.Assert(boxed is Point);
        Point unboxed = (Point)boxed;
        Debug.Assert(unboxed.X == 1 && unboxed.Y == 2);
        Console.WriteLine($"  unbox → ({unboxed.X},{unboxed.Y}); mem Δ≈{after - before} (noisy)");
        // interface dispatch on value type also boxes unless constrained
        IFormattable f = new Point(5, 6);
        string s = f.ToString("G", null);
        Console.WriteLine($"  interface on struct may box: '{s}'");
        Debug.Assert(s.Contains('5', StringComparison.Ordinal));
    }

    private static void DemoNullableBox()
    {
        Console.WriteLine("-- Nullable<T> boxing special case --");
        int? has = 42;
        int? empty = null;
        object? o1 = has;   // boxes as boxed int, not Nullable
        object? o2 = empty; // null
        Console.WriteLine($"  (int?)42 boxed type={o1?.GetType().Name}, null?={o2 is null}");
        Debug.Assert(o1 is int);
        Debug.Assert(o2 is null);
        Console.WriteLine("  No 'boxed Nullable' wrapper for null; non-null boxes T.");
    }

    private readonly struct Point(int x, int y) : IFormattable
    {
        public int X { get; } = x;
        public int Y { get; } = y;

        public string ToString(string? format, IFormatProvider? formatProvider) => $"({X},{Y})";
    }
}
