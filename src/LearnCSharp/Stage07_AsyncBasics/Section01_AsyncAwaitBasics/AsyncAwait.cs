// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第1部分-async-await基础.md
// Stage    : Stage07_AsyncBasics
// Section  : Section01_AsyncAwaitBasics
// Item     : AsyncAwait
// Topic id : stage07/section01/async_await
//
// 步骤 2：async 方法解剖、await 不阻塞、返回类型 Task / Task<T> / IAsyncEnumerable

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section01;

internal static class AsyncAwait
{
    [LearnTopic("stage07/section01/async_await")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AsyncAwait ===");
        DemoAsyncTaskAndTaskT().GetAwaiter().GetResult();
        DemoChainedAwaits().GetAwaiter().GetResult();
        DemoAwaitDoesNotBlockThread().GetAwaiter().GetResult();
        DemoAsyncEnumerable().GetAwaiter().GetResult();
        return 0;
    }

    private static async Task DemoAsyncTaskAndTaskT()
    {
        Console.WriteLine("-- async Task / Task<T> return types --");
        await DoAsync();
        int n = await GetAsync();
        Debug.Assert(n == 42);
        Console.WriteLine($"  DoAsync done; GetAsync → {n}");
        // async is an implementation detail: callers only see Task / Task<T>.
        Func<Task<int>> asDelegate = GetAsync;
        Debug.Assert(asDelegate is not null);
    }

    private static async Task DemoChainedAwaits()
    {
        Console.WriteLine("-- chain: second await uses first result --");
        string result = await DownloadAndProcessAsync("sample");
        Debug.Assert(result == "SAMPLE!");
        Console.WriteLine($"  chained result: {result}");
    }

    private static async Task DemoAwaitDoesNotBlockThread()
    {
        Console.WriteLine("-- await yields; thread can do other work --");
        int workerThreadId = -1;
        Task background = Task.Run(async () =>
        {
            workerThreadId = Environment.CurrentManagedThreadId;
            await Task.Delay(20);
        });

        int mainBefore = Environment.CurrentManagedThreadId;
        // While background is pending, this thread continues (no .Wait on UI-style block).
        int busyWork = 0;
        for (int i = 0; i < 1000; i++)
            busyWork += i;

        await background;
        Debug.Assert(busyWork > 0);
        Console.WriteLine($"  caller thread before await-complete path: {mainBefore}");
        Console.WriteLine($"  Task.Run worker thread id: {workerThreadId}");
        Console.WriteLine("  await registers continuation then returns control (no thread spin-wait)");
    }

    private static async Task DemoAsyncEnumerable()
    {
        Console.WriteLine("-- IAsyncEnumerable + await foreach --");
        List<int> list = [];
        await foreach (int x in StreamAsync())
            list.Add(x);
        Debug.Assert(list is [1, 2, 3]);
        Console.WriteLine($"  stream: [{string.Join(", ", list)}]");
    }

    private static async Task DoAsync()
    {
        await Task.Delay(5);
    }

    private static async Task<int> GetAsync()
    {
        await Task.Delay(5);
        return 42;
    }

    private static async Task<string> DownloadAndProcessAsync(string payload)
    {
        string data = await FetchAsync(payload);
        string processed = data.ToUpperInvariant() + "!";
        await Task.Delay(5); // stand-in for WriteAllTextAsync
        return processed;
    }

    private static async Task<string> FetchAsync(string payload)
    {
        await Task.Delay(5);
        return payload;
    }

    private static async IAsyncEnumerable<int> StreamAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return 1;
        await Task.Delay(5, cancellationToken);
        yield return 2;
        await Task.Delay(5, cancellationToken);
        yield return 3;
    }
}
