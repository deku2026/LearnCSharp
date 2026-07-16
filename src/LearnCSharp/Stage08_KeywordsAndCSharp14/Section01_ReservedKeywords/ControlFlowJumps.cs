// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : ControlFlowJumps (七、控制流 · 跳转 — 4 个)
// Topic id : stage08/section01/control_flow_jumps
//
// break / continue / goto / return。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class ControlFlowJumps
{
    [LearnTopic("stage08/section01/control_flow_jumps")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ControlFlowJumps ===");
        DemoBreakContinue();
        DemoGotoCase();
        DemoReturn();
        return 0;
    }

    private static void DemoBreakContinue()
    {
        Console.WriteLine("-- break / continue --");
        var kept = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            if (i % 2 == 0) continue; // 跳过偶数
            if (i > 7) break;         // 终止
            kept.Add(i);
        }
        Debug.Assert(kept is [1, 3, 5, 7]);
        Console.WriteLine($"  kept=[{string.Join(',', kept)}]");
    }

    private static void DemoGotoCase()
    {
        Console.WriteLine("-- goto case / goto default --");
        string Route(int code)
        {
            string tag = "";
            switch (code)
            {
                case 1:
                    tag = "one";
                    goto case 2; // 显式穿透
                case 2:
                    tag += "+two";
                    break;
                case 99:
                    goto default;
                default:
                    tag = "default";
                    break;
            }
            return tag;
        }
        Debug.Assert(Route(1) == "one+two");
        Debug.Assert(Route(99) == "default");
        Console.WriteLine($"  Route(1)={Route(1)}, Route(99)={Route(99)}");
    }

    private static void DemoReturn()
    {
        Console.WriteLine("-- return --");
        Debug.Assert(Abs(-5) == 5);
        Debug.Assert(Abs(3) == 3);
        EarlyExit(false);
        Console.WriteLine($"  Abs(-5)={Abs(-5)}");
    }

    private static int Abs(int n)
    {
        if (n < 0) return -n;
        return n;
    }

    private static void EarlyExit(bool flag)
    {
        if (!flag) return;
        Debug.Assert(false, "should not reach");
    }
}
