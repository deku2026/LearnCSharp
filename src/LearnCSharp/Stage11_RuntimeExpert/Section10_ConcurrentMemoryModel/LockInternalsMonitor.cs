// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第10部分-并发内存模型.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section10_ConcurrentMemoryModel
// Item     : LockInternalsMonitor
// Topic id : stage11/section10/lock_internals_monitor
//
// Lesson: lock → Monitor.Enter/Exit; thin lock in object header; Wait/Pulse.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section10;

internal static class LockInternalsMonitor
{
    [LearnTopic("stage11/section10/lock_internals_monitor")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== LockInternalsMonitor ===");
        DemoLockStatement();
        DemoTryEnter();
        DemoWaitPulse();
        return 0;
    }

    private static void DemoLockStatement()
    {
        Console.WriteLine("-- lock expands to Monitor --");
        object gate = new();
        int value = 0;
        lock (gate)
        {
            value = 1;
            Debug.Assert(Monitor.IsEntered(gate));
            Console.WriteLine("  inside lock: Monitor.IsEntered=true");
        }

        Debug.Assert(!Monitor.IsEntered(gate));
        Debug.Assert(value == 1);
        Console.WriteLine("  Header thin lock → fat lock/sync block under contention.");
    }

    private static void DemoTryEnter()
    {
        Console.WriteLine("-- Monitor.TryEnter timeout --");
        object gate = new();
        bool taken = false;
        try
        {
            Monitor.Enter(gate, ref taken);
            bool other = Monitor.TryEnter(gate, TimeSpan.FromMilliseconds(10));
            // same thread reentrant
            Debug.Assert(other);
            if (other) Monitor.Exit(gate);
            Console.WriteLine($"  reentrant TryEnter={other}");
        }
        finally
        {
            if (taken) Monitor.Exit(gate);
        }
    }

    private static void DemoWaitPulse()
    {
        Console.WriteLine("-- Wait / Pulse condition variable style --");
        object gate = new();
        bool ready = false;
        int payload = 0;
        Thread worker = new Thread(() =>
        {
            Thread.Sleep(5);
            lock (gate)
            {
                payload = 99;
                ready = true;
                Monitor.Pulse(gate);
            }
        });
        worker.Start();
        lock (gate)
        {
            while (!ready)
                Monitor.Wait(gate, 2000);
            Debug.Assert(payload == 99);
            Console.WriteLine($"  Wait/Pulse delivered payload={payload}");
        }

        worker.Join(2000);
    }
}
