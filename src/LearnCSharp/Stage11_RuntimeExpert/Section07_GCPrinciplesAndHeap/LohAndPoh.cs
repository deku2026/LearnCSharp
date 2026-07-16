// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第7部分-GC原理与堆结构.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section07_GCPrinciplesAndHeap
// Item     : LohAndPoh
// Topic id : stage11/section07/loh_and_poh
//
// Lesson: LOH for large objects (~85KB+); POH for pinned; compaction rules differ.

using System.Diagnostics;
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
        DemoLoh();
        DemoPohConcept();
        DemoCompactLohNote();
        return 0;
    }

    private static void DemoLoh()
    {
        Console.WriteLine("-- Large Object Heap --");
        Console.WriteLine($"  Threshold typically {LohThreshold} bytes (array payload)");
        byte[] small = new byte[1024];
        byte[] large = new byte[LohThreshold];
        Console.WriteLine($"  small gen={GC.GetGeneration(small)}, large gen={GC.GetGeneration(large)}");
        // LOH objects are gen 2 from allocation perspective for collection frequency
        Debug.Assert(large.Length >= LohThreshold);
        Debug.Assert(small.Length < LohThreshold);
        GC.KeepAlive(small);
        GC.KeepAlive(large);
    }

    private static void DemoPohConcept()
    {
        Console.WriteLine("-- Pinned Object Heap (.NET 5+) --");
        Console.WriteLine("  GC.AllocateArray<T>(..., pinned: true) places on POH");
        byte[] pinned = GC.AllocateArray<byte>(256, pinned: true);
        GCHandle h = GCHandle.Alloc(pinned, GCHandleType.Pinned);
        try
        {
            IntPtr addr = h.AddrOfPinnedObject();
            Console.WriteLine($"  pinned array addr=0x{addr:X}, len={pinned.Length}");
            Debug.Assert(addr != IntPtr.Zero);
        }
        finally
        {
            h.Free();
        }

        Console.WriteLine("  POH reduces fragmentation from random pinning on SOH/LOH.");
    }

    private static void DemoCompactLohNote()
    {
        Console.WriteLine("-- LOH compaction --");
        Console.WriteLine("  Default: LOH often swept not compacted (fragmentation risk).");
        Console.WriteLine("  GCSettings.LargeObjectHeapCompactionMode can force compact next full GC.");
        var mode = System.Runtime.GCSettings.LargeObjectHeapCompactionMode;
        Console.WriteLine($"  Current LargeObjectHeapCompactionMode={mode}");
        Debug.Assert(Enum.IsDefined(mode));
    }
}
