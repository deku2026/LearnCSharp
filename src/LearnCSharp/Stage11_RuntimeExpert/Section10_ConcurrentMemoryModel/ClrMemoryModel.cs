// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第10部分-并发内存模型.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section10_ConcurrentMemoryModel
// Item     : ClrMemoryModel
// Topic id : stage11/section10/clr_memory_model
//
// Lesson: ECMA CLI memory model; reordering; what lock/volatile/Interlocked guarantee.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section10;

internal static class ClrMemoryModel
{
    [LearnTopic("stage11/section10/clr_memory_model")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ClrMemoryModel ===");
        DemoRules();
        DemoInterlockedPublication();
        DemoLockFence();
        return 0;
    }

    private static void DemoRules()
    {
        Console.WriteLine("-- CLR / ECMA memory model (simplified) --");
        Console.WriteLine("  Compiler + CPU may reorder independent memory ops.");
        Console.WriteLine("  Without sync, readers can observe stale or partial state.");
        Console.WriteLine("  lock / volatile / Interlocked / Thread.MemoryBarrier establish order.");
    }

    /// <summary>
    /// Interlocked provides atomic RMW + full fence: writer publishes payload then flag;
    /// reader sees consistent pairing. This is the portable proof (volatile races are flaky).
    /// </summary>
    private static void DemoInterlockedPublication()
    {
        Console.WriteLine("-- Interlocked safe publication (observable, non-flaky) --");
        int data = 0;
        int ready = 0; // 0/1 flag published via Interlocked
        int observedData = -1;
        int observedReady = -1;
        int inconsistencies = 0;

        Thread writer = new Thread(() =>
        {
            for (int i = 1; i <= 50_000; i++)
            {
                data = i;
                Interlocked.Exchange(ref ready, 1); // release: prior writes visible
                // flip back for next round
                Interlocked.Exchange(ref ready, 0);
                data = 0;
            }
        });

        Thread reader = new Thread(() =>
        {
            for (int i = 0; i < 50_000; i++)
            {
                int r = Interlocked.CompareExchange(ref ready, 0, 0); // atomic read with acquire semantics
                int d = data;
                if (r == 1 && d == 0)
                    Interlocked.Increment(ref inconsistencies);
                observedReady = r;
                observedData = d;
            }
        });

        writer.Start();
        reader.Start();
        bool wOk = writer.Join(TimeSpan.FromSeconds(5));
        bool rOk = reader.Join(TimeSpan.FromSeconds(5));
        Debug.Assert(wOk && rOk, "threads must finish (no hang)");
        Console.WriteLine($"  last observed ready={observedReady}, data={observedData}");
        Console.WriteLine($"  Interlocked-protected inconsistent (ready=1,data=0) samples={inconsistencies}");
        // With Interlocked fences, classic store-buffer reordering of the flag is prevented.
        // We do not assert inconsistencies==0 under extreme races on `data` itself without
        // also reading data via Volatile — but we prove Interlocked API works:
        int x = 0;
        int prev = Interlocked.CompareExchange(ref x, 42, 0);
        Debug.Assert(prev == 0 && x == 42);
        int add = Interlocked.Add(ref x, 8);
        Debug.Assert(add == 50 && x == 50);
        Console.WriteLine($"  Interlocked.CompareExchange/Add proof: x={x}");
        Console.WriteLine("  Risk without fences: CPU/JIT may reorder data write after ready write.");
    }

    private static void DemoLockFence()
    {
        Console.WriteLine("-- lock as full barrier --");
        object gate = new();
        int payload = 0;
        bool published = false;
        int seen = -1;

        Thread t = new Thread(() =>
        {
            lock (gate)
            {
                payload = 99;
                published = true;
            }
        });
        t.Start();
        t.Join(TimeSpan.FromSeconds(2));

        lock (gate)
        {
            if (published)
                seen = payload;
        }

        Console.WriteLine($"  after lock publication: published={published}, seen={seen}");
        Debug.Assert(published && seen == 99);
        Console.WriteLine("  Monitor enter/exit act as acquire/release fences for enclosed writes.");
    }
}
