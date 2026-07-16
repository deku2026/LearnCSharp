// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第2部分-模式匹配全谱.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section02_PatternMatchingFullSpectrum
// Item     : LogicalPatterns
// Topic id : stage04/section02/logical_patterns
//
// 步骤 4：逻辑模式 not / and / or + 括号。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section02;

internal static class LogicalPatterns
{
    [LearnTopic("stage04/section02/logical_patterns")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== LogicalPatterns ===");
        DemoNot();
        DemoAnd();
        DemoOr();
        DemoParenthesesAndPriority();
        DemoSwitchClassify();
        return 0;
    }

    private static void DemoNot()
    {
        Console.WriteLine("-- not --");
        string? s = "x";
        Debug.Assert(s is not null);
        int n = 5;
        Debug.Assert(n is not 0);
        Console.WriteLine("  is not null / is not 0");
    }

    private static void DemoAnd()
    {
        Console.WriteLine("-- and：区间 --");
        int age = 30;
        Debug.Assert(age is >= 0 and <= 120);
        Debug.Assert(!(age is >= 0 and <= 20));
        Console.WriteLine($"  age=30 is >=0 and <=120 → true");
    }

    private static void DemoOr()
    {
        Console.WriteLine("-- or：任一匹配 --");
        int day = 6;
        Debug.Assert(day is 6 or 7);
        day = 3;
        Debug.Assert(!(day is 6 or 7));
        Console.WriteLine("  day is 6 or 7 → 周末");
    }

    private static void DemoParenthesesAndPriority()
    {
        Console.WriteLine("-- 括号 + 优先级 not > and > or --");
        char c = 'M';
        Debug.Assert(c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z'));

        int n = 50;
        Debug.Assert(n is not (< 0 or > 100));
        Debug.Assert(!(150 is not (< 0 or > 100)));
        Console.WriteLine("  字母模式；not (<0 or >100) = 在 [0,100]");
    }

    private static void DemoSwitchClassify()
    {
        Console.WriteLine("-- switch 中组合逻辑 --");
        static string Classify(int n) => n switch
        {
            < 0 => "负",
            0 or 1 => "零或一",
            > 1 and < 100 => "小正数",
            _ => "大数",
        };

        Debug.Assert(Classify(-1) == "负");
        Debug.Assert(Classify(0) == "零或一");
        Debug.Assert(Classify(1) == "零或一");
        Debug.Assert(Classify(50) == "小正数");
        Debug.Assert(Classify(200) == "大数");
        Console.WriteLine($"  50→{Classify(50)}, 200→{Classify(200)}");
    }
}
