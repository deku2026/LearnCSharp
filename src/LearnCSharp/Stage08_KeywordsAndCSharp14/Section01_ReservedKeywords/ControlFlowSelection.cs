// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : ControlFlowSelection (五、控制流 · 选择 — 5 个)
// Topic id : stage08/section01/control_flow_selection
//
// if / else / switch / case / default。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class ControlFlowSelection
{
    [LearnTopic("stage08/section01/control_flow_selection")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ControlFlowSelection ===");
        DemoIfElse();
        DemoSwitchStatement();
        DemoSwitchExpression();
        DemoPatternCases();
        return 0;
    }

    private static void DemoIfElse()
    {
        Console.WriteLine("-- if / else --");
        int n = 7;
        string kind;
        if (n % 2 == 0)
            kind = "even";
        else if (n < 0)
            kind = "neg";
        else
            kind = "odd-pos";
        Debug.Assert(kind == "odd-pos");
        // 条件必须 bool：// if (n) 非法
        Console.WriteLine($"  n={n} => {kind}");
    }

    private static void DemoSwitchStatement()
    {
        Console.WriteLine("-- switch / case / default（无穿透） --");
        string Describe(int code) => code switch
        {
            0 => "ok",
            1 or 2 => "retry",
            _ => "other",
        };

        // 语句形式
        string label;
        int x = 2;
        switch (x)
        {
            case 1:
                label = "one";
                break;
            case 2:
            case 3:
                label = "two-or-three";
                break;
            default:
                label = "default";
                break;
        }
        Debug.Assert(label == "two-or-three");
        Debug.Assert(Describe(0) == "ok");
        Debug.Assert(Describe(2) == "retry");
        Debug.Assert(Describe(9) == "other");
        Console.WriteLine($"  label={label}, Describe(2)={Describe(2)}");
    }

    private static void DemoSwitchExpression()
    {
        Console.WriteLine("-- switch 表达式 --");
        string Grade(int score) => score switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 60 => "C",
            _ => "F",
        };
        Debug.Assert(Grade(95) == "A");
        Debug.Assert(Grade(50) == "F");
        Console.WriteLine($"  Grade(95)={Grade(95)}, Grade(50)={Grade(50)}");
    }

    private static void DemoPatternCases()
    {
        Console.WriteLine("-- case 模式 --");
        object o = "hello";
        string r = o switch
        {
            string s when s.Length > 3 => $"long:{s.Length}",
            string s => $"short:{s}",
            int n => $"int:{n}",
            null => "null",
            _ => "other",
        };
        Debug.Assert(r == "long:5");
        Console.WriteLine($"  pattern result={r}");
    }
}
