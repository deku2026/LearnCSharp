// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第3部分-LINQ全谱.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section03_LinqFullSpectrum
// Item     : DeferredVsImmediateExecution
// Topic id : stage05/section03/deferred_vs_immediate_execution
//
// 步骤 2：延迟 vs 立即执行；查询复用 vs 结果复用。

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section03;

internal static class DeferredVsImmediateExecution
{
    [LearnTopic("stage05/section03/deferred_vs_immediate_execution")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DeferredVsImmediateExecution ===");
        DemoDeferredDefinition();
        DemoImmediateScalars();
        DemoQueryVsResultReuse();
        DemoStreamingVsNonStreaming();
        return 0;
    }

    private static void DemoDeferredDefinition()
    {
        Console.WriteLine("-- 定义 Where 不执行，foreach 才检查 --");
        int[] nums = [1, 5, 8, 3];
        StringBuilder log = new();
        IEnumerable<int> query = nums.Where(n =>
        {
            log.Append($"check{n};");
            return n > 4;
        });
        Debug.Assert(log.Length == 0);
        List<int> got = [];
        foreach (int x in query)
            got.Add(x);
        Debug.Assert(log.ToString().Contains("check1;") && got.SequenceEqual([5, 8]));
        Console.WriteLine($"  log={log} got=[{string.Join(",", got)}]");
    }

    private static void DemoImmediateScalars()
    {
        Console.WriteLine("-- 标量运算符立即执行 --");
        int[] nums = [1, 5, 8, 3];
        int count = nums.Count(n => n > 4);
        int max = nums.Max();
        List<int> list = nums.Where(n => n > 4).ToList();
        Debug.Assert(count == 2 && max == 8 && list.SequenceEqual([5, 8]));
        Console.WriteLine($"  Count={count}, Max={max}, ToList=[{string.Join(",", list)}]");
    }

    private static void DemoQueryVsResultReuse()
    {
        Console.WriteLine("-- 延迟=查询复用（看最新源）；ToList=结果快照 --");
        List<int> source = [1, 2, 3];
        IEnumerable<int> deferred = source.Where(x => x > 1);
        List<int> snapshot = source.Where(x => x > 1).ToList();
        source.Add(10);
        List<int> deferredNow = deferred.ToList();
        Debug.Assert(deferredNow.SequenceEqual([2, 3, 10]));
        Debug.Assert(snapshot.SequenceEqual([2, 3]));
        Console.WriteLine($"  deferred→[{string.Join(",", deferredNow)}] snapshot→[{string.Join(",", snapshot)}]");
    }

    private static void DemoStreamingVsNonStreaming()
    {
        Console.WriteLine("-- streaming Where vs non-streaming OrderBy --");
        int checks = 0;
        int[] nums = [3, 1, 4, 2];
        IEnumerable<int> stream = nums.Where(n =>
        {
            checks++;
            return n > 0;
        }).Take(2);
        List<int> two = stream.ToList();
        Debug.Assert(two.Count == 2 && checks == 2);

        checks = 0;
        List<int> ordered = nums.OrderBy(n =>
        {
            checks++;
            return n;
        }).Take(1).ToList();
        Debug.Assert(ordered is [1] && checks == 4);
        Console.WriteLine($"  Where+Take checks={2} (stream); OrderBy+Take key-evals={checks} (need all)");
    }
}
