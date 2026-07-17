// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第1部分-async-await基础.md
// Stage    : Stage07_AsyncBasics
// Section  : Section01_AsyncAwaitBasics
// Item     : AsyncNotThreadIoVsCpu
// Topic id : stage07/section01/async_not_thread_io_vs_cpu
//
// 步骤 3：async/await 不创建线程；I/O 密集直接 await；CPU 密集用 Task.Run

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section01;

internal static class AsyncNotThreadIoVsCpu
{
    [LearnTopic("stage07/section01/async_not_thread_io_vs_cpu")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AsyncNotThreadIoVsCpu ===");
        DemoAsyncDoesNotCreateThread().GetAwaiter().GetResult();
        DemoIoBoundStyle().GetAwaiter().GetResult();
        DemoCpuBoundWithTaskRun().GetAwaiter().GetResult();
        DemoAwaitDoesNotParallelizeCpu();
        return 0;
    }

    private static async Task DemoAsyncDoesNotCreateThread()
    {
        Console.WriteLine("-- async/await does not create threads --");
        int before = Environment.CurrentManagedThreadId;
        await Task.Delay(5); // timer I/O-like: no dedicated wait thread for the delay itself
        int after = Environment.CurrentManagedThreadId;
        Console.WriteLine($"  thread before delay: {before}, after continuation: {after}");
        Console.WriteLine("  official rule: async/await keywords do not cause extra threads to be created");
        Debug.Assert(before > 0 && after > 0);
    }

    private static async Task DemoIoBoundStyle()
    {
        Console.WriteLine("-- I/O-bound: await real async APIs (here: Delay + temp file) --");
        string path = Path.Combine(Path.GetTempPath(), $"learncsharp-io-{Guid.NewGuid():N}.txt");
        try
        {
            string content = await SimulateIoAsync("hello-io");
            await File.WriteAllTextAsync(path, content);
            string roundTrip = await File.ReadAllTextAsync(path);
            Debug.Assert(roundTrip == "hello-io");
            Console.WriteLine($"  file async round-trip: {roundTrip}");
            Console.WriteLine("  waiting on I/O uses OS async completion, not a blocked worker per request");
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    private static async Task DemoCpuBoundWithTaskRun()
    {
        Console.WriteLine("-- CPU-bound: Task.Run moves work to thread pool --");
        int caller = Environment.CurrentManagedThreadId;
        int worker = -1;
        long sum = await Task.Run(() =>
        {
            worker = Environment.CurrentManagedThreadId;
            return HeavySum(50_000);
        });
        Debug.Assert(sum > 0);
        Console.WriteLine($"  caller thread={caller}, pool worker={worker}, sum={sum}");
        Console.WriteLine("  Task.Run is where threads are used for CPU work");
    }

    private static void DemoAwaitDoesNotParallelizeCpu()
    {
        Console.WriteLine("-- await alone does not speed up pure CPU loops --");
        long serial = HeavySum(20_000);
        // Wrong intuition: wrapping CPU in async without Task.Run / Parallel does not multi-core it.
        long same = HeavySum(20_000);
        Debug.Assert(serial == same);
        Console.WriteLine("  pure CPU stays single-threaded unless Task.Run / Parallel / PLINQ");
        Console.WriteLine("  anti-pattern: Task.Run(() => File.ReadAllText(...)) fakes async and wastes a thread");
    }

    private static async Task<string> SimulateIoAsync(string payload)
    {
        await Task.Delay(5);
        return payload;
    }

    private static long HeavySum(int n)
    {
        long acc = 0;
        for (int i = 1; i <= n; i++)
            acc += i;
        return acc;
    }
}
