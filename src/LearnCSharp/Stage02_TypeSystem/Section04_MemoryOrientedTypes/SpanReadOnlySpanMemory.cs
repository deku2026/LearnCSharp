// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第4部分-面向内存的类型.md
// Stage    : Stage02_TypeSystem
// Section  : Section04_MemoryOrientedTypes
// Item     : SpanReadOnlySpanMemory
// Topic id : stage02/section04/span_readonly_span_memory
//
// 步骤 2：Span/ReadOnlySpan 零拷贝视图、stackalloc、Memory、ArrayPool。

using System.Buffers;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section04;

internal static class SpanReadOnlySpanMemory
{
    [LearnTopic("stage02/section04/span_readonly_span_memory")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SpanReadOnlySpanMemory ===");
        DemoSpanView();
        DemoReadOnlySpan();
        DemoStackalloc();
        DemoMemory();
        DemoArrayPool();
        DemoRefStructLimits();
        return 0;
    }

    private static void DemoSpanView()
    {
        Console.WriteLine("-- Span 零分配视图 --");
        int[] numbers = [1, 2, 3, 4, 5];
        Span<int> span = numbers.AsSpan();
        Span<int> slice = span[1..4]; // {2,3,4}
        slice[0] = 99;
        Debug.Assert(numbers[1] == 99);
        Debug.Assert(slice.Length == 3);
        Console.WriteLine($"  改 slice[0] → numbers[1]={numbers[1]}");
    }

    private static void DemoReadOnlySpan()
    {
        Console.WriteLine("-- ReadOnlySpan（字符串） --");
        ReadOnlySpan<char> chars = "hello".AsSpan();
        Debug.Assert(chars.Length == 5);
        Debug.Assert(chars[0] == 'h');
        ReadOnlySpan<char> sub = chars.Slice(1, 3); // "ell"
        Debug.Assert(sub.SequenceEqual("ell"));
        Console.WriteLine($"  \"hello\".AsSpan().Slice(1,3)={sub.ToString()}");
    }

    private static void DemoStackalloc()
    {
        Console.WriteLine("-- stackalloc 栈缓冲 --");
        Span<int> buffer = stackalloc int[8];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = i * i;
        Debug.Assert(buffer[3] == 9);
        Console.WriteLine($"  stackalloc int[8], buffer[3]={buffer[3]}（零堆分配）");
    }

    private static void DemoMemory()
    {
        Console.WriteLine("-- Memory：可存堆/可进 async 的视图 --");
        int[] data = [1, 2, 3, 4];
        Memory<int> mem = data.AsMemory(1, 2); // {2,3}
        Span<int> span = mem.Span;
        span[0] = 20;
        Debug.Assert(data[1] == 20);
        Console.WriteLine("  Memory 不是 ref struct，可作字段/async 参数；操作时取 .Span");
    }

    private static void DemoArrayPool()
    {
        Console.WriteLine("-- ArrayPool 复用大缓冲 --");
        var pool = ArrayPool<byte>.Shared;
        byte[] rented = pool.Rent(1024);
        try
        {
            Span<byte> slice = rented.AsSpan(0, 16);
            slice.Clear();
            slice[0] = 0xAB;
            Debug.Assert(rented[0] == 0xAB);
            Console.WriteLine($"  Rent 至少 1024，实际 Length={rented.Length}");
        }
        finally
        {
            pool.Return(rented);
        }
    }

    private static void DemoRefStructLimits()
    {
        Console.WriteLine("-- Span 是 ref struct：不能装箱/不能作 class 字段 --");
        Span<int> s = stackalloc int[2] { 1, 2 };
        // object o = s; // 编译错误
        // 不能捕获进 lambda
        int sum = Sum(s);
        Debug.Assert(sum == 3);
        Console.WriteLine("  只能活在栈上；跨 await 用 Memory");
    }

    private static int Sum(ReadOnlySpan<int> data)
    {
        int acc = 0;
        foreach (var n in data) acc += n;
        return acc;
    }
}
