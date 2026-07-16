// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第4部分-unsafe指针Marshal函数指针.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section04_UnsafePointersMarshalAndFunctionPointers
// Item     : SpanStackallocPointerInterop
// Topic id : stage13/section04/span_stackalloc_pointer_interop
//
// Lesson: Span/stackalloc ↔ pointers; prefer Span, fixed only at the native edge.

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section04;

internal static class SpanStackallocPointerInterop
{
    [LearnTopic("stage13/section04/span_stackalloc_pointer_interop")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SpanStackallocPointerInterop ===");
        DemoStackallocSpanVsPointer();
        DemoSpanToPointer();
        DemoPointerToSpan();
        DemoDecisionGuide();
        return 0;
    }

    private static void DemoStackallocSpanVsPointer()
    {
        Console.WriteLine("-- stackalloc: Span (safe) vs pointer (unsafe) --");
        Span<byte> safe = stackalloc byte[8];
        safe.Fill(1);
        int sum = 0;
        foreach (byte b in safe)
            sum += b;
        Debug.Assert(sum == 8);
        Console.WriteLine($"  safe stackalloc Span sum={sum}");

        unsafe
        {
            byte* raw = stackalloc byte[8];
            for (int i = 0; i < 8; i++)
                raw[i] = 2;
            int s = 0;
            for (int i = 0; i < 8; i++)
                s += raw[i];
            Debug.Assert(s == 16);
            Console.WriteLine($"  unsafe stackalloc byte* sum={s}");
        }
    }

    private static unsafe void DemoSpanToPointer()
    {
        Console.WriteLine("-- Span → fixed pointer (native edge) --");
        Span<byte> data = stackalloc byte[] { 1, 2, 3, 4 };
        fixed (byte* p = data)
        {
            // Simulate native API consuming a pointer
            int sum = NativeStyleSum(p, data.Length);
            Debug.Assert(sum == 10);
            Console.WriteLine($"  fixed (byte* p = span) NativeStyleSum => {sum}");
        }
    }

    private static unsafe void DemoPointerToSpan()
    {
        Console.WriteLine("-- pointer+length → Span (safe view over native) --");
        void* mem = NativeMemory.Alloc(16);
        try
        {
            Span<byte> view = new(mem, 16);
            view.Fill(0x11);
            Debug.Assert(view[0] == 0x11 && view[15] == 0x11);
            Console.WriteLine($"  Span over NativeMemory: len={view.Length}, [0]={view[0]:X2}");
            Console.WriteLine("  Bounds-checked access; still must Free the native buffer yourself.");
        }
        finally
        {
            NativeMemory.Free(mem);
        }
    }

    private static void DemoDecisionGuide()
    {
        Console.WriteLine("-- decision guide (Stage13 close) --");
        Console.WriteLine("  Prefer: Span/stackalloc, LibraryImport, SafeHandle");
        Console.WriteLine("  Use unsafe when: raw native pointer required, complex Marshal, hot callbacks");
        Console.WriteLine("  Metaprogramming: generators > reflection for known shapes");
        Console.WriteLine("  Interop: blittable + pin only as long as needed");
    }

    private static unsafe int NativeStyleSum(byte* p, int length)
    {
        int s = 0;
        for (int i = 0; i < length; i++)
            s += p[i];
        return s;
    }
}
