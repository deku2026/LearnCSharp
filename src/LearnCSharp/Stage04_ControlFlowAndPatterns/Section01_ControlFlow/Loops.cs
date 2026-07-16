// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第1部分-控制流.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section01_ControlFlow
// Item     : Loops
// Topic id : stage04/section01/loops
//
// 步骤 2：while / do-while / for / foreach + GetEnumerator 机制。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section01;

internal static class Loops
{
    [LearnTopic("stage04/section01/loops")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Loops ===");
        DemoWhile();
        DemoDoWhile();
        DemoFor();
        DemoForeach();
        DemoForeachMechanism();
        DemoCustomGetEnumerator();
        DemoModifyDuringEnumerate();
        return 0;
    }

    private static void DemoWhile()
    {
        Console.WriteLine("-- while：顶判，0 次或多次 --");
        int i = 0;
        var buf = new List<int>();
        while (i < 5)
        {
            buf.Add(i);
            i++;
        }
        Debug.Assert(string.Join("", buf) == "01234");
        Console.WriteLine($"  while 0..4 → {string.Join("", buf)}");
    }

    private static void DemoDoWhile()
    {
        Console.WriteLine("-- do-while：底判，至少 1 次 --");
        int j = 0;
        var buf = new List<int>();
        do
        {
            buf.Add(j);
            j++;
        } while (j < 5);
        Debug.Assert(string.Join("", buf) == "01234");

        int once = 0;
        do { once++; } while (false);
        Debug.Assert(once == 1);
        Console.WriteLine($"  do-while 0..4 → {string.Join("", buf)}; false 条件仍执行 1 次");
    }

    private static void DemoFor()
    {
        Console.WriteLine("-- for：初始化; 条件; 迭代 --");
        var buf = new List<int>();
        for (int k = 0; k < 5; k++)
            buf.Add(k);
        Debug.Assert(string.Join("", buf) == "01234");
        Console.WriteLine($"  for 0..4 → {string.Join("", buf)}");
    }

    private static void DemoForeach()
    {
        Console.WriteLine("-- foreach：遍历元素 --");
        int[] nums = [10, 20, 30];
        var buf = new List<int>();
        foreach (int n in nums)
            buf.Add(n);
        Debug.Assert(buf is [10, 20, 30]);

        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        int sum = 0;
        foreach (var (key, value) in dict)
            sum += value;
        Debug.Assert(sum == 3);
        Console.WriteLine($"  foreach array → [{string.Join(",", buf)}]; dict sum={sum}");
    }

    private static void DemoForeachMechanism()
    {
        Console.WriteLine("-- foreach ≈ GetEnumerator + MoveNext + Current --");
        int[] nums = [1, 2, 3];
        var e = ((IEnumerable<int>)nums).GetEnumerator();
        var manual = new List<int>();
        try
        {
            while (e.MoveNext())
                manual.Add(e.Current);
        }
        finally
        {
            e.Dispose();
        }
        Debug.Assert(manual is [1, 2, 3]);
        Console.WriteLine("  手写 MoveNext/Current 与 foreach 等价");
    }

    private static void DemoCustomGetEnumerator()
    {
        Console.WriteLine("-- 自定义 GetEnumerator（鸭子类型） --");
        var bag = new NumberBag([7, 8, 9]);
        var got = new List<int>();
        foreach (int n in bag)
            got.Add(n);
        Debug.Assert(got is [7, 8, 9]);
        Console.WriteLine($"  NumberBag foreach → [{string.Join(",", got)}]");
    }

    private static void DemoModifyDuringEnumerate()
    {
        Console.WriteLine("-- 枚举期间改集合 → InvalidOperationException --");
        var list = new List<int> { 1, 2, 3 };
        bool threw = false;
        try
        {
            foreach (int n in list)
            {
                if (n == 1)
                    list.Add(99);
            }
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }
        Debug.Assert(threw);
        Console.WriteLine("  foreach 中 list.Add → InvalidOperationException ✓");
    }

    private sealed class NumberBag(int[] items)
    {
        private readonly int[] _items = items;

        public Enumerator GetEnumerator() => new(_items);

        public struct Enumerator
        {
            private readonly int[] _items;
            private int _index;

            public Enumerator(int[] items)
            {
                _items = items;
                _index = -1;
            }

            public int Current => _items[_index];

            public bool MoveNext()
            {
                _index++;
                return _index < _items.Length;
            }
        }
    }
}
