// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第2部分-异步进阶与陷阱.md
// Stage    : Stage07_AsyncBasics
// Section  : Section02_AsyncAdvancedAndPitfalls
// Item     : TaskCompositionWhenAllParallel
// Topic id : stage07/section02/task_composition_whenall_parallel
//
// 步骤 5：WhenAll/WhenAny 并发组合；Parallel/PLINQ 并行；并发 vs 并行

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section02;

internal static class TaskCompositionWhenAllParallel
{
    [LearnTopic("stage07/section02/task_composition_whenall_parallel")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TaskCompositionWhenAllParallel ===");
        DemoWhenAllConcurrent().GetAwaiter().GetResult();
        DemoWhenAnyTimeout().GetAwaiter().GetResult();
        DemoWhenAllExceptionFirst().GetAwaiter().GetResult();
        DemoParallelAndPlinq();
        DemoConcurrencyVsParallelism();
        return 0;
    }

    private static async Task DemoWhenAllConcurrent()
    {
        Console.WriteLine("-- WhenAll: start all, then await together --");
        Stopwatch swSerial = Stopwatch.StartNew();
        _ = await WorkAsync(40, "a");
        _ = await WorkAsync(40, "b");
        _ = await WorkAsync(40, "c");
        swSerial.Stop();

        Stopwatch swAll = Stopwatch.StartNew();
        Task<string> t1 = WorkAsync(40, "a");
        Task<string> t2 = WorkAsync(40, "b");
        Task<string> t3 = WorkAsync(40, "c");
        string[] results = await Task.WhenAll(t1, t2, t3);
        swAll.Stop();

        Debug.Assert(results is ["a", "b", "c"]);
        Console.WriteLine($"  serial ~{swSerial.ElapsedMilliseconds}ms; WhenAll ~{swAll.ElapsedMilliseconds}ms");
        Console.WriteLine("  concurrent I/O: total ≈ slowest, not sum");
        Debug.Assert(swAll.ElapsedMilliseconds < swSerial.ElapsedMilliseconds);
    }

    private static async Task DemoWhenAnyTimeout()
    {
        Console.WriteLine("-- WhenAny: work vs timeout --");
        Task<string> work = WorkAsync(80, "done");
        Task timeout = Task.Delay(20);
        Task winner = await Task.WhenAny(work, timeout);
        if (winner == timeout)
        {
            Console.WriteLine("  timeout won (work still running; cancel in real apps)");
            try
            {
                await work; // observe completion for clean demo exit
            }
            catch
            {
                // ignore
            }
        }
        else
        {
            Console.WriteLine($"  work won: {await work}");
        }

        Debug.Assert(winner == timeout || winner == work);
    }

    private static async Task DemoWhenAllExceptionFirst()
    {
        Console.WriteLine("-- WhenAll faults: await rethrows first; Exception has all --");
        Task a = FailAsync("A");
        Task b = FailAsync("B");
        Task all = Task.WhenAll(a, b);
        try
        {
            await all;
            Debug.Fail("expected fault");
        }
        catch (Exception ex)
        {
            Debug.Assert(all.IsFaulted);
            Debug.Assert(all.Exception is not null);
            int innerCount = all.Exception!.InnerExceptions.Count;
            Debug.Assert(innerCount == 2);
            Console.WriteLine($"  await saw: {ex.GetType().Name}; Aggregate InnerExceptions={innerCount}");
        }
    }

    private static void DemoParallelAndPlinq()
    {
        Console.WriteLine("-- Parallel / PLINQ: CPU multi-core --");
        int[] data = Enumerable.Range(0, 2000).ToArray();
        int[] results = new int[data.Length];

        Parallel.For(0, data.Length, i => results[i] = data[i] * data[i]);
        Debug.Assert(results[10] == 100);

        List<int> plinq = data.AsParallel()
            .Where(n => n % 2 == 0)
            .Select(n => n * 2)
            .ToList();
        Debug.Assert(plinq.Count == 1000);
        Console.WriteLine($"  Parallel.For squares; PLINQ evens*2 count={plinq.Count}");
        Console.WriteLine("  shared state needs lock / Interlocked / concurrent collections");
    }

    private static void DemoConcurrencyVsParallelism()
    {
        Console.WriteLine("-- concurrency vs parallelism --");
        Console.WriteLine("  concurrency (async/WhenAll): structure many I/O waits without blocking");
        Console.WriteLine("  parallelism (Parallel/PLINQ): execute CPU work on many cores at once");
        Console.WriteLine("  I/O-bound → concurrency; CPU-bound → parallelism");
        Debug.Assert(Environment.ProcessorCount >= 1);
    }

    private static async Task<string> WorkAsync(int delayMs, string tag)
    {
        await Task.Delay(delayMs);
        return tag;
    }

    private static async Task FailAsync(string name)
    {
        await Task.Delay(5);
        throw new InvalidOperationException(name);
    }
}
