// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第4部分-unsafe指针Marshal函数指针.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section04_UnsafePointersMarshalAndFunctionPointers
// Item     : FixedPinning
// Topic id : stage13/section04/fixed_pinning
//
// Lesson: fixed pins GC objects for a scope so pointers stay valid.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section04;

internal static class FixedPinning
{
    [LearnTopic("stage13/section04/fixed_pinning")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== FixedPinning ===");
        DemoFixedArray();
        DemoFixedStringAndSpan();
        DemoFixedSizeBuffer();
        return 0;
    }

    private static unsafe void DemoFixedArray()
    {
        Console.WriteLine("-- fixed on array --");
        int[] arr = [1, 2, 3, 4, 5];
        fixed (int* p = arr)
        {
            int* q = p;
            for (int i = 0; i < arr.Length; i++)
                *q++ *= 10;
        }

        Debug.Assert(arr is [10, 20, 30, 40, 50]);
        Console.WriteLine($"  after fixed walk: [{string.Join(',', arr)}]");
        Console.WriteLine("  Do not store p outside the fixed block — GC may move after unpin.");
    }

    private static unsafe void DemoFixedStringAndSpan()
    {
        Console.WriteLine("-- fixed string / Span --");
        string s = "hello";
        int sum = 0;
        fixed (char* ps = s)
        {
            for (int i = 0; i < s.Length; i++)
                sum += ps[i];
        }

        Debug.Assert(sum == 'h' + 'e' + 'l' + 'l' + 'o');
        Console.WriteLine($"  sum of chars via fixed char* = {sum}");

        Span<byte> span = stackalloc byte[] { 1, 2, 3, 4 };
        int total = 0;
        fixed (byte* pb = span)
        {
            for (int i = 0; i < span.Length; i++)
                total += pb[i];
        }

        Debug.Assert(total == 10);
        Console.WriteLine($"  fixed Span sum={total}");
    }

    private static unsafe void DemoFixedSizeBuffer()
    {
        Console.WriteLine("-- fixed-size buffer in struct (C char buf[N]) --");
        Buffer16 b = default;
        for (int i = 0; i < 16; i++)
            b.Data[i] = (byte)i;

        Debug.Assert(b.Data[0] == 0 && b.Data[15] == 15);
        Console.WriteLine($"  Buffer16.Data[0..3]={b.Data[0]},{b.Data[1]},{b.Data[2]},{b.Data[3]} size={sizeof(Buffer16)}");
        Console.WriteLine("  Inline array matches C struct layout for interop.");
    }

    private unsafe struct Buffer16
    {
        public fixed byte Data[16];
    }
}
