// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第2部分-聚合与现代类型.md
// Stage    : Stage02_TypeSystem
// Section  : Section02_AggregatesAndModernTypes
// Item     : TuplesAndDestructuring
// Topic id : stage02/section02/tuples_and_destructuring
//
// 步骤 2：ValueTuple、命名元素、解构、Deconstruct。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section02;

internal static class TuplesAndDestructuring
{
    [LearnTopic("stage02/section02/tuples_and_destructuring")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TuplesAndDestructuring ===");
        DemoNamedElements();
        DemoEqualityIgnoresNames();
        DemoDestructure();
        DemoCustomDeconstruct();
        DemoWhenNotToUse();
        return 0;
    }

    private static void DemoNamedElements()
    {
        Console.WriteLine("-- 命名元素（编译期糖） --");
        (int Count, string Name) t = (5, "Ada");
        Debug.Assert(t.Count == 5 && t.Name == "Ada");
        Debug.Assert(t.Item1 == 5 && t.Item2 == "Ada");
        (double Latitude, double Longitude) loc = (Latitude: 47.6, Longitude: -122.3);
        Debug.Assert(loc.Latitude == 47.6);
        Console.WriteLine($"  t=({t.Count},{t.Name}); Item1={t.Item1}");
    }

    private static void DemoEqualityIgnoresNames()
    {
        Console.WriteLine("-- 相等只看位置与值，名字无关 --");
#pragma warning disable CS8383 // 演示元组相等忽略元素名
        Debug.Assert((a: 1, b: 2) == (x: 1, y: 2));
#pragma warning restore CS8383
        Debug.Assert((1, 2) != (1, 3));
        Console.WriteLine("  (a:1,b:2) == (x:1,y:2) → true");
    }

    private static void DemoDestructure()
    {
        Console.WriteLine("-- 解构与弃元 --");
        (int diameter, double area) = GetCircle(3);
        Debug.Assert(diameter == 6);
        Debug.Assert(Math.Abs(area - Math.PI * 9) < 1e-9);

        (int d, double a) = GetCircle(4);
        Debug.Assert(d == 8);
        (int d2, double _) = GetCircle(5);
        Debug.Assert(d2 == 10);
        Console.WriteLine($"  GetCircle(3)=({diameter}, {area:F2})");
        _ = a;
    }

    private static void DemoCustomDeconstruct()
    {
        Console.WriteLine("-- 任意类型可 Deconstruct --");
        Point p = new Point(3, 4);
        (int x, int y) = p;
        Debug.Assert(x == 3 && y == 4);
        (x, y) = (y, x); // 元组交换赋值（无需临时变量）
        Debug.Assert(x == 4 && y == 3);
        Console.WriteLine($"  Point Deconstruct → swap → ({x},{y})");
    }

    private static void DemoWhenNotToUse()
    {
        Console.WriteLine("-- 何时用 record 而非元组 --");
        // 临时返回多值 → 元组；有领域含义/行为 → record/具名类型
        (bool ok, int value) temp = (ok: true, value: 42);
        Debug.Assert(temp.ok);
        Console.WriteLine("  临时打包用元组；长期领域模型用 record");
    }

    private static (int Diameter, double Area) GetCircle(int r) => (2 * r, Math.PI * r * r);

    private sealed class Point
    {
        public int X { get; }
        public int Y { get; }
        public Point(int x, int y) => (X, Y) = (x, y);
        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
    }
}
