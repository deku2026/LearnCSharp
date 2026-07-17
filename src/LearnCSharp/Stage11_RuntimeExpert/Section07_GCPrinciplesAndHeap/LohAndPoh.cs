// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第7部分-GC原理与堆结构.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section07_GCPrinciplesAndHeap
// Item     : LohAndPoh
// Topic id : stage11/section07/loh_and_poh
//
// Lesson: LOH for large objects (~85KB+); POH for pinned; compaction rules differ.

using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section07;

internal static class LohAndPoh
{
    private const int LohThreshold = 85_000;

    [LearnTopic("stage11/section07/loh_and_poh")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== LohAndPoh ===");
        DemoLohGeneration();
        DemoPoh();
        DemoCompactLohNote();
        return 0;
    }

    private static void DemoLohGeneration()
    {
        Console.WriteLine("-- Large Object Heap: gen of large array --");
        Console.WriteLine($"  LOH threshold typically {LohThreshold} bytes (array payload)");
        byte[] small = new byte[1024];
        byte[] large = new byte[LohThreshold];
        int genSmall = GC.GetGeneration(small);
        int genLarge = GC.GetGeneration(large);
        Console.WriteLine($"  small ({small.Length} B) gen={genSmall}");
        Console.WriteLine($"  large ({large.Length} B) gen={genLarge}");
        Debug.Assert(large.Length >= LohThreshold);
        Debug.Assert(small.Length < LohThreshold);
        // LOH allocations are treated as gen 2 for collection purposes.
        Debug.Assert(genLarge == 2, "LOH object should report generation 2");
        Debug.Assert(genSmall is 0 or 1 or 2);
        GC.KeepAlive(small);
        GC.KeepAlive(large);
    }

    private static void DemoPoh()
    {
        Console.WriteLine("-- Pinned Object Heap (.NET 5+) --");
        byte[] pinned = GC.AllocateArray<byte>(256, pinned: true);
        GCHandle h = GCHandle.Alloc(pinned, GCHandleType.Pinned);
        try
        {
            IntPtr addr = h.AddrOfPinnedObject();
            Console.WriteLine($"  GC.AllocateArray(pinned:true) addr=0x{addr:X}, len={pinned.Length}");
            Debug.Assert(addr != IntPtr.Zero);
            pinned[0] = 0xAB;
            Debug.Assert(pinned[0] == 0xAB);
        }
        finally
        {
            h.Free();
        }

        Console.WriteLine("  POH reduces fragmentation from random pinning on SOH/LOH.");
    }

    private static void DemoCompactLohNote()
    {
        Console.WriteLine("-- LOH compaction mode --");
        GCLargeObjectHeapCompactionMode mode = System.Runtime.GCSettings.LargeObjectHeapCompactionMode;
        Console.WriteLine($"  LargeObjectHeapCompactionMode={mode}");
        Debug.Assert(Enum.IsDefined(mode));
        Console.WriteLine("  Default: LOH often swept not compacted; can force compact next full GC.");
    }
}
