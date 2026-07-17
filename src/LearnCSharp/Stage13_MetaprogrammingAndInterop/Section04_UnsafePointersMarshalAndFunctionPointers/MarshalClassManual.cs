// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第4部分-unsafe指针Marshal函数指针.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section04_UnsafePointersMarshalAndFunctionPointers
// Item     : MarshalClassManual
// Topic id : stage13/section04/marshal_class_manual
//
// Lesson: Marshal/NativeMemory = manual malloc/memcpy/struct-at-address (must Free).

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section04;

internal static class MarshalClassManual
{
    [LearnTopic("stage13/section04/marshal_class_manual")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MarshalClassManual ===");
        DemoAllocCopyFree();
        DemoStructAtAddress();
        DemoNativeMemory();
        return 0;
    }

    private static void DemoAllocCopyFree()
    {
        Console.WriteLine("-- AllocHGlobal / Copy / FreeHGlobal ≈ malloc/memcpy/free --");
        byte[] managed = [10, 20, 30, 40];
        nint buf = Marshal.AllocHGlobal(managed.Length);
        try
        {
            Marshal.Copy(managed, 0, buf, managed.Length);
            byte[] back = new byte[managed.Length];
            Marshal.Copy(buf, back, 0, back.Length);
            Debug.Assert(back.AsSpan().SequenceEqual(managed));
            Console.WriteLine($"  round-trip: [{string.Join(',', back)}]");
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }

        Console.WriteLine("  Forget Free = real leak (GC does not track native heaps).");
    }

    private static void DemoStructAtAddress()
    {
        Console.WriteLine("-- PtrToStructure / StructureToPtr / SizeOf / OffsetOf --");
        int size = Marshal.SizeOf<Header>();
        nint offLen = Marshal.OffsetOf<Header>(nameof(Header.Length));
        Debug.Assert(size >= 5);
        Debug.Assert(offLen is 1 or 4); // Pack-dependent; Sequential default often pads

        nint p = Marshal.AllocHGlobal(size);
        try
        {
            Header h = new Header { Tag = 7, Length = 99 };
            Marshal.StructureToPtr(h, p, fDeleteOld: false);
            Header back = Marshal.PtrToStructure<Header>(p);
            Debug.Assert(back.Tag == 7 && back.Length == 99);
            Console.WriteLine($"  SizeOf={size}, OffsetOf(Length)={offLen}, Tag={back.Tag}, Length={back.Length}");

            Marshal.WriteInt32(p, (int)offLen, 123);
            int v = Marshal.ReadInt32(p, (int)offLen);
            Debug.Assert(v == 123);
            Console.WriteLine($"  WriteInt32/ReadInt32 at Length offset => {v}");
        }
        finally
        {
            Marshal.FreeHGlobal(p);
        }
    }

    private static unsafe void DemoNativeMemory()
    {
        Console.WriteLine("-- NativeMemory (.NET 6+) = portable malloc/free --");
        void* mem = NativeMemory.AllocZeroed(64);
        try
        {
            Span<byte> view = new(mem, 64);
            Debug.Assert(view[0] == 0 && view[63] == 0);
            view[0] = 0xAB;
            view[63] = 0xCD;
            Debug.Assert(view[0] == 0xAB && view[63] == 0xCD);
            Console.WriteLine($"  AllocZeroed(64) then write ends: {view[0]:X2}..{view[63]:X2}");
        }
        finally
        {
            NativeMemory.Free(mem);
        }

        nint s = Marshal.StringToCoTaskMemUTF8("hi");
        try
        {
            string back = Marshal.PtrToStringUTF8(s)!;
            Debug.Assert(back == "hi");
            Console.WriteLine($"  StringToCoTaskMemUTF8/PtrToStringUTF8 => {back}");
        }
        finally
        {
            Marshal.FreeCoTaskMem(s);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Header
    {
        public byte Tag;
        public int Length;
    }
}
