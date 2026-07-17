// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第2部分-迭代器.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section02_Iterators
// Item     : LazyEvaluationPauseResume
// Topic id : stage05/section02/lazy_evaluation_pause_resume
//
// 步骤 2：惰性求值——调用不执行、遍历才跑、无限序列。

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section02;

internal static class LazyEvaluationPauseResume
{
    [LearnTopic("stage05/section02/lazy_evaluation_pause_resume")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== LazyEvaluationPauseResume ===");
        DemoCallDoesNotExecute();
        DemoPauseResumeOrder();
        DemoInfiniteWithTake();
        DemoStreamingLines();
        return 0;
    }

    private static void DemoCallDoesNotExecute()
    {
        Console.WriteLine("-- 调用迭代器方法不执行方法体 --");
        int started = 0;
        IEnumerable<int> seq = Numbers(() => started++);
        Debug.Assert(started == 0);
        Console.WriteLine($"  after Numbers(): started={started}");
        _ = seq.ToList();
        Debug.Assert(started == 1);
        Console.WriteLine($"  after ToList(): started={started}");
    }

    private static IEnumerable<int> Numbers(Action onStart)
    {
        onStart();
        yield return 1;
        yield return 2;
    }

    private static void DemoPauseResumeOrder()
    {
        Console.WriteLine("-- 暂停/恢复：每次 MoveNext 从上次 yield 后继续 --");
        StringBuilder log = new();
        foreach (int n in LoggedNumbers(log))
            log.Append($"got{n};");
        string s = log.ToString();
        Debug.Assert(s.Contains("start;") && s.Contains("after1;") && s.Contains("got1;"));
        Console.WriteLine($"  log={s}");
    }

    private static IEnumerable<int> LoggedNumbers(StringBuilder log)
    {
        log.Append("start;");
        yield return 1;
        log.Append("after1;");
        yield return 2;
        log.Append("after2;");
    }

    private static void DemoInfiniteWithTake()
    {
        Console.WriteLine("-- 无限 Naturals + Take(5) 不死循环 --");
        List<int> five = Naturals().Take(5).ToList();
        Debug.Assert(five.SequenceEqual([0, 1, 2, 3, 4]));
        Console.WriteLine($"  Naturals().Take(5) → [{string.Join(", ", five)}]");
    }

    private static IEnumerable<int> Naturals()
    {
        int i = 0;
        while (true)
            yield return i++;
    }

    private static void DemoStreamingLines()
    {
        Console.WriteLine("-- 惰性逐行：不全量加载 --");
        string path = Path.Combine(Path.GetTempPath(), "learncsharp-stage05-lines.txt");
        File.WriteAllText(path, "alpha\nbeta\ngamma\n");
        try
        {
            int count = 0;
            foreach (string line in ReadLines(path))
            {
                count++;
                if (count == 2)
                    break;
            }
            Debug.Assert(count == 2);
            List<string> all = ReadLines(path).ToList();
            Debug.Assert(all.Count == 3 && all[0] == "alpha");
            Console.WriteLine($"  streamed 2 then full={all.Count} lines");
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static IEnumerable<string> ReadLines(string path)
    {
        using StreamReader reader = new(path);
        string? line;
        while ((line = reader.ReadLine()) is not null)
            yield return line;
    }
}
