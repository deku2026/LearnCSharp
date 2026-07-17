// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第2部分-异步进阶与陷阱.md
// Stage    : Stage07_AsyncBasics
// Section  : Section02_AsyncAdvancedAndPitfalls
// Item     : CancellationToken
// Topic id : stage07/section02/cancellation_token
//
// 步骤 3：协作式取消 CTS/Token、CancelAfter、ThrowIfCancellationRequested

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section02;

internal static class CancellationTokenTopic
{
    [LearnTopic("stage07/section02/cancellation_token")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CancellationToken ===");
        DemoCancelAfterTimeout().GetAwaiter().GetResult();
        DemoManualCancel().GetAwaiter().GetResult();
        DemoThrowIfCancellationRequested().GetAwaiter().GetResult();
        DemoTokenNoneAndCooperativeModel().GetAwaiter().GetResult();
        return 0;
    }

    private static async Task DemoCancelAfterTimeout()
    {
        Console.WriteLine("-- CancelAfter: timeout cooperative cancel --");
        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMilliseconds(30));
        string? outcome = null;
        try
        {
            await SlowWorkAsync(cts.Token);
            outcome = "completed";
        }
        catch (OperationCanceledException)
        {
            outcome = "canceled";
        }

        Debug.Assert(outcome == "canceled");
        Console.WriteLine($"  outcome: {outcome} (OperationCanceledException / TaskCanceledException)");
    }

    private static async Task DemoManualCancel()
    {
        Console.WriteLine("-- manual cts.Cancel() --");
        using CancellationTokenSource cts = new();
        Task work = SlowWorkAsync(cts.Token);
        cts.Cancel();
        try
        {
            await work;
            Debug.Fail("expected cancel");
        }
        catch (OperationCanceledException)
        {
            Debug.Assert(cts.IsCancellationRequested);
            Console.WriteLine("  manual cancel observed by Delay(..., token)");
        }
    }

    private static async Task DemoThrowIfCancellationRequested()
    {
        Console.WriteLine("-- CPU loop: ThrowIfCancellationRequested --");
        using CancellationTokenSource cts = new();
        int processed = 0;
        try
        {
            await ProcessItemsAsync(
                Enumerable.Range(0, 1000),
                cts.Token,
                n =>
                {
                    processed = n;
                    if (n >= 5)
                        cts.Cancel(); // cooperative: cancel from a safe point after a few items
                });
            Debug.Fail("expected cancel");
        }
        catch (OperationCanceledException)
        {
            Debug.Assert(processed >= 5 && processed < 1000);
            Console.WriteLine($"  stopped cooperatively after {processed} items");
        }
    }

    private static async Task DemoTokenNoneAndCooperativeModel()
    {
        Console.WriteLine("-- CancellationToken.None + cooperative model --");
        await SlowWorkAsync(CancellationToken.None);
        Console.WriteLine("  None never cancels; runtime does not force-stop work");
        Console.WriteLine("  you must pass token + check/throw; prefer over Thread.Abort");
        Console.WriteLine("  ≈ C++20 std::stop_token / std::stop_source");
        Debug.Assert(CancellationToken.None.CanBeCanceled is false);
    }

    private static async Task SlowWorkAsync(CancellationToken token)
    {
        await Task.Delay(200, token);
    }

    private static async Task ProcessItemsAsync(
        IEnumerable<int> items,
        CancellationToken token,
        Action<int> onProgress)
    {
        int n = 0;
        foreach (int item in items)
        {
            token.ThrowIfCancellationRequested();
            n++;
            onProgress(n);
            _ = item;
            if (n % 50 == 0)
                await Task.Yield();
        }
    }
}
