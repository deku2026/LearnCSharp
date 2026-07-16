// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第2部分-迭代器.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section02_Iterators
// Item     : IteratorLimitationsAndPitfalls
// Topic id : stage05/section02/iterator_limitations_and_pitfalls
//
// 步骤 5：yield 限制、lock 危险、重复枚举重跑。

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section02;

internal static class IteratorLimitationsAndPitfalls
{
    [LearnTopic("stage05/section02/iterator_limitations_and_pitfalls")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IteratorLimitationsAndPitfalls ===");
        DemoYieldBreakNotReturnValue();
        DemoTryFinallyOk();
        DemoReenumerateReruns();
        DemoMaterializeOnce();
        DemoCaptureChangingState();
        return 0;
    }

    private static void DemoYieldBreakNotReturnValue()
    {
        Console.WriteLine("-- 迭代器只能 yield return/break，不能 return 值（CS1622） --");
        // return 5; 在迭代器里会 CS1622
        Debug.Assert(OnlyYield().SequenceEqual([1]));
        Console.WriteLine("  yield return 1 合法；return 值非法");
    }

    private static IEnumerable<int> OnlyYield()
    {
        yield return 1;
        yield break;
    }

    private static void DemoTryFinallyOk()
    {
        Console.WriteLine("-- try-finally 可 yield；带 catch 的 try 不能 yield --");
        StringBuilder log = new();
        foreach (int n in WithFinally(log))
            log.Append($"n{n};");
        Debug.Assert(log.ToString().Contains("finally;") && log.ToString().Contains("n1;"));
        Console.WriteLine($"  log={log}");
    }

    private static IEnumerable<int> WithFinally(StringBuilder log)
    {
        try
        {
            yield return 1;
            yield return 2;
        }
        finally
        {
            log.Append("finally;");
        }
    }

    private static void DemoReenumerateReruns()
    {
        Console.WriteLine("-- ⚠ 重复枚举 = 重新执行整条链 --");
        int checks = 0;
        IEnumerable<int> query = Enumerable.Range(0, 10)
            .Where(x =>
            {
                checks++;
                return x % 2 == 0;
            })
            .Take(2);
        List<int> a = query.ToList();
        int afterFirst = checks;
        List<int> b = query.ToList();
        Debug.Assert(a.SequenceEqual([0, 2]) && b.SequenceEqual([0, 2]));
        Debug.Assert(checks == afterFirst * 2);
        Console.WriteLine($"  two ToList → checks {afterFirst} then {checks} (doubled)");
    }

    private static void DemoMaterializeOnce()
    {
        Console.WriteLine("-- 需要复用：ToList 物化一次 --");
        int checks = 0;
        List<int> snapshot = Enumerable.Range(0, 10)
            .Where(x =>
            {
                checks++;
                return x % 2 == 0;
            })
            .Take(2)
            .ToList();
        int afterBuild = checks;
        _ = snapshot.ToList();
        _ = snapshot.Count;
        Debug.Assert(checks == afterBuild);
        Console.WriteLine($"  snapshot reused, checks stayed {checks}");
    }

    private static void DemoCaptureChangingState()
    {
        Console.WriteLine("-- 捕获会变的外部状态 → 两次枚举结果可能不同 --");
        int threshold = 5;
        IEnumerable<int> q = Enumerable.Range(0, 10).Where(x => x > threshold);
        List<int> first = q.ToList();
        threshold = 8;
        List<int> second = q.ToList();
        Debug.Assert(first.Count == 4 && second.Count == 1);
        Console.WriteLine($"  threshold 5→{first.Count} items; threshold 8→{second.Count} items");
    }
}
