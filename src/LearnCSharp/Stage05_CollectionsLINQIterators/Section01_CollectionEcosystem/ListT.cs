// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第1部分-集合体系.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section01_CollectionEcosystem
// Item     : ListT
// Topic id : stage05/section01/list_t
//
// 步骤 2：List<T> 动态数组 ≈ std::vector（命名陷阱：不是链表）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section01;

internal static class ListT
{
    [LearnTopic("stage05/section01/list_t")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ListT ===");
        DemoBasicOps();
        DemoCountVsCapacity();
        DemoReserveCapacity();
        DemoNamingTrap();
        return 0;
    }

    private static void DemoBasicOps()
    {
        Console.WriteLine("-- Add / Insert / 索引 / RemoveAt / Contains / Sort --");
        List<int> list = [1, 2, 3];
        list.Add(4);
        list.Insert(0, 0);
        list[2] = 99;
        list.RemoveAt(1);
        Debug.Assert(list.Contains(99));
        Debug.Assert(list.IndexOf(99) >= 0);
        list.Sort();
        Debug.Assert(list is [0, 3, 4, 99] or [0, 3, 4, 99]);
        Console.WriteLine($"  after ops+Sort → [{string.Join(", ", list)}]");
    }

    private static void DemoCountVsCapacity()
    {
        Console.WriteLine("-- Count vs Capacity（底层数组翻倍） --");
        List<int> list = [];
        int prev = list.Capacity;
        List<int> capacities = new List<int> { prev };
        for (int i = 0; i < 20; i++)
        {
            list.Add(i);
            if (list.Capacity != prev)
            {
                capacities.Add(list.Capacity);
                prev = list.Capacity;
            }
        }
        Debug.Assert(list.Count == 20);
        Debug.Assert(list.Capacity >= list.Count);
        Console.WriteLine($"  Count={list.Count}, Capacity={list.Capacity}, growth={string.Join("→", capacities)}");
    }

    private static void DemoReserveCapacity()
    {
        Console.WriteLine("-- new List(capacity) 预留，避免反复扩容 --");
        List<int> reserved = new(1000);
        Debug.Assert(reserved.Count == 0 && reserved.Capacity >= 1000);
        for (int i = 0; i < 100; i++)
            reserved.Add(i);
        Debug.Assert(reserved.Capacity >= 1000);
        Console.WriteLine($"  reserved Capacity stays ≥1000 after 100 Adds: {reserved.Capacity}");
    }

    private static void DemoNamingTrap()
    {
        Console.WriteLine("-- ⚠ C# List<T> ≈ vector，不是链表；链表是 LinkedList<T> --");
        List<int> vectorLike = [1, 2, 3];
        Debug.Assert(vectorLike[1] == 2);
        Console.WriteLine("  List[i] O(1) 连续内存；C++ std::list = C# LinkedList");
    }
}
