// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : ControlFlowLoops (六、控制流 · 循环 — 5 个)
// Topic id : stage08/section01/control_flow_loops
//
// for / foreach / in / do / while。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class ControlFlowLoops
{
    [LearnTopic("stage08/section01/control_flow_loops")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ControlFlowLoops ===");
        DemoFor();
        DemoWhileAndDo();
        DemoForeachIn();
        return 0;
    }

    private static void DemoFor()
    {
        Console.WriteLine("-- for --");
        int sum = 0;
        for (int i = 1; i <= 5; i++)
            sum += i;
        Debug.Assert(sum == 15);
        Console.WriteLine($"  sum 1..5={sum}");
    }

    private static void DemoWhileAndDo()
    {
        Console.WriteLine("-- while / do --");
        int n = 3, w = 0;
        while (n > 0)
        {
            w += n;
            n--;
        }
        Debug.Assert(w == 6);

        int d = 0, times = 0;
        do
        {
            times++;
            d++;
        } while (d < 1); // 至少执行 1 次
        Debug.Assert(times == 1);
        Console.WriteLine($"  while sum={w}, do times={times}");
    }

    private static void DemoForeachIn()
    {
        Console.WriteLine("-- foreach / in --");
        int[] data = [10, 20, 30];
        int sum = 0;
        foreach (int x in data)
            sum += x;
        Debug.Assert(sum == 60);

        var list = new List<string> { "a", "b" };
        var joined = new List<string>();
        foreach (string s in list)
            joined.Add(s.ToUpperInvariant());
        Debug.Assert(joined is ["A", "B"]);
        Console.WriteLine($"  foreach sum={sum}, joined={string.Join(',', joined)}");
    }
}
