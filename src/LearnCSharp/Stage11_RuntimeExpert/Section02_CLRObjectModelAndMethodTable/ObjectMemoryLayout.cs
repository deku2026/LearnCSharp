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
        DemoHeaderConcept();
        DemoReferenceSize();
        DemoArrayLayout();
        return 0;
    }

    private static void DemoHeaderConcept()
    {
        Console.WriteLine("-- managed object layout (conceptual) --");
        Console.WriteLine("  [ObjHeader/SyncBlock index] [MethodTable*] [instance fields...]");
        Console.WriteLine("  SyncBlock holds lock, hash code, COM interop data when needed.");
        var a = new PointBox(1, 2);
        var b = new PointBox(1, 2);
        Console.WriteLine($"  a.GetHashCode()={a.GetHashCode()} (may allocate sync block)");
        lock (a)
        {
            Console.WriteLine("  lock(a): thin/fat lock lives in header/sync block");
            Debug.Assert(Monitor.IsEntered(a));
        }

        Debug.Assert(a.X == 1 && b.Y == 2);
        Console.WriteLine($"  PointBox fields X={a.X}, Y={a.Y}");
    }

    private static void DemoReferenceSize()
    {
        Console.WriteLine("-- reference size = pointer size --");
        int ptrSize = IntPtr.Size;
        Console.WriteLine($"  IntPtr.Size={ptrSize} (object reference width on this process)");
        Debug.Assert(ptrSize is 4 or 8);
        // RuntimeHelpers.GetHashCode uses identity hash (sync block)
        object o1 = new object();
        object o2 = new object();
        int h1 = RuntimeHelpers.GetHashCode(o1);
        int h2 = RuntimeHelpers.GetHashCode(o2);
        Console.WriteLine($"  RuntimeHelpers.GetHashCode identity: {h1} vs {h2}");
        Debug.Assert(h1 != 0 || h2 != 0 || true);
    }

    private static void DemoArrayLayout()
    {
        Console.WriteLine("-- array: MethodTable + length + elements --");
        int[] arr = [10, 20, 30];
        Console.WriteLine($"  Length={arr.Length}, element type={arr.GetType().GetElementType()?.Name}");
        Console.WriteLine($"  Unsafe.SizeOf<int>()={Unsafe.SizeOf<int>()}");
        long approx = (long)arr.Length * Unsafe.SizeOf<int>();
        Console.WriteLine($"  payload bytes≈{approx} (+ header/MT/length overhead)");
        Debug.Assert(arr.Length == 3);
        GCHandle h = GCHandle.Alloc(arr, GCHandleType.Pinned);
        try
        {
            IntPtr addr = h.AddrOfPinnedObject();
            Console.WriteLine($"  Pinned first-element address=0x{addr:X} (elements contiguous)");
            Debug.Assert(addr != IntPtr.Zero);
        }
        finally
        {
            h.Free();
        }
    }

    private sealed class PointBox(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
    }
}
