// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第1部分-集合体系.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section01_CollectionEcosystem
// Item     : ConcurrentCollections
// Topic id : stage05/section01/concurrent_collections
//
// 步骤 7：ConcurrentDictionary / Queue / Stack / Bag + 原子复合操作。
// 对比：List/Dictionary 在 Parallel.For 下的竞态 vs ConcurrentDictionary。

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
        DemoRaceOnListAndDictionaryVsConcurrent();
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

    private static void DemoRaceOnListAndDictionaryVsConcurrent()
    {
        Console.WriteLine("-- race: List/Dictionary + Parallel.For vs ConcurrentDictionary --");
        const int n = 5000;

        // List.Add is not thread-safe → IndexOutOfRange / ArgumentException / lost items.
        List<int> unsafeList = [];
        int listExceptions = 0;
        try
        {
            Parallel.For(0, n, i =>
            {
                try
                {
                    unsafeList.Add(i);
                }
                catch (Exception)
                {
                    Interlocked.Increment(ref listExceptions);
                }
            });
        }
        catch (AggregateException ae)
        {
            listExceptions += ae.Flatten().InnerExceptions.Count;
        }

        bool listCorrupt = listExceptions > 0 || unsafeList.Count != n;
        Console.WriteLine($"  List.Add parallel: Count={unsafeList.Count}/{n}, caught={listExceptions}, corrupt/incomplete={listCorrupt}");

        // Dictionary indexer concurrent write → often throws or loses updates.
        Dictionary<int, int> unsafeDict = [];
        int dictExceptions = 0;
        try
        {
            Parallel.For(0, n, i =>
            {
                try
                {
                    unsafeDict[i] = i;
                }
                catch (Exception)
                {
                    Interlocked.Increment(ref dictExceptions);
                }
            });
        }
        catch (AggregateException ae)
        {
            dictExceptions += ae.Flatten().InnerExceptions.Count;
        }

        bool dictCorrupt = dictExceptions > 0 || unsafeDict.Count != n;
        Console.WriteLine($"  Dictionary[] parallel: Count={unsafeDict.Count}/{n}, caught={dictExceptions}, corrupt/incomplete={dictCorrupt}");

        // ConcurrentDictionary is safe for concurrent writes.
        ConcurrentDictionary<int, int> safe = new();
        Parallel.For(0, n, i => safe[i] = i);
        Debug.Assert(safe.Count == n);
        Console.WriteLine($"  ConcurrentDictionary parallel: Count={safe.Count}/{n} ✓");

        // At least one of the unsafe structures should show a problem on a busy machine;
        // if not (lucky schedule), still document the contract.
        if (!listCorrupt && !dictCorrupt)
            Console.WriteLine("  (schedule luck: no throw this run — still undefined behavior without sync)");
        else
            Debug.Assert(listCorrupt || dictCorrupt);
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
