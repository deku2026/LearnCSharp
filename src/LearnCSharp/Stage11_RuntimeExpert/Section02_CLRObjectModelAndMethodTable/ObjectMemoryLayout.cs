// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第2部分-CLR对象模型与方法表.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section02_CLRObjectModelAndMethodTable
// Item     : ObjectMemoryLayout
// Topic id : stage11/section02/object_memory_layout
//
// Lesson: object header + MethodTable pointer + fields; SyncBlock; size of refs.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section02;

internal static class ObjectMemoryLayout
{
    [LearnTopic("stage11/section02/object_memory_layout")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ObjectMemoryLayout ===");
        DemoAllocationDeltas();
        DemoHeaderAndSyncBlock();
        DemoReferenceAndArrayLayout();
        return 0;
    }

    private static void DemoAllocationDeltas()
    {
        Console.WriteLine("-- allocation deltas: empty class vs fields --");
        // Warm
        _ = new EmptyClass();
        _ = new TwoInts(1, 2);
        _ = new TwoIntsAndRef(1, 2, "x");

        long b0 = GC.GetAllocatedBytesForCurrentThread();
        var empty = new EmptyClass();
        long b1 = GC.GetAllocatedBytesForCurrentThread();
        var two = new TwoInts(10, 20);
        long b2 = GC.GetAllocatedBytesForCurrentThread();
        var withRef = new TwoIntsAndRef(10, 20, "name");
        long b3 = GC.GetAllocatedBytesForCurrentThread();

        long emptyBytes = b1 - b0;
        long twoBytes = b2 - b1;
        long withRefBytes = b3 - b2;
        Console.WriteLine($"  EmptyClass          Δ={emptyBytes} bytes (header + MethodTable* + padding)");
        Console.WriteLine($"  TwoInts (8B fields) Δ={twoBytes} bytes");
        Console.WriteLine($"  TwoIntsAndRef       Δ={withRefBytes} bytes");
        Debug.Assert(emptyBytes >= (uint)(IntPtr.Size * 2), "at least header + MT pointer");
        Debug.Assert(twoBytes >= emptyBytes, "fields increase size");
        Debug.Assert(withRefBytes >= twoBytes, "extra ref field increases size");
        Debug.Assert(two.X == 10 && withRef.Name == "name");
        GC.KeepAlive(empty);
        GC.KeepAlive(two);
        GC.KeepAlive(withRef);
        Console.WriteLine("  Layout: [ObjHeader] [MethodTable*] [fields...]");
    }

    private static void DemoHeaderAndSyncBlock()
    {
        Console.WriteLine("-- sync block / identity hash --");
        var a = new TwoInts(1, 2);
        int id1 = RuntimeHelpers.GetHashCode(a);
        lock (a)
        {
            Debug.Assert(Monitor.IsEntered(a));
            Console.WriteLine("  lock(a): thin/fat lock in header/sync block");
        }

        int id2 = RuntimeHelpers.GetHashCode(a);
        Console.WriteLine($"  RuntimeHelpers.GetHashCode stable: {id1} → {id2}");
        Debug.Assert(id1 == id2);
        Console.WriteLine($"  IntPtr.Size={IntPtr.Size} (object reference width)");
        Debug.Assert(IntPtr.Size is 4 or 8);
    }

    private static void DemoReferenceAndArrayLayout()
    {
        Console.WriteLine("-- array: MethodTable + length + elements --");
        int[] arr = [10, 20, 30];
        Console.WriteLine($"  Length={arr.Length}, elemSize={Unsafe.SizeOf<int>()}");
        long payload = (long)arr.Length * Unsafe.SizeOf<int>();
        Console.WriteLine($"  payload≈{payload} bytes (+ header/MT/length overhead)");

        long before = GC.GetAllocatedBytesForCurrentThread();
        int[] big = new int[1000];
        long after = GC.GetAllocatedBytesForCurrentThread();
        Console.WriteLine($"  new int[1000] Δalloc={after - before} (payload 4000 + overhead)");
        Debug.Assert(after - before >= 4000);
        Debug.Assert(arr.Length == 3);

        GCHandle h = GCHandle.Alloc(arr, GCHandleType.Pinned);
        try
        {
            IntPtr addr = h.AddrOfPinnedObject();
            Console.WriteLine($"  Pinned first element=0x{addr:X} (contiguous elements)");
            Debug.Assert(addr != IntPtr.Zero);
        }
        finally
        {
            h.Free();
        }

        GC.KeepAlive(big);
    }

    private sealed class EmptyClass;

    private sealed class TwoInts(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }

    private sealed class TwoIntsAndRef(int x, int y, string name)
    {
        public int X = x;
        public int Y = y;
        public string Name = name;
    }
}
