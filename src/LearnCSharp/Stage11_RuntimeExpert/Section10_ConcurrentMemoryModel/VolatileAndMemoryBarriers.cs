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

    private static int s_plainFlag;
    private static int s_plainData;

    [LearnTopic("stage11/section10/volatile_and_memory_barriers")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== VolatileAndMemoryBarriers ===");
        DemoVolatileFlag();
        DemoVolatileClassApi();
        DemoInterlockedVsPlainNote();
        DemoMemoryBarrier();
        return 0;
    }

    private static void DemoVolatileFlag()
    {
        Console.WriteLine("-- volatile flag publication --");
        s_ready = false;
        s_data = 0;
        var t = new Thread(() =>
        {
            s_data = 123;
            s_ready = true; // volatile write: release semantics for prior stores
        });
        t.Start();
        var sw = Stopwatch.StartNew();
        while (!s_ready && sw.ElapsedMilliseconds < 2000)
            Thread.Yield();
        bool joined = t.Join(2000);
        int observed = s_data;
        Console.WriteLine($"  ready={s_ready}, data={observed}, joined={joined}");
        Debug.Assert(joined && s_ready);
        Debug.Assert(observed == 123);
        Console.WriteLine("  Without volatile/locks, reader may spin forever or see data=0 (reorder risk).");
    }

    private static void DemoVolatileClassApi()
    {
        Console.WriteLine("-- System.Threading.Volatile --");
        int x = 0;
        Volatile.Write(ref x, 99);
        int y = Volatile.Read(ref x);
        Debug.Assert(y == 99);
        Console.WriteLine($"  Volatile.Write/Read → {y}");

        // Multi-thread stop flag without declaring field volatile
        int stop = 0;
        int ticks = 0;
        var worker = new Thread(() =>
        {
            while (Volatile.Read(ref stop) == 0)
                ticks++;
        });
        worker.Start();
        Thread.Sleep(5);
        Volatile.Write(ref stop, 1);
        bool ok = worker.Join(2000);
        Debug.Assert(ok);
        Console.WriteLine($"  Volatile stop-flag: worker stopped after ticks≈{ticks}");
    }

    private static void DemoInterlockedVsPlainNote()
    {
        Console.WriteLine("-- Interlocked proof (portable) vs plain field risk --");
        // Portable assert: Interlocked.Increment is atomic
        int counter = 0;
        var threads = new Thread[4];
        for (int t = 0; t < threads.Length; t++)
        {
            threads[t] = new Thread(() =>
            {
                for (int i = 0; i < 25_000; i++)
                    Interlocked.Increment(ref counter);
            });
            threads[t].Start();
        }

        foreach (Thread th in threads)
            Debug.Assert(th.Join(5000));

        Console.WriteLine($"  4×25k Interlocked.Increment → {counter}");
        Debug.Assert(counter == 100_000);

        // Plain ++ races (may or may not lose counts — document, soft check)
        int plain = 0;
        var plainThreads = new Thread[4];
        for (int t = 0; t < plainThreads.Length; t++)
        {
            plainThreads[t] = new Thread(() =>
            {
                for (int i = 0; i < 25_000; i++)
                    plain++; // data race — educational only
            });
            plainThreads[t].Start();
        }

        foreach (Thread th in plainThreads)
            Debug.Assert(th.Join(5000));

        Console.WriteLine($"  4×25k plain++ → {plain} (often < 100000 under race; not asserted)");
        Console.WriteLine("  Reordering risk: publish flag without volatile/Interlocked may tear.");

        // One-shot Interlocked publish
        s_plainData = 0;
        s_plainFlag = 0;
        var pub = new Thread(() =>
        {
            s_plainData = 777;
            Interlocked.Exchange(ref s_plainFlag, 1);
        });
        pub.Start();
        var sw = Stopwatch.StartNew();
        while (Interlocked.CompareExchange(ref s_plainFlag, 0, 0) == 0 && sw.ElapsedMilliseconds < 2000)
            Thread.Yield();
        pub.Join(2000);
        int d = s_plainData;
        Console.WriteLine($"  Interlocked flag publish: flag={s_plainFlag}, data={d}");
        Debug.Assert(s_plainFlag == 1 && d == 777);
    }

    private static void DemoMemoryBarrier()
    {
        Console.WriteLine("-- Thread.MemoryBarrier --");
        int a = 1;
        Thread.MemoryBarrier();
        int b = a;
        Debug.Assert(b == 1);
        Console.WriteLine("  Full fence prevents reordering across the barrier both ways.");
        Console.WriteLine("  Prefer Interlocked/lock/volatile over raw barriers.");
    }
}
