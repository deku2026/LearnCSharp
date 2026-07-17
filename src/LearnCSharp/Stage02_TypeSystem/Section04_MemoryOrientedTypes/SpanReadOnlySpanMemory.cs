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
        DemoMemoryAcrossAwait();
        DemoArrayPool();
        DemoArrayPoolClearOnReturnPitfall();
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

    private static void DemoMemoryAcrossAwait()
    {
        Console.WriteLine("-- Memory 可跨 await；Span 不行（编译期） --");
        // 编译期叙事：
        //   async Task Bad(Span<byte> s) { await Task.Yield(); s[0] = 1; }  // CS4012 等
        //   Span 是 ref struct → 不能进 async 状态机字段
        //   Memory<T> 是普通 struct → 可存字段，await 后再 .Span 使用
        DemoMemoryAcrossAwaitCore().GetAwaiter().GetResult();
    }

    private static async Task DemoMemoryAcrossAwaitCore()
    {
        byte[] backing = new byte[4];
        Memory<byte> mem = backing.AsMemory();
        mem.Span[0] = 0x11;
        await Task.Yield();
        // await 之后只能继续用 Memory；临时取 Span 操作底层缓冲
        Span<byte> span = mem.Span;
        span[1] = 0x22;
        Debug.Assert(backing[0] == 0x11 && backing[1] == 0x22);
        Console.WriteLine($"  after await: backing=[{backing[0]:X2},{backing[1]:X2},...] via Memory.Span");
    }

    private static void DemoArrayPool()
    {
        Console.WriteLine("-- ArrayPool 复用大缓冲 --");
        ArrayPool<byte> pool = ArrayPool<byte>.Shared;
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

    private static void DemoArrayPoolClearOnReturnPitfall()
    {
        Console.WriteLine("-- ArrayPool clear-on-return 陷阱 --");
        // Return(array, clearArray: false) 默认不清空 → 下一租户可能读到旧敏感数据
        // clearArray: true 会在归还时清零（更安全，略慢）
        ArrayPool<byte> pool = ArrayPool<byte>.Shared;
        const int size = 64;
        byte[] first = pool.Rent(size);
        try
        {
            Array.Clear(first);
            first[0] = 0xEE;
            first[1] = 0xFF;
            Debug.Assert(first[0] == 0xEE);
        }
        finally
        {
            pool.Return(first, clearArray: false); // 故意不清
        }

        byte[] second = pool.Rent(size);
        try
        {
            // 同一池缓冲可能被再次租出；不清空时旧内容仍可能在
            // （Shared 池不保证同一实例，但 clearArray 语义仍是正确教学点）
            bool sawStale = second[0] == 0xEE && second[1] == 0xFF;
            Console.WriteLine($"  Return(clearArray:false) 后再 Rent：可能看到脏数据 sawStale={sawStale}");
            // 安全路径：归还时清空
            second[0] = 0xAA;
            pool.Return(second, clearArray: true);
            second = pool.Rent(size);
            // 若拿到刚清过的缓冲，[0] 应为 0；池实现可能换缓冲，故只断言可写可读
            second[0] = 1;
            Debug.Assert(second[0] == 1);
            Console.WriteLine("  敏感数据：Return(..., clearArray: true) 或租用后自行 Clear");
        }
        finally
        {
            pool.Return(second, clearArray: true);
        }
    }

    private static void DemoRefStructLimits()
    {
        Console.WriteLine("-- Span 是 ref struct：不能装箱/不能作 class 字段 --");
        Span<int> s = stackalloc int[2] { 1, 2 };
        // object o = s; // 编译错误
        // 不能捕获进 lambda
        // 不能：async Task F(Span<int> x) { await ...; } — 跨 await 用 Memory
        int sum = Sum(s);
        Debug.Assert(sum == 3);
        Console.WriteLine("  只能活在栈上；跨 await 用 Memory（见上）");
    }

    private static int Sum(ReadOnlySpan<int> data)
    {
        int acc = 0;
        foreach (int n in data) acc += n;
        return acc;
    }
}
