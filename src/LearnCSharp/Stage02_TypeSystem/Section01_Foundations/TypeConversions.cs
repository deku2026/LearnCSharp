// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : TypeConversions
// Topic id : stage02/section01/type_conversions
//
// 步骤 6：隐式/显式/is/as/用户定义转换/Parse/TryParse。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section01;

internal static class TypeConversions
{
    [LearnTopic("stage02/section01/type_conversions")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TypeConversions ===");
        DemoImplicitExplicit();
        DemoIsAsPattern();
        DemoUserDefined();
        DemoParseTryParse();
        DemoIsAsIgnoreUserDefined();
        return 0;
    }

    private static void DemoImplicitExplicit()
    {
        Console.WriteLine("-- 隐式 vs 显式 --");
        int i = 42;
        long l = i;          // 隐式小→大
        double d = i;        // 隐式整→浮
        int back = (int)d;   // 显式截断
        Debug.Assert(l == 42L && back == 42);

        double pi = 3.9;
        int truncated = (int)pi;
        Debug.Assert(truncated == 3);
        Console.WriteLine($"  (int)3.9={truncated}; long from int={l}");
    }

    private static void DemoIsAsPattern()
    {
        Console.WriteLine("-- is / as / 模式匹配 --");
        object o = "hello";
        if (o is string s)
            Debug.Assert(s.Length == 5);

        string? maybe = o as string;
        Debug.Assert(maybe is not null);

        object n = 42;
        string? fail = n as string;
        Debug.Assert(fail is null);

        try
        {
            _ = (string)n;
            Debug.Assert(false);
        }
        catch (InvalidCastException)
        {
            Console.WriteLine("  cast 失败抛异常；as 失败返 null；is 返 false");
        }

        Debug.Assert(typeof(int) == typeof(System.Int32));
    }

    private static void DemoUserDefined()
    {
        Console.WriteLine("-- 用户定义转换 --");
        Celsius c = new(25);
        double d = c;                 // implicit
        Celsius back = (Celsius)d;    // explicit
        Debug.Assert(d == 25.0 && back.Degrees == 25.0);
        Console.WriteLine($"  Celsius→double={d}, back={back.Degrees}");
    }

    private static void DemoParseTryParse()
    {
        Console.WriteLine("-- Parse / TryParse --");
        int a = int.Parse("42");
        Debug.Assert(a == 42);

        bool ok = int.TryParse("not-a-number", out int bad);
        Debug.Assert(!ok && bad == 0);

        bool ok2 = int.TryParse("99", out int n);
        Debug.Assert(ok2 && n == 99);
        Console.WriteLine($"  Parse(\"42\")={a}; TryParse bad={ok}; good={n}");
    }

    private static void DemoIsAsIgnoreUserDefined()
    {
        Console.WriteLine("-- ⚠ is/as 不触发用户定义转换 --");
        object o = 25.0;
        // is/as 只看运行时类型，不会走 Celsius 的 explicit operator
        Debug.Assert(o is not Celsius);
        Debug.Assert(o as Celsius? is null);
        Celsius c = (Celsius)(double)o; // 必须先到 double 再 cast
        Debug.Assert(c.Degrees == 25.0);
        Console.WriteLine("  用户定义转换必须用 (T) cast，is/as 不会调用");
    }

    private readonly struct Celsius
    {
        public double Degrees { get; }
        public Celsius(double d) => Degrees = d;
        public static implicit operator double(Celsius c) => c.Degrees;
        public static explicit operator Celsius(double d) => new(d);
    }
}
