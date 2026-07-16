// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第1部分-集合体系.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section01_CollectionEcosystem
// Item     : ConcurrentCollections
// Topic id : stage05/section01/concurrent_collections
//
// 步骤 7：ConcurrentDictionary / Queue / Stack / Bag + 原子复合操作。

using System.Collections.Concurrent;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section01;

internal static class ConcurrentCollections
{
    [LearnTopic("stage05/section01/concurrent_collections")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ConcurrentCollections ===");
        DemoConcurrentDictionary();
        DemoQueueStackBag();
        DemoParallelCount();
        DemoBlockingCollectionNote();
        return 0;
    }

    private static void DemoConcurrentDictionary()
    {
        Console.WriteLine("-- ConcurrentDictionary：TryAdd / GetOrAdd / AddOrUpdate --");
        ConcurrentDictionary<string, int> dict = new();
        Debug.Assert(dict.TryAdd("a", 1));
        Debug.Assert(!dict.TryAdd("a", 9));
        int got = dict.GetOrAdd("b", _ => 42);
        Debug.Assert(got == 42 && dict["b"] == 42);
        int updated = dict.AddOrUpdate("a", 1, (_, old) => old + 1);
        Debug.Assert(updated == 2 && dict["a"] == 2);
        Console.WriteLine($"  a={dict["a"]}, b={dict["b"]}");
    }

    private static void DemoQueueStackBag()
    {
        Console.WriteLine("-- ConcurrentQueue / Stack / Bag --");
        ConcurrentQueue<int> q = new();
        q.Enqueue(1);
        q.Enqueue(2);
        Debug.Assert(q.TryDequeue(out int x) && x == 1);

        ConcurrentStack<int> s = new();
        s.Push(10);
        s.Push(20);
        bool popped = s.TryPop(out int y);
        Debug.Assert(popped && y == 20);

        ConcurrentBag<int> bag = [1, 2, 3];
        Debug.Assert(bag.Count == 3);
        Console.WriteLine($"  queue next after Dequeue ok; stack Pop={y}; bag.Count={bag.Count}");
    }

    private static void DemoParallelCount()
    {
        Console.WriteLine("-- 多线程 AddOrUpdate 计数 --");
        ConcurrentDictionary<string, int> counts = new();
        Parallel.For(0, 1000, _ =>
        {
            counts.AddOrUpdate("hits", 1, (_, old) => old + 1);
        });
        Debug.Assert(counts["hits"] == 1000);
        Console.WriteLine($"  parallel hits={counts["hits"]}");
    }

    private static void DemoBlockingCollectionNote()
    {
        Console.WriteLine("-- BlockingCollection：生产者-消费者阻塞队列 --");
        using BlockingCollection<int> bc = new(boundedCapacity: 2);
        bc.Add(1);
        bc.Add(2);
        Debug.Assert(bc.TryTake(out int v) && v == 1);
        bc.CompleteAdding();
        Console.WriteLine("  bounded + CompleteAdding 支持产消模式");
    }
}
