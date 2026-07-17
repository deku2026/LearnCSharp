// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第7部分-GC原理与堆结构.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section07_GCPrinciplesAndHeap
// Item     : HeapStructureGen0Gen1Gen2
// Topic id : stage11/section07/heap_structure_gen0_gen1_gen2
//
// Lesson: ephemeral Gen0/1 vs Gen2; regions vs segments; promotion path.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section07;

internal static class HeapStructureGen0Gen1Gen2
{
    [LearnTopic("stage11/section07/heap_structure_gen0_gen1_gen2")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== HeapStructureGen0Gen1Gen2 ===");
        DemoStructure();
        DemoPromotionPath();
        DemoMemoryInfo();
        return 0;
    }

    private static void DemoStructure()
    {
        Console.WriteLine("-- heap structure --");
        Console.WriteLine("  Gen0: nursery, bump-pointer alloc");
        Console.WriteLine("  Gen1: buffer between young and old");
        Console.WriteLine("  Gen2: long-lived; full GC covers Gen2 + LOH/POH");
        Console.WriteLine("  .NET 6+: regions allocator (vs classic segments)");
        Console.WriteLine($"  Collection counts G0={GC.CollectionCount(0)} G1={GC.CollectionCount(1)} G2={GC.CollectionCount(2)}");
    }

    private static void DemoPromotionPath()
    {
        Console.WriteLine("-- promotion path demo --");
        object o = new byte[128];
        int[] gens = new int[4];
        gens[0] = GC.GetGeneration(o);
        for (int i = 1; i <= 3; i++)
        {
            GC.Collect(i - 1);
            gens[i] = GC.GetGeneration(o);
        }

        Console.WriteLine($"  generation trail: {string.Join(" → ", gens)}");
        Debug.Assert(gens[^1] >= gens[0]);
        GC.KeepAlive(o);
    }

    private static void DemoMemoryInfo()
    {
        Console.WriteLine("-- GC.GetGCMemoryInfo snapshot --");
        GCMemoryInfo info = GC.GetGCMemoryInfo();
        Console.WriteLine($"  HeapSizeBytes={info.HeapSizeBytes}");
        Console.WriteLine($"  MemoryLoadBytes={info.MemoryLoadBytes}");
        Console.WriteLine($"  TotalAvailableMemoryBytes={info.TotalAvailableMemoryBytes}");
        Console.WriteLine($"  FragmentedBytes={info.FragmentedBytes}");
        Debug.Assert(info.HeapSizeBytes >= 0);
    }
}
