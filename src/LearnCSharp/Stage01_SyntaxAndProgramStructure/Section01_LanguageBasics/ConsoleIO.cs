// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : ConsoleIO
// Topic id : stage01/section01/console_io
//
// 步骤 9：Write/WriteLine、复合格式化 vs 内插、标准流。不调用 ReadLine（非交互宿主）。

using System.Diagnostics;
using System.Globalization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class ConsoleIO
{
    [LearnTopic("stage01/section01/console_io")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ConsoleIO ===");
        DemoWriteVsWriteLine();
        DemoCompositeFormatting();
        DemoInterpolation();
        DemoStandardStreams();
        DemoInputApiShape();
        DemoParseWithoutReadLine();
        return 0;
    }

    private static void DemoWriteVsWriteLine()
    {
        Console.WriteLine("-- Write / WriteLine --");
        Console.Write("  不换行-A");
        Console.Write("-B");
        Console.WriteLine("-C 然后换行");
        Console.WriteLine(42);
        Console.WriteLine(); // 空行

        Debug.Assert(Environment.NewLine.Length is 1 or 2);
        Console.WriteLine($"  Environment.NewLine 转义可见长度={Environment.NewLine.Length}");
    }

    private static void DemoCompositeFormatting()
    {
        Console.WriteLine("-- 复合格式化 {索引[:格式]} --");
        int x = 5, y = 7;
        Console.WriteLine("  {0} + {1} = {2}", x, y, x + y);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  价格:{0:C}", 19.99m));
        Console.WriteLine("  两位小数 N2: {0:N2}", 1234.5);
        Console.WriteLine("  十六进制 X: {0:X4}", 255);

        string composite = string.Format(CultureInfo.InvariantCulture, "{0}+{1}={2}", x, y, x + y);
        Debug.Assert(composite == "5+7=12");
    }

    private static void DemoInterpolation()
    {
        Console.WriteLine("-- 字符串内插（推荐）--");
        int x = 5, y = 7;
        Console.WriteLine($"  {x} + {y} = {x + y}");
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"  价格:{19.99m:C}"));
        Console.WriteLine($"  平均值 N2: {((x + y) / 2.0):N2}");

        // 内插是语法糖 → Format / InterpolatedStringHandler
        string s = $"{x}+{y}={x + y}";
        Debug.Assert(s == "5+7=12");
        Console.WriteLine("  SharpLab: $\"...\" lower 成 Format 或 DefaultInterpolatedStringHandler");
    }

    private static void DemoStandardStreams()
    {
        Console.WriteLine("-- 三个标准流 --");
        Console.WriteLine($"  Console.Out  type = {Console.Out.GetType().Name}");
        Console.WriteLine($"  Console.Error type = {Console.Error.GetType().Name}");
        Console.WriteLine($"  Console.In   type = {Console.In.GetType().Name}");

        // 错误信息走 stderr（本 demo 写一行示意）
        Console.Error.WriteLine("  [stderr] 错误信息走 Console.Error");

        Debug.Assert(Console.Out is TextWriter);
        Debug.Assert(Console.Error is TextWriter);
        Debug.Assert(Console.In is TextReader);
    }

    private static void DemoInputApiShape()
    {
        Console.WriteLine("-- 输入 API 形状（本主题不调用 ReadLine）--");
        Console.WriteLine("  ReadLine() → string?  （EOF 时 null；可空引用类型相关）");
        Console.WriteLine("  Read()     → int 字符码，无输入 -1");
        Console.WriteLine("  ReadKey()  → ConsoleKeyInfo，常用于“按任意键”");
        Console.WriteLine("  输入永远是字符串，数字需 Parse/TryParse");
        Console.WriteLine("  ⚠ 服务器/无人值守程序不要用 Console 做日志 → 用 ILogger");

        // 用反射确认签名，避免真实阻塞读入
        System.Reflection.MethodInfo? readLine = typeof(Console).GetMethod(nameof(Console.ReadLine));
        Debug.Assert(readLine is not null);
        Debug.Assert(readLine.ReturnType == typeof(string));
    }

    private static void DemoParseWithoutReadLine()
    {
        Console.WriteLine("-- 解析演示（模拟用户输入字符串）--");
        string rawA = "12";
        string rawB = "8";
        string bad = "xx";

        bool okA = int.TryParse(rawA, out int a);
        bool okB = int.TryParse(rawB, out int b);
        bool okBad = int.TryParse(bad, out _);

        Debug.Assert(okA && okB && !okBad);
        Console.WriteLine($"  TryParse \"{rawA}\" → {okA}, value={a}");
        Console.WriteLine($"  TryParse \"{rawB}\" → {okB}, value={b}");
        Console.WriteLine($"  TryParse \"{bad}\" → {okBad}");

        if (okA && okB)
        {
            int sum = a + b;
            double avg = (a + b) / 2.0;
            Console.WriteLine($"  和(内插)={sum}");
            Console.WriteLine($"  平均(复合 N2)={string.Format(CultureInfo.InvariantCulture, "{0:N2}", avg)}");
            Debug.Assert(sum == 20);
        }

        // 🔶 C++: cin >> n 按类型读；C# 更显式：先 string 再解析
        Console.WriteLine("  🔶 C++ cin>>n 自动按类型；C# 默认 string + 显式解析");
    }
}
