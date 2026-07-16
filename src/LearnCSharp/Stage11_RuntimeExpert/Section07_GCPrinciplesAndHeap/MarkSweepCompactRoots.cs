// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第7部分-GC原理与堆结构.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section07_GCPrinciplesAndHeap
// Item     : MarkSweepCompactRoots
// Topic id : stage11/section07/mark_sweep_compact_roots
//
// Lesson: mark from roots, plan, compact or sweep; unreachable objects reclaimed.

using System.Diagnostics;
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
        DemoUnreachable();
        DemoWeakRef();
        return 0;
    }

    private static void DemoRoots()
    {
        Console.WriteLine("-- GC roots --");
        Console.WriteLine("  stack locals, statics, CPU registers, GCHandle, finalization queue");
        object rooted = new string('x', 32);
        Console.WriteLine($"  rooted gen={GC.GetGeneration(rooted)}, hash={RuntimeHelpersHash(rooted)}");
        Debug.Assert(rooted is not null);
        GC.KeepAlive(rooted);
    }

    private static void DemoUnreachable()
    {
        Console.WriteLine("-- mark-sweep-compact outline --");
        Console.WriteLine("  SuspendEE → mark from roots → plan → relocate/compact or sweep → RestartEE");
        long before = GC.GetTotalMemory(forceFullCollection: false);
        MakeGarbage();
        long mid = GC.GetTotalMemory(forceFullCollection: false);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long after = GC.GetTotalMemory(forceFullCollection: true);
        Console.WriteLine($"  mem before={before}, after garbage={mid}, after full GC={after}");
        Debug.Assert(after >= 0);
    }

    private static void DemoWeakRef()
    {
        Console.WriteLine("-- weak reference is not a strong root --");
        var wr = new WeakReference(new byte[1024]);
        Debug.Assert(wr.IsAlive);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Console.WriteLine($"  WeakReference.IsAlive after GC={wr.IsAlive} (often false)");
        // Not asserting false — timing/finalizer can vary; educational observation
    }

    private static void MakeGarbage()
    {
        for (int i = 0; i < 200; i++)
            _ = new byte[10_000];
    }

    private static int RuntimeHelpersHash(object o) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(o);
}
