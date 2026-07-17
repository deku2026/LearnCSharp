// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第1部分-控制流.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section01_ControlFlow
// Item     : JumpStatements
// Topic id : stage04/section01/jump_statements
//
// 步骤 3：break / continue / return / goto；goto case 见 SwitchStatement。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section01;

internal static class JumpStatements
{
    [LearnTopic("stage04/section01/jump_statements")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== JumpStatements ===");
        DemoBreak();
        DemoContinue();
        DemoReturn();
        DemoGotoLoop();
        DemoGotoOutOfNested();
        return 0;
    }

    private static void DemoBreak()
    {
        Console.WriteLine("-- break：终止最近循环 --");
        int[] nums = [1, 2, -1, 4];
        List<int> taken = new List<int>();
        foreach (int n in nums)
        {
            if (n < 0)
                break;
            taken.Add(n);
        }
        Debug.Assert(taken is [1, 2]);
        Console.WriteLine($"  遇负 break → [{string.Join(",", taken)}]");
    }

    private static void DemoContinue()
    {
        Console.WriteLine("-- continue：跳过本次迭代 --");
        List<int> odds = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            if (i % 2 == 0)
                continue;
            odds.Add(i);
        }
        Debug.Assert(string.Join("", odds) == "13579");
        Console.WriteLine($"  跳过偶数 → {string.Join("", odds)}");
    }

    private static void DemoReturn()
    {
        Console.WriteLine("-- return：卫语句尽早返回 --");
        static int Find(int[] a, int target)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == target)
                    return i;
            }
            return -1;
        }

        int[] data = [10, 20, 30];
        Debug.Assert(Find(data, 20) == 1);
        Debug.Assert(Find(data, 99) == -1);
        Console.WriteLine($"  Find 20→1, 99→-1");
    }

    private static void DemoGotoLoop()
    {
        Console.WriteLine("-- goto 标签（慎用） --");
        int j = 0;
        List<int> buf = new List<int>();
    loop:
        if (j < 3)
        {
            buf.Add(j);
            j++;
            goto loop;
        }
        Debug.Assert(string.Join("", buf) == "012");
        Console.WriteLine($"  goto loop → {string.Join("", buf)}");
    }

    private static void DemoGotoOutOfNested()
    {
        Console.WriteLine("-- goto 跳出嵌套循环 --");
        int found = -1;
        int[,] grid =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
        };

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                if (grid[r, c] == 5)
                {
                    found = r * 10 + c;
                    goto done;
                }
            }
        }
    done:
        Debug.Assert(found == 11); // row1,col1
        Console.WriteLine($"  嵌套中找到 5 → pos code {found}（也可用方法+return）");
    }
}
