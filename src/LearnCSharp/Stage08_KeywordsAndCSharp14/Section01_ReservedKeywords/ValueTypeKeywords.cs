// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : ValueTypeKeywords (一、值类型关键字 — 15 个)
// Topic id : stage08/section01/value_type_keywords
//
// 值类型关键字：bool/byte/sbyte/short/ushort/int/uint/long/ulong/float/double/decimal/char/enum/struct。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class ValueTypeKeywords
{
    [LearnTopic("stage08/section01/value_type_keywords")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ValueTypeKeywords ===");
        DemoIntegerFamily();
        DemoFloatingAndDecimal();
        DemoBoolCharEnum();
        DemoStructValueSemantics();
        return 0;
    }

    private static void DemoIntegerFamily()
    {
        Console.WriteLine("-- 整数族 (byte..ulong) --");
        byte b = 255;
        sbyte sb = -128;
        short s = -1;
        ushort us = 65535;
        int i = int.MaxValue;
        uint ui = 1u;
        long l = 1L;
        ulong ul = 1UL;
        Debug.Assert(typeof(byte) == typeof(System.Byte));
        Debug.Assert(typeof(int) == typeof(System.Int32));
        Debug.Assert(typeof(long) == typeof(System.Int64));
        Debug.Assert(b == byte.MaxValue);
        Debug.Assert(sb == sbyte.MinValue);
        Debug.Assert(us == ushort.MaxValue);
        Console.WriteLine($"  byte={b}, sbyte={sb}, short={s}, ushort={us}");
        Console.WriteLine($"  int={i}, uint={ui}, long={l}, ulong={ul}");
    }

    private static void DemoFloatingAndDecimal()
    {
        Console.WriteLine("-- float / double / decimal --");
        float f = 0.1f;
        double d = 0.1;
        decimal m = 0.1m;
        Debug.Assert(typeof(float) == typeof(System.Single));
        Debug.Assert(typeof(double) == typeof(System.Double));
        Debug.Assert(typeof(decimal) == typeof(System.Decimal));
        // 二进制浮点不精确；decimal 十进制精确（金额用）
        Debug.Assert(f + f + f != 0.3f || true); // may or may not equal; demo point is binary vs decimal
        Debug.Assert(m + m + m == 0.3m);
        Console.WriteLine($"  0.1f+0.1f+0.1f={f + f + f}, 0.1d+0.1d+0.1d={d + d + d}, 0.1m*3={m + m + m}");
    }

    private static void DemoBoolCharEnum()
    {
        Console.WriteLine("-- bool / char / enum --");
        bool ok = true;
        char c = 'A';
        FileAccess flags = FileAccess.Read | FileAccess.Write;
        Debug.Assert(ok);
        Debug.Assert(c == 65);
        Debug.Assert(typeof(char) == typeof(System.Char));
        Debug.Assert(typeof(FileAccess).IsEnum);
        Debug.Assert(flags.HasFlag(FileAccess.Read));
        Console.WriteLine($"  bool={ok}, char={c}, flags={flags}");
    }

    private static void DemoStructValueSemantics()
    {
        Console.WriteLine("-- struct 值语义 --");
        Point a = new Point(1, 2);
        Point b = a; // 拷贝
        b.X = 99;
        Debug.Assert(a.X == 1);
        Debug.Assert(b.X == 99);
        Debug.Assert(typeof(Point).IsValueType);
        Console.WriteLine($"  after copy-mutate: a.X={a.X}, b.X={b.X}");
    }

    [Flags]
    private enum FileAccess
    {
        None = 0,
        Read = 1,
        Write = 2,
    }

    private struct Point
    {
        public int X;
        public int Y;
        public Point(int x, int y) { X = x; Y = y; }
    }
}
