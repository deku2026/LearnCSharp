// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第10部分-并发内存模型.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section10_ConcurrentMemoryModel
// Item     : DotNet9LockAndPrimitives
// Topic id : stage11/section10/dotnet9_lock_and_primitives
//
// Lesson: System.Threading.Lock (.NET 9+), lock statement target; other primitives.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section10;

internal static class DotNet9LockAndPrimitives
{
    [LearnTopic("stage11/section10/dotnet9_lock_and_primitives")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DotNet9LockAndPrimitives ===");
        DemoSystemThreadingLock();
        DemoOtherPrimitives();
        DemoLockObjectVsLockType();
        return 0;
    }

    private static void DemoSystemThreadingLock()
    {
        Console.WriteLine("-- System.Threading.Lock (.NET 9+) --");
        var gate = new Lock();
        int n = 0;
        using (gate.EnterScope())
        {
            n = 1;
            Console.WriteLine("  EnterScope() held");
        }

        // lock statement can target Lock in modern C#
        lock (gate)
        {
            n = 2;
        }

        Debug.Assert(n == 2);
        Console.WriteLine($"  Lock value after critical sections n={n}");
        Console.WriteLine("  Dedicated Lock type avoids sync-block on arbitrary objects.");
    }

    private static void DemoOtherPrimitives()
    {
        Console.WriteLine("-- other primitives --");
        using var sem = new SemaphoreSlim(1, 1);
        bool ok = sem.Wait(0);
        Debug.Assert(ok);
        sem.Release();
        using var mre = new ManualResetEventSlim(false);
        mre.Set();
        Debug.Assert(mre.IsSet);
        Console.WriteLine("  SemaphoreSlim, ManualResetEventSlim, Mutex, ReaderWriterLockSlim, channels…");
        Console.WriteLine("  Prefer lock/Lock for simple mutual exclusion; special tools for special cases.");
    }

    private static void DemoLockObjectVsLockType()
    {
        Console.WriteLine("-- guidance --");
        Console.WriteLine("  Do not lock(this)/lock(typeof(T))/lock(string) in library code.");
        Console.WriteLine("  Private readonly object _gate = new(); or private readonly Lock _gate = new();");
        var classic = new object();
        lock (classic)
        {
            Debug.Assert(Monitor.IsEntered(classic));
        }

        Console.WriteLine($"  Environment.Version={Environment.Version}");
    }
}
