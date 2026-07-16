// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第1部分-async-await基础.md
// Stage    : Stage07_AsyncBasics
// Section  : Section01_AsyncAwaitBasics
// Item     : AsyncExceptions
// Topic id : stage07/section01/async_exceptions
//
// 步骤 5：异常存进 Task、await 重抛；参数验证延迟；async void 危险

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section01;

internal static class AsyncExceptions
{
    [LearnTopic("stage07/section01/async_exceptions")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AsyncExceptions ===");
        DemoExceptionStoredInTask().GetAwaiter().GetResult();
        DemoTryCatchAroundAwait().GetAwaiter().GetResult();
        DemoArgValidationDeferredUntilAwait().GetAwaiter().GetResult();
        DemoAsyncVoidCannotBeAwaitedSafely().GetAwaiter().GetResult();
        return 0;
    }

    private static async Task DemoExceptionStoredInTask()
    {
        Console.WriteLine("-- exception is stored in Task (Faulted), not thrown at throw site to caller --");
        Task<int> faulted = MightFailAsync();
        // Before await, task may already be faulted after the delay completes; observe via await.
        try
        {
            _ = await faulted;
            Debug.Fail("expected exception");
        }
        catch (InvalidOperationException ex)
        {
            Debug.Assert(faulted.IsFaulted);
            Debug.Assert(ex.Message == "出错了");
            Console.WriteLine($"  await rethrows: {ex.Message}; Status={faulted.Status}");
        }
    }

    private static async Task DemoTryCatchAroundAwait()
    {
        Console.WriteLine("-- try/catch around await works like sync code --");
        string? caught = null;
        try
        {
            await MightFailAsync();
        }
        catch (InvalidOperationException ex)
        {
            caught = ex.Message;
        }

        Debug.Assert(caught == "出错了");
        Console.WriteLine($"  caught: {caught}");
    }

    private static async Task DemoArgValidationDeferredUntilAwait()
    {
        Console.WriteLine("-- validation before first await still lands in Task --");
        Task bad = ProcessAsync(null!);
        // Method returned a Task immediately; exception surfaces when awaited.
        try
        {
            await bad;
            Debug.Fail("expected ArgumentNullException");
        }
        catch (ArgumentNullException ex)
        {
            Debug.Assert(ex.ParamName == "path");
            Console.WriteLine($"  deferred validation: {ex.GetType().Name} ParamName={ex.ParamName}");
        }
    }

    private static async Task DemoAsyncVoidCannotBeAwaitedSafely()
    {
        Console.WriteLine("-- async void: cannot await; exceptions escape the Task model --");
        // We demonstrate the safe pattern: async Task + try/catch.
        // Real async void would post exceptions to SynchronizationContext (often crash).
        Exception? escaped = null;
        try
        {
            await SafeFireAsync();
        }
        catch (Exception ex)
        {
            escaped = ex;
        }

        Debug.Assert(escaped is InvalidOperationException);
        Console.WriteLine("  async Task: caller can catch after await");
        Console.WriteLine("  async void: only for event handlers; always try/catch inside; prefer async Task");
    }

    private static async Task<int> MightFailAsync()
    {
        await Task.Delay(5);
        throw new InvalidOperationException("出错了");
    }

    private static async Task ProcessAsync(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        await Task.Delay(5);
        _ = path.Length;
    }

    private static async Task SafeFireAsync()
    {
        await Task.Delay(5);
        throw new InvalidOperationException("boom");
    }
}
