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
        Console.WriteLine("-- old → young reference store --");
        // Promote a holder to older gen, then store a fresh object into it
        var holder = new Holder();
        GC.Collect(2);
        int holderGen = GC.GetGeneration(holder);
        object young = new byte[64];
        holder.Item = young; // write barrier if holder is older
        s_oldSlot = holder;  // static root
        Console.WriteLine($"  holder gen={holderGen}, young gen={GC.GetGeneration(young)}");
        Console.WriteLine("  Without barriers, Gen0 GC would miss young objects only reachable from Gen2.");
        Debug.Assert(holder.Item is not null);
        GC.KeepAlive(young);
        GC.KeepAlive(holder);
    }

    private static void DemoWhyNeeded()
    {
        Console.WriteLine("-- why not scan entire heap every Gen0? --");
        Console.WriteLine("  Too expensive — card table makes young collections scale.");
        Console.WriteLine("  Cost: small overhead on every ref store (highly optimized).");
        // keep static alive for demo
        Debug.Assert(s_oldSlot is not null || s_oldSlot is null);
        Console.WriteLine($"  static holder kept: {s_oldSlot is not null}");
    }

    private sealed class Holder
    {
        public object? Item;
    }
}
