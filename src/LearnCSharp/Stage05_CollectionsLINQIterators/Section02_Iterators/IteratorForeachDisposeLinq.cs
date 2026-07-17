// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第2部分-迭代器.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section02_Iterators
// Item     : IteratorForeachDisposeLinq
// Topic id : stage05/section02/iterator_foreach_dispose_linq
//
// 步骤 4：迭代器 × foreach Dispose × LINQ 延迟基础。

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section02;

internal static class IteratorForeachDisposeLinq
{
    [LearnTopic("stage05/section02/iterator_foreach_dispose_linq")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IteratorForeachDisposeLinq ===");
        DemoForeachDisposesOnBreak();
        DemoMyWhere();
        DemoLinqDeferred();
        DemoManualWithoutDispose();
        return 0;
    }

    private static void DemoForeachDisposesOnBreak()
    {
        Console.WriteLine("-- foreach break 仍调用迭代器 Dispose → using 清理 --");
        StringBuilder log = new();
        foreach (string line in TrackedLines(log, ["a", "b", "c", "d"]))
        {
            log.Append($"use:{line};");
            if (line == "b")
                break;
        }
        string s = log.ToString();
        Debug.Assert(s.Contains("dispose;") && s.Contains("use:b;"));
        Console.WriteLine($"  log={s}");
    }

    private static IEnumerable<string> TrackedLines(StringBuilder log, string[] lines)
    {
        using TrackingResource res = new(log);
        foreach (string line in lines)
            yield return line;
    }

    private static void DemoMyWhere()
    {
        Console.WriteLine("-- 自实现 MyWhere（yield = LINQ Where 内核） --");
        int[] nums = [1, -2, 3, -4, 5];
        List<int> pos = MyWhere(nums, x => x > 0).ToList();
        Debug.Assert(pos.SequenceEqual([1, 3, 5]));
        Console.WriteLine($"  MyWhere(>0) → [{string.Join(", ", pos)}]");
    }

    private static IEnumerable<T> MyWhere<T>(IEnumerable<T> src, Func<T, bool> pred)
    {
        foreach (T x in src)
        {
            if (pred(x))
                yield return x;
        }
    }

    private static void DemoLinqDeferred()
    {
        Console.WriteLine("-- LINQ 定义不执行，枚举才跑（迭代器惰性） --");
        int checks = 0;
        int[] nums = [1, 2, 3, 4];
        IEnumerable<int> query = nums
            .Where(x => { checks++; return x > 0; })
            .Select(x => x * x);
        Debug.Assert(checks == 0);
        List<int> list = query.ToList();
        Debug.Assert(checks == 4 && list.SequenceEqual([1, 4, 9, 16]));
        Console.WriteLine($"  checks after define=0, after ToList={checks}");
    }

    private static void DemoManualWithoutDispose()
    {
        Console.WriteLine("-- 手动 MoveNext 须 Dispose；foreach 自动保证 --");
        StringBuilder log = new();
        IEnumerator<string> e = TrackedLines(log, ["x", "y"]).GetEnumerator();
        try
        {
            Debug.Assert(e.MoveNext());
            Debug.Assert(e.Current == "x");
        }
        finally
        {
            e.Dispose();
        }
        Debug.Assert(log.ToString().Contains("dispose;"));
        Console.WriteLine("  manual path disposed via finally");
    }

    private sealed class TrackingResource(StringBuilder log) : IDisposable
    {
        private readonly StringBuilder _log = log;
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _log.Append("dispose;");
        }
    }
}
