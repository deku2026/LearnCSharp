// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第2部分-函数成员与构造.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section02_FunctionMembersAndConstruction
// Item     : Methods
// Topic id : stage03/section02/methods
//
// 步骤 1：重载、可选/命名参数、params(+collections C#13)、表达式主体、ref/in/out。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section02;

internal static class Methods
{
    [LearnTopic("stage03/section02/methods")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Methods ===");
        DemoOverloads();
        DemoOptionalAndNamed();
        DemoParamsArrayAndSpan();
        DemoExpressionBodiedAndRef();
        return 0;
    }

    private static void DemoOverloads()
    {
        Console.WriteLine("-- 方法重载：参数列表区分 --");
        var c = new Calculator();
        Debug.Assert(c.Add(1, 2) == 3);
        Debug.Assert(Math.Abs(c.Add(1.5, 2.5) - 4.0) < 1e-9);
        Debug.Assert(c.Add(1, 2, 3) == 6);
        Console.WriteLine($"  Add(1,2)={c.Add(1, 2)}, Add(1,2,3)={c.Add(1, 2, 3)}");
        // 不能仅靠返回类型重载
    }

    private static void DemoOptionalAndNamed()
    {
        Console.WriteLine("-- 可选参数 + 命名参数 --");
        string a = Log("启动");
        string b = Log("出错", "ERROR");
        string c = Log("事件", timestamp: false);
        string d = Log(level: "WARN", msg: "注意");
        Debug.Assert(a.Contains("INFO") && a.Contains("启动"));
        Debug.Assert(b.Contains("ERROR"));
        Debug.Assert(!c.Contains("|"));
        Debug.Assert(d.Contains("WARN"));
        Console.WriteLine($"  {a}");
        Console.WriteLine($"  {d}");
    }

    private static void DemoParamsArrayAndSpan()
    {
        Console.WriteLine("-- params 数组 / params ReadOnlySpan(C#13) --");
        Debug.Assert(SumArray(1, 2, 3) == 6);
        Debug.Assert(SumArray() == 0);
        Debug.Assert(SumSpan(1, 2, 3, 4) == 10);
        Debug.Assert(SumSpan(new[] { 5, 5 }) == 10);
        Console.WriteLine($"  SumArray(1,2,3)={SumArray(1, 2, 3)}");
        Console.WriteLine($"  SumSpan(1..4)={SumSpan(1, 2, 3, 4)}");
    }

    private static void DemoExpressionBodiedAndRef()
    {
        Console.WriteLine("-- 表达式主体 + ref/in/out --");
        var util = new Util();
        Debug.Assert(util.Square(5) == 25);
        int x = 10;
        util.Double(ref x);
        Debug.Assert(x == 20);
        util.ReadOnlyAdd(in x, 5, out int sum);
        Debug.Assert(sum == 25);
        Console.WriteLine($"  Square(5)=25, Double->20, in/out sum={sum}");
    }

    private static string Log(string msg, string level = "INFO", bool timestamp = true)
    {
        return timestamp ? $"[{level}|ts] {msg}" : $"[{level}] {msg}";
    }

    private static int SumArray(params int[] nums)
    {
        int s = 0;
        foreach (var n in nums) s += n;
        return s;
    }

    private static int SumSpan(params ReadOnlySpan<int> nums)
    {
        int s = 0;
        foreach (var n in nums) s += n;
        return s;
    }

    private sealed class Calculator
    {
        public int Add(int a, int b) => a + b;
        public double Add(double a, double b) => a + b;
        public int Add(int a, int b, int c) => a + b + c;
    }

    private sealed class Util
    {
        public int Square(int x) => x * x;
        public void Double(ref int n) => n *= 2;
        public void ReadOnlyAdd(in int a, int b, out int result) => result = a + b;
    }
}
