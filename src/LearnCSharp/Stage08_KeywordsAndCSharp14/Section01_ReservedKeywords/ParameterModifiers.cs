// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : ParameterModifiers (九、方法参数修饰符 — 4 个)
// Topic id : stage08/section01/parameter_modifiers
//
// ref / out / in / params。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class ParameterModifiers
{
    [LearnTopic("stage08/section01/parameter_modifiers")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ParameterModifiers ===");
        DemoRef();
        DemoOut();
        DemoIn();
        DemoParams();
        return 0;
    }

    private static void DemoRef()
    {
        Console.WriteLine("-- ref --");
        int x = 1;
        Increment(ref x);
        Debug.Assert(x == 2);
        Swap(ref x, ref x); // no-op self
        int a = 3, b = 4;
        Swap(ref a, ref b);
        Debug.Assert(a == 4 && b == 3);
        Console.WriteLine($"  after Increment x={x}, Swap a={a} b={b}");
    }

    private static void DemoOut()
    {
        Console.WriteLine("-- out --");
        bool ok = TryParsePositive("42", out int n);
        Debug.Assert(ok);
        Debug.Assert(n == 42);
        Debug.Assert(!TryParsePositive("-1", out _));
        Console.WriteLine($"  TryParsePositive(\"42\")={n}");
    }

    private static void DemoIn()
    {
        Console.WriteLine("-- in (只读引用，避免大 struct 拷贝) --");
        var big = new BigPoint(1, 2, 3);
        double d = DistanceFromOrigin(in big);
        Debug.Assert(Math.Abs(d - Math.Sqrt(14)) < 1e-9);
        Console.WriteLine($"  Distance={d:F3}");
    }

    private static void DemoParams()
    {
        Console.WriteLine("-- params --");
        Debug.Assert(Sum(1, 2, 3) == 6);
        Debug.Assert(Sum() == 0);
        // C#13 params collections
        Debug.Assert(SumSpan(1, 2, 3, 4) == 10);
        Console.WriteLine($"  Sum(1,2,3)={Sum(1, 2, 3)}, SumSpan={SumSpan(1, 2, 3, 4)}");
    }

    private static void Increment(ref int n) => n++;
    private static void Swap(ref int a, ref int b) => (a, b) = (b, a);

    private static bool TryParsePositive(string s, out int value)
    {
        if (int.TryParse(s, out value) && value > 0) return true;
        value = 0;
        return false;
    }

    private static double DistanceFromOrigin(in BigPoint p)
        => Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);

    private static int Sum(params int[] values)
    {
        int s = 0;
        foreach (int v in values) s += v;
        return s;
    }

    private static int SumSpan(params ReadOnlySpan<int> values)
    {
        int s = 0;
        foreach (int v in values) s += v;
        return s;
    }

    private readonly struct BigPoint(double x, double y, double z)
    {
        public double X { get; } = x;
        public double Y { get; } = y;
        public double Z { get; } = z;
    }
}
