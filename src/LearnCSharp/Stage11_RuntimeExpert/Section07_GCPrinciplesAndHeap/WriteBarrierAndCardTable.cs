// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第7部分-GC原理与堆结构.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section07_GCPrinciplesAndHeap
// Item     : WriteBarrierAndCardTable
// Topic id : stage11/section07/write_barrier_and_card_table
//
// Lesson: store of ref into old gen marks card table so ephemeral GC can scan dirty cards.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section07;

internal static class WriteBarrierAndCardTable
{
    private static object? s_oldSlot;

    [LearnTopic("stage11/section07/write_barrier_and_card_table")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WriteBarrierAndCardTable ===");
        DemoConcept();
        DemoCrossGenStore();
        DemoWhyNeeded();
        return 0;
    }

    private static void DemoConcept()
    {
        Console.WriteLine("-- write barrier --");
        Console.WriteLine("  JIT inserts barrier code on reference field stores.");
        Console.WriteLine("  Marks a 'card' covering the destination address as dirty.");
        Console.WriteLine("  Ephemeral GC scans dirty cards instead of all of Gen2.");
    }

    private static void DemoCrossGenStore()
    {
        Console.WriteLine("-- old → young reference store (dirty card keeps young alive) --");
        // Promote a holder into Gen2, then store a fresh Gen0 object into it.
        Holder holder = new Holder();
        GC.Collect(2);
        GC.WaitForPendingFinalizers();
        GC.Collect(2);
        int holderGen = GC.GetGeneration(holder);
        Debug.Assert(holderGen == 2, "holder should be promoted to Gen2");
        object young = new byte[64];
        int youngGenBefore = GC.GetGeneration(young);
        holder.Item = young; // write barrier marks the card covering holder as dirty
        s_oldSlot = holder;  // static root keeps holder (hence young) reachable
        Console.WriteLine($"  holder gen={holderGen}, young gen(before Gen0 collect)={youngGenBefore}");
        // Drop the local reference to young; it's now only reachable via the Gen2 holder.
        young = new byte[16]; // rebind local so the original young has no young root
        // Collect Gen0 only. The dirty card must be scanned so the original young survives.
        GC.Collect(0);
        GC.WaitForPendingFinalizers();
        GC.Collect(0);
        object? survived = ((Holder?)s_oldSlot!).Item;
        Debug.Assert(survived is not null, "young object must survive Gen0 via dirty card");
        int youngGenAfter = GC.GetGeneration(survived!);
        Console.WriteLine($"  after Gen0-only collect: survived={survived is not null}, promoted to gen={youngGenAfter}");
        // Without the write barrier, a Gen0 collection would reclaim young (no young root, not scanned in Gen2) → AV/use-after-free.
        GC.KeepAlive(holder);
    }

    private static void DemoWhyNeeded()
    {
        Console.WriteLine("-- why not scan entire heap every Gen0? --");
        Console.WriteLine("  Too expensive — card table makes young collections scale.");
        Console.WriteLine("  Cost: small overhead on every ref store (highly optimized).");
        // keep static alive for demo
        Debug.Assert(s_oldSlot is not null);
        Console.WriteLine($"  static holder kept: {s_oldSlot is not null}");
    }

    private sealed class Holder
    {
        public object? Item;
    }
}
