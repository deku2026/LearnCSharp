// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第10部分-并发内存模型.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section10_ConcurrentMemoryModel
// Item     : VolatileAndMemoryBarriers
// Topic id : stage11/section10/volatile_and_memory_barriers
//
// Lesson: volatile read/write; Volatile.Read/Write; Thread.MemoryBarrier.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section10;

internal static class VolatileAndMemoryBarriers
{
    private static volatile bool s_ready;
    private static int s_data;

    [LearnTopic("stage11/section10/volatile_and_memory_barriers")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== VolatileAndMemoryBarriers ===");
        DemoVolatileFlag();
        DemoVolatileClass();
        DemoMemoryBarrier();
        return 0;
    }

    private static void DemoVolatileFlag()
    {
        Console.WriteLine("-- volatile flag publication (short, non-flaky) --");
        s_ready = false;
        s_data = 0;
        var t = new Thread(() =>
        {
            s_data = 123;
            s_ready = true; // volatile write releases prior stores
        });
        t.Start();
        // spin with timeout
        var sw = Stopwatch.StartNew();
        while (!s_ready && sw.ElapsedMilliseconds < 2000)
            Thread.Yield();
        t.Join(2000);
        int observed = s_data;
        Console.WriteLine($"  ready={s_ready}, data={observed}");
        Debug.Assert(s_ready);
        Debug.Assert(observed == 123);
        Console.WriteLine("  Without volatile/locks, reader might spin forever or see data=0 (CPU/JIT reorder).");
    }

    private static void DemoVolatileClass()
    {
        Console.WriteLine("-- System.Threading.Volatile --");
        int x = 0;
        Volatile.Write(ref x, 99);
        int y = Volatile.Read(ref x);
        Debug.Assert(y == 99);
        Console.WriteLine($"  Volatile.Write/Read → {y}");
        Console.WriteLine("  Use for explicit acquire/release without declaring field volatile.");
    }

    private static void DemoMemoryBarrier()
    {
        Console.WriteLine("-- Thread.MemoryBarrier --");
        int a = 1;
        Thread.MemoryBarrier(); // full fence
        int b = a;
        Debug.Assert(b == 1);
        Console.WriteLine("  Full fence prevents reordering across the barrier both ways.");
        Console.WriteLine("  Prefer higher-level primitives; raw barriers are easy to misuse.");
    }
}
