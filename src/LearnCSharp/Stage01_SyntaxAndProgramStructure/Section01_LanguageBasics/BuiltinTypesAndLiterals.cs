// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : BuiltinTypesAndLiterals
// Topic id : stage01/section01/builtin_types_and_literals
//
// 步骤 8：内建类型别名、整数/浮点/decimal/bool/char/string 字面值、易错点。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class BuiltinTypesAndLiterals
{
    [LearnTopic("stage01/section01/builtin_types_and_literals")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== BuiltinTypesAndLiterals ===");
        DemoKeywordAliases();
        DemoIntegralLiterals();
        DemoFloatingAndDecimal();
        DemoBoolAndChar();
        DemoStringLiterals();
        DemoNullAndDefault();
        DemoPitfalls();
        return 0;
    }

    private static void DemoKeywordAliases()
    {
        Console.WriteLine("-- 关键字 = .NET 类型别名 --");
        int x = 42;
        System.Int32 y = 42;
        Debug.Assert(x == y);
        Debug.Assert(typeof(int) == typeof(System.Int32));
        Debug.Assert(typeof(string) == typeof(System.String));
        Debug.Assert(typeof(bool) == typeof(System.Boolean));

        // 统一类型系统：数值也是对象
        string s = 42.ToString();
        Console.WriteLine($"  42.ToString()={s}, int.MaxValue={int.MaxValue}");
        Debug.Assert(s == "42");
        Debug.Assert(42.CompareTo(10) > 0);
    }

    private static void DemoIntegralLiterals()
    {
        Console.WriteLine("-- 整数与分隔符/进制 --");
        int population = 67_000_000;
        long distance = 384_400_000L;
        byte red = 255;
        uint flags = 0xFF00_00FFu;
        int mask = 0b1010_0101;

        Console.WriteLine($"  population={population}, distance={distance}L");
        Console.WriteLine($"  red={red}, flags=0x{flags:X8}, mask=0b{Convert.ToString(mask, 2)}");
        Debug.Assert(population == 67_000_000);
        Debug.Assert(distance == 384_400_000L);
        Debug.Assert(red == 255);
        Debug.Assert(flags == 0xFF0000FFu);
        Debug.Assert(mask == 0b1010_0101);

        nint native = 100;
        Console.WriteLine($"  nint size={nint.Size} bytes, value={native}");
        Debug.Assert(native == 100);
    }

    private static void DemoFloatingAndDecimal()
    {
        Console.WriteLine("-- 浮点与 decimal --");
        double pi = 3.141592653589793;
        float gravity = 9.81f;          // 必须 f；9.81 是 double
        decimal price = 19.99m;
        double exp = 1.5e3;

        Console.WriteLine($"  pi={pi}, gravity={gravity}f, price={price}m, exp={exp}");
        Debug.Assert(gravity == 9.81f);
        Debug.Assert(price == 19.99m);
        Debug.Assert(exp == 1500.0);

        // double 精度误差 vs decimal 精确
        double dSum = 0.1 + 0.2;
        decimal mSum = 0.1m + 0.2m;
        Console.WriteLine($"  double  0.1+0.2 = {dSum:R}");
        Console.WriteLine($"  decimal 0.1+0.2 = {mSum}");
        Debug.Assert(mSum == 0.3m);
        Debug.Assert(dSum != 0.3); // 经典浮点坑

        // decimal 不能与 double/float 直接混用
        decimal fromInt = 10; // 整数可隐式转 decimal
        Debug.Assert(fromInt == 10m);
    }

    private static void DemoBoolAndChar()
    {
        Console.WriteLine("-- bool 与 char --");
        bool ok = true;
        char ch = 'A';
        char nl = '\n';
        char hex = '\x0041';
        char uni = '\u0041';

        Console.WriteLine($"  ok={ok}, ch={ch}, hex={hex}, uni={uni}, sizeof(char)概念上=2 UTF-16");
        Debug.Assert(ok);
        Debug.Assert(ch == 'A' && hex == 'A' && uni == 'A');
        Debug.Assert(sizeof(char) == 2);
        Debug.Assert(nl == '\n');

        // ⚠ 无隐式 int↔bool
        int n = 5;
        // if (n) { } // 编译错误
        if (n != 0)
            Console.WriteLine($"  if (n != 0) 才合法, n={n}");
        Debug.Assert(n != 0);
    }

    private static void DemoStringLiterals()
    {
        Console.WriteLine("-- 字符串四种形态 --");
        string a = "普通\t带转义";
        string b = @"C:\path\no\escape";
        string c = """
            原始字符串(raw)
            可含 " 和 \ 而不用转义
            """;
        string d = $"插值:1+2={1 + 2}";

        Console.WriteLine($"  regular: {a}");
        Console.WriteLine($"  verbatim: {b}");
        Console.WriteLine($"  raw: {c.Trim()}");
        Console.WriteLine($"  interp: {d}");

        Debug.Assert(a.Contains('\t'));
        Debug.Assert(b.Contains(@"\path\"));
        Debug.Assert(c.Contains('"'));
        Debug.Assert(d == "插值:1+2=3");
    }

    private static void DemoNullAndDefault()
    {
        Console.WriteLine("-- null 与 default --");
        string? maybe = null;
        int zero = default;
        bool f = default;
        string? r = default;

        Console.WriteLine($"  default(int)={zero}, default(bool)={f}, default(string) is null? {r is null}");
        Debug.Assert(maybe is null);
        Debug.Assert(zero == 0);
        Debug.Assert(!f);
        Debug.Assert(r is null);
    }

    private static void DemoPitfalls()
    {
        Console.WriteLine("-- 易错点汇总 --");
        Console.WriteLine("  1) if (someInt) 非法 → if (someInt != 0)");
        Console.WriteLine("  2) float g = 9.81; 非法 → 9.81f");
        Console.WriteLine("  3) char 是 UTF-16 码元(2 字节)，不是 C++ 1 字节 char");
        Console.WriteLine("  4) 金额用 decimal，别用 double 累加");

        float g = 9.81f;
        Debug.Assert(g > 9.8f && g < 9.82f);
    }
}
