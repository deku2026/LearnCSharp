// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : ControlFlowLoops (六、控制流 · 循环 — 5 个)
// Topic id : stage08/section01/control_flow_loops
//
// for / foreach / in / do / while。
// 相对 Stage04 Loops：补 foreach 变异异常、嵌套 break（关键字视角加深）。

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
        DemoForeachMutationInvalidOperation();
        DemoNestedBreak();
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

        List<string> list = new List<string> { "a", "b" };
        List<string> joined = new List<string>();
        foreach (string s in list)
            joined.Add(s.ToUpperInvariant());
        Debug.Assert(joined is ["A", "B"]);
        Console.WriteLine($"  foreach sum={sum}, joined={string.Join(',', joined)}");
    }

    private static void DemoForeachMutationInvalidOperation()
    {
        Console.WriteLine("-- foreach mutation → InvalidOperationException (caught) --");
        // Stage04 covers mechanism; here keyword-side: foreach enumerator version check.
        List<int> list = new List<int> { 1, 2, 3 };
        bool threw = false;
        try
        {
            foreach (int n in list)
            {
                if (n == 2)
                    list.Remove(n); // mutates during enumerate
            }
        }
        catch (InvalidOperationException ex)
        {
            threw = true;
            Console.WriteLine($"  caught: {ex.GetType().Name}");
        }

        Debug.Assert(threw);
        Console.WriteLine("  never mutate List/Dictionary while foreach-ing it");
    }

    private static void DemoNestedBreak()
    {
        Console.WriteLine("-- nested loops: break only exits the innermost --");
        int hits = 0;
        bool outerContinued = false;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                hits++;
                if (j == 1)
                    break; // leaves inner only
            }

            if (i == 1)
                outerContinued = true;
        }

        // Per outer i: j=0, j=1 then break → 2 inner iterations × 3 outer = 6
        Debug.Assert(hits == 6);
        Debug.Assert(outerContinued);
        Console.WriteLine($"  hits={hits} (break stops inner; outer still runs)");
        Console.WriteLine("  multi-level exit: use goto label, flag, or extract method + return");
    }
}
