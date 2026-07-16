// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第1部分-集合体系.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section01_CollectionEcosystem
// Item     : HashSetAndOrdered
// Topic id : stage05/section01/hashset_and_ordered
//
// 步骤 4：HashSet + SortedSet / SortedDictionary / SortedList。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section01;

internal static class HashSetAndOrdered
{
    [LearnTopic("stage05/section01/hashset_and_ordered")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== HashSetAndOrdered ===");
        DemoHashSetBasics();
        DemoSetOperations();
        DemoMemberTestNotDictBool();
        DemoSortedSetAndMaps();
        return 0;
    }

    private static void DemoHashSetBasics()
    {
        Console.WriteLine("-- HashSet：去重 + O(1) Contains --");
        HashSet<int> set = [1, 2, 3, 2];
        Debug.Assert(set.Count == 3);
        Debug.Assert(set.Add(4));
        Debug.Assert(!set.Add(4));
        Debug.Assert(set.Contains(2));
        Debug.Assert(set.Remove(1));
        Console.WriteLine($"  set → [{string.Join(", ", set.Order())}]");
    }

    private static void DemoSetOperations()
    {
        Console.WriteLine("-- ISet：UnionWith / IntersectWith / ExceptWith --");
        HashSet<int> a = [1, 2, 3];
        HashSet<int> b = [3, 4, 5];
        HashSet<int> u = [.. a];
        u.UnionWith(b);
        Debug.Assert(u.SetEquals([1, 2, 3, 4, 5]));

        HashSet<int> i = [.. a];
        i.IntersectWith(b);
        Debug.Assert(i.SetEquals([3]));

        HashSet<int> e = [.. a];
        e.ExceptWith(b);
        Debug.Assert(e.SetEquals([1, 2]));
        Console.WriteLine($"  union={{{string.Join(",", u.Order())}}} inter={{{string.Join(",", i)}}} except={{{string.Join(",", e.Order())}}}");
    }

    private static void DemoMemberTestNotDictBool()
    {
        Console.WriteLine("-- 成员测试用 HashSet，别用 Dictionary<T,bool> --");
        HashSet<string> seen = ["a", "b"];
        Debug.Assert(seen.Contains("a"));
        Console.WriteLine("  HashSet 语义准、更省内存");
    }

    private static void DemoSortedSetAndMaps()
    {
        Console.WriteLine("-- SortedSet / SortedDictionary / SortedList --");
        SortedSet<int> ss = [3, 1, 2];
        Debug.Assert(ss.SequenceEqual([1, 2, 3]));

        SortedDictionary<string, int> sd = new() { ["c"] = 3, ["a"] = 1, ["b"] = 2 };
        Debug.Assert(sd.Keys.SequenceEqual(["a", "b", "c"]));

        SortedList<string, int> sl = new() { ["z"] = 9, ["m"] = 5, ["a"] = 1 };
        Debug.Assert(sl.Keys[0] == "a" && sl.Keys[^1] == "z");
        Console.WriteLine($"  SortedSet={string.Join(",", ss)}; SortedDict keys={string.Join(",", sd.Keys)}");
    }
}
