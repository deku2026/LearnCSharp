// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第7部分-GC原理与堆结构.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section07_GCPrinciplesAndHeap
// Item     : MarkSweepCompactRoots
// Topic id : stage11/section07/mark_sweep_compact_roots
//
// Lesson: mark from roots, plan, compact or sweep; unreachable objects reclaimed.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section07;

internal static class MarkSweepCompactRoots
{
    [LearnTopic("stage11/section07/mark_sweep_compact_roots")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MarkSweepCompactRoots ===");
        DemoRoots();
        DemoCyclicReferenceReclaimed();
        DemoWeakRefNotRoot();
        DemoGen0CollectCount();
        return 0;
    }

    private static void DemoRoots()
    {
        Console.WriteLine("-- GC roots --");
        Console.WriteLine("  stack locals, statics, registers, GCHandle, finalization queue");
        object rooted = new string('x', 32);
        Console.WriteLine($"  rooted gen={GC.GetGeneration(rooted)}");
        Debug.Assert(rooted is not null);
        GC.KeepAlive(rooted);
    }

    private static void DemoCyclicReferenceReclaimed()
    {
        Console.WriteLine("-- cyclic refs are NOT leaks; GC reclaims after unroot --");
        (WeakReference wrA, WeakReference wrB) = MakeCycleWeakRefs();
        Debug.Assert(wrA.IsAlive && wrB.IsAlive);

        for (int i = 0; i < 5 && (wrA.IsAlive || wrB.IsAlive); i++)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
        }

        Console.WriteLine($"  cycle WeakRefs alive after GC: A={wrA.IsAlive}, B={wrB.IsAlive}");
        Debug.Assert(!wrA.IsAlive && !wrB.IsAlive, "unreachable cycle must be collected");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference, WeakReference) MakeCycleWeakRefs()
    {
        Node a = new("A");
        Node b = new("B");
        a.Next = b;
        b.Next = a; // cycle with no external root after return
        return (new WeakReference(a), new WeakReference(b));
    }

    private static void DemoWeakRefNotRoot()
    {
        Console.WriteLine("-- weak reference is not a strong root --");
        WeakReference wr = MakeWeakByteArray();
        Debug.Assert(wr.IsAlive);
        for (int i = 0; i < 5 && wr.IsAlive; i++)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
        }

        Console.WriteLine($"  WeakReference.IsAlive after GC={wr.IsAlive}");
        Debug.Assert(!wr.IsAlive);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference MakeWeakByteArray() => new(new byte[4096]);

    private static void DemoGen0CollectCount()
    {
        Console.WriteLine("-- Gen0 collection count increases after forced collect --");
        int before = GC.CollectionCount(0);
        for (int i = 0; i < 50; i++)
            _ = new byte[16_000];
        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        int after = GC.CollectionCount(0);
        Console.WriteLine($"  GC.CollectionCount(0): before={before}, after={after}");
        Debug.Assert(after > before, "forced Gen0 collect should bump CollectionCount(0)");
    }

    private sealed class Node(string name)
    {
        public string Name { get; } = name;
        public Node? Next;
    }
}
