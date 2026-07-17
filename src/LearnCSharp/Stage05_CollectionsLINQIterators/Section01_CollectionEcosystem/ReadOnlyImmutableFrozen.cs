// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第1部分-集合体系.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section01_CollectionEcosystem
// Item     : ReadOnlyImmutableFrozen
// Topic id : stage05/section01/readonly_immutable_frozen
//
// 步骤 6：只读视图 vs 不可变 vs 冻结（三层）。

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section01;

internal static class ReadOnlyImmutableFrozen
{
    [LearnTopic("stage05/section01/readonly_immutable_frozen")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReadOnlyImmutableFrozen ===");
        DemoReadOnlyViewNotSnapshot();
        DemoImmutableReturnsNew();
        DemoImmutableBuilder();
        DemoFrozenLookup();
        DemoCompareLayers();
        return 0;
    }

    private static void DemoReadOnlyViewNotSnapshot()
    {
        Console.WriteLine("-- ReadOnlyCollection：视图，底层变跟着变 --");
        List<int> source = [1, 2, 3];
        ReadOnlyCollection<int> view = source.AsReadOnly();
        Debug.Assert(view.Count == 3);
        source.Add(4);
        Debug.Assert(view.Count == 4);
        Console.WriteLine($"  source.Add(4) → view.Count={view.Count}（非快照）");
    }

    private static void DemoImmutableReturnsNew()
    {
        Console.WriteLine("-- ImmutableList.Add 返回新实例，原集合不动 --");
        ImmutableList<int> a = ImmutableList.Create(1, 2, 3);
        ImmutableList<int> b = a.Add(4);
        Debug.Assert(a.Count == 3 && b.Count == 4);
        Debug.Assert(a is [1, 2, 3]);
        Console.WriteLine($"  a.Count={a.Count}, b.Count={b.Count}");
    }

    private static void DemoImmutableBuilder()
    {
        Console.WriteLine("-- Builder 批量构建再 ToImmutable --");
        ImmutableList<int>.Builder builder = ImmutableList.CreateBuilder<int>();
        for (int i = 0; i < 5; i++)
            builder.Add(i);
        ImmutableList<int> list = builder.ToImmutable();
        Debug.Assert(list.Count == 5 && list[4] == 4);
        Console.WriteLine($"  built [{string.Join(", ", list)}]");
    }

    private static void DemoFrozenLookup()
    {
        Console.WriteLine("-- FrozenSet / FrozenDictionary：建一次，读极快 --");
        FrozenSet<int> fs = new[] { 1, 2, 3 }.ToFrozenSet();
        Debug.Assert(fs.Contains(2) && !fs.Contains(9));

        Dictionary<string, int> dict = new() { ["a"] = 1, ["b"] = 2 };
        FrozenDictionary<string, int> fd = dict.ToFrozenDictionary();
        Debug.Assert(fd["a"] == 1 && fd.TryGetValue("b", out int bv) && bv == 2);
        Console.WriteLine($"  FrozenSet has 2={fs.Contains(2)}; FrozenDict a={fd["a"]}");
    }

    private static void DemoCompareLayers()
    {
        Console.WriteLine("-- 三层：视图 / 真不可变 / 预计算只读 --");
        Console.WriteLine("  ReadOnly=view(非线程安全); Immutable=新实例+线程安全; Frozen=建贵读最快");
    }
}
