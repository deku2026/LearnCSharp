// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : OperatorKeywords (十、运算符关键字 — 11 个)
// Topic id : stage08/section01/operator_keywords
//
// new / as / is / typeof / sizeof / stackalloc / checked / unchecked / operator / explicit / implicit。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class OperatorKeywords
{
    [LearnTopic("stage08/section01/operator_keywords")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== OperatorKeywords ===");
        DemoNewAsIsTypeof();
        DemoSizeofStackalloc();
        DemoCheckedUnchecked();
        DemoOperatorOverloadAndConversions();
        return 0;
    }

    private static void DemoNewAsIsTypeof()
    {
        Console.WriteLine("-- new / as / is / typeof --");
        object o = new Sample(9);
        Sample? s = o as Sample;
        Debug.Assert(s is not null && s.Value == 9);
        Debug.Assert(o is Sample sample && sample.Value == 9);
        Debug.Assert(typeof(Sample) == o.GetType());
        object bad = "x";
        Debug.Assert(bad as Sample is null);
        Console.WriteLine($"  typeof={typeof(Sample).Name}, as Sample Value={s!.Value}");
    }

    private static void DemoSizeofStackalloc()
    {
        Console.WriteLine("-- sizeof / stackalloc --");
        int sizeInt = sizeof(int);
        int sizeLong = sizeof(long);
        Debug.Assert(sizeInt == 4 && sizeLong == 8);
        Span<byte> buf = stackalloc byte[4];
        buf[0] = 0xAB;
        Debug.Assert(buf.Length == 4 && buf[0] == 0xAB);
        Console.WriteLine($"  sizeof(int)={sizeInt}, stackalloc len={buf.Length}");
    }

    private static void DemoCheckedUnchecked()
    {
        Console.WriteLine("-- checked / unchecked --");
        int max = int.MaxValue;
        int wrap = unchecked(max + 1);
        Debug.Assert(wrap == int.MinValue);
        bool threw = false;
        try
        {
            _ = checked(max + 1);
        }
        catch (OverflowException)
        {
            threw = true;
        }
        Debug.Assert(threw);
        Console.WriteLine($"  unchecked wrap={wrap}, checked throws={threw}");
    }

    private static void DemoOperatorOverloadAndConversions()
    {
        Console.WriteLine("-- operator / explicit / implicit --");
        Meter a = new Meter(2);
        Meter b = new Meter(3);
        Meter sum = a + b;
        Debug.Assert(sum.Value == 5);
        double d = a; // implicit
        Debug.Assert(Math.Abs(d - 2) < 1e-9);
        Meter m = (Meter)10; // explicit
        Debug.Assert(m.Value == 10);
        Console.WriteLine($"  2m+3m={sum.Value}, implicit double={d}, explicit Meter={m.Value}");
    }

    private sealed class Sample(int value)
    {
        public int Value { get; } = value;
    }

    private readonly struct Meter(double value)
    {
        public double Value { get; } = value;
        public static Meter operator +(Meter a, Meter b) => new(a.Value + b.Value);
        public static implicit operator double(Meter m) => m.Value;
        public static explicit operator Meter(int cm) => new(cm);
    }
}
