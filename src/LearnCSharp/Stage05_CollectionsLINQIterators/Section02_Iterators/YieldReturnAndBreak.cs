// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第2部分-迭代器.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section02_Iterators
// Item     : YieldReturnAndBreak
// Topic id : stage05/section02/yield_return_and_break
//
// 步骤 1：yield return / yield break 基本用法。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section02;

internal static class YieldReturnAndBreak
{
    [LearnTopic("stage05/section02/yield_return_and_break")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== YieldReturnAndBreak ===");
        DemoCountToThree();
        DemoYieldBreak();
        DemoEmptyIterator();
        DemoCustomReverse();
        return 0;
    }

    private static void DemoCountToThree()
    {
        Console.WriteLine("-- yield return 三个值 --");
        List<int> viaForeach = [];
        foreach (int n in CountToThree())
            viaForeach.Add(n);
        List<int> viaToList = CountToThree().ToList();
        Debug.Assert(viaForeach.SequenceEqual([1, 2, 3]));
        Debug.Assert(viaToList.SequenceEqual([1, 2, 3]));
        Console.WriteLine($"  foreach+ToList → [{string.Join(", ", viaToList)}]");
    }

    private static IEnumerable<int> CountToThree()
    {
        yield return 1;
        yield return 2;
        yield return 3;
    }

    private static void DemoYieldBreak()
    {
        Console.WriteLine("-- for + yield break：小于 N 的十的倍数 --");
        List<int> got = MultiplesOfTenUnder(35).ToList();
        Debug.Assert(got.SequenceEqual([10, 20, 30]));
        Console.WriteLine($"  MultiplesOfTenUnder(35) → [{string.Join(", ", got)}]");
    }

    private static IEnumerable<int> MultiplesOfTenUnder(int limit)
    {
        for (int i = 1; ; i++)
        {
            int tens = i * 10;
            if (tens >= limit)
                yield break;
            yield return tens;
        }
    }

    private static void DemoEmptyIterator()
    {
        Console.WriteLine("-- 仅 yield break → 空序列 --");
        Debug.Assert(!Empty().Any());
        Console.WriteLine("  Empty().Any() == false");
    }

    private static IEnumerable<int> Empty()
    {
        yield break;
    }

    private static void DemoCustomReverse()
    {
        Console.WriteLine("-- 自定义类：逆序迭代器 --");
        Buffer buf = new([10, 20, 30]);
        Debug.Assert(buf.ReverseItems().SequenceEqual([30, 20, 10]));
        Console.WriteLine($"  ReverseItems → [{string.Join(", ", buf.ReverseItems())}]");
    }

    private sealed class Buffer(int[] items)
    {
        private readonly int[] _items = items;

        public IEnumerable<int> ReverseItems()
        {
            for (int i = _items.Length - 1; i >= 0; i--)
                yield return _items[i];
        }
    }
}
