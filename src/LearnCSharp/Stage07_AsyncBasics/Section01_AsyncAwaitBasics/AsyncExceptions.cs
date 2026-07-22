// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第1部分-async-await基础.md
// Stage    : Stage07_AsyncBasics
// Section  : Section01_AsyncAwaitBasics
// Item     : AsyncExceptions
// Topic id : stage07/section01/async_exceptions
//
// 步骤 5：异常存进 Task、await 重抛；参数验证延迟；async void 危险；WhenAll 多异常

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
        DemoNonAsyncValidationThrowsBeforeTask();
        DemoAsyncVoidCatchInsideOnly();
        DemoWhenAllAggregateException();
        return 0;
    }

    private static async Task DemoExceptionStoredInTask()
    {
        Console.WriteLine("-- exception is stored in Task (Faulted), not thrown at throw site to caller --");
        Task<int> faulted = MightFailAsync();
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
        Console.WriteLine("-- async method: validation before first await still lands in Task --");
        Task bad = ProcessAsync(null!);
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

    private static void DemoNonAsyncValidationThrowsBeforeTask()
    {
        Console.WriteLine("-- non-async wrapper: throw before returning Task (eager) --");
        bool threwSync = false;
        try
        {
            // Not async: exception escapes immediately — caller never gets a Task.
            _ = ProcessValidatedAsync(null!);
            Debug.Fail("expected sync throw");
        }
        catch (ArgumentNullException ex)
        {
            threwSync = true;
            Debug.Assert(ex.ParamName == "path");
            Console.WriteLine($"  sync throw before Task: {ex.GetType().Name} ParamName={ex.ParamName}");
        }

        Debug.Assert(threwSync);
        Console.WriteLine("  pattern: validate in non-async wrapper, then return CoreAsync()");
    }

    private static void DemoAsyncVoidCatchInsideOnly()
    {
        Console.WriteLine("-- async void carefully: catch inside; outer try cannot catch --");
        bool innerCaught = false;
        bool outerCaught = false;
        using ManualResetEventSlim done = new ManualResetEventSlim(false);

        async void RiskyHandler()
        {
            try
            {
                await Task.Delay(10);
                throw new InvalidOperationException("async void fault");
            }
            catch (InvalidOperationException ex)
            {
                innerCaught = true;
                Console.WriteLine($"  caught inside async void: {ex.Message}");
            }
            finally
            {
                done.Set();
            }
        }

        try
        {
            RiskyHandler();
        }
        catch (Exception)
        {
            outerCaught = true;
        }

        bool signaled = done.Wait(TimeSpan.FromSeconds(2));
        Debug.Assert(signaled && innerCaught && !outerCaught);
        Console.WriteLine($"  outerCaught={outerCaught}; prefer async Task for testable error flow");
    }

    private static void DemoWhenAllAggregateException()
    {
        Console.WriteLine("-- WhenAll multi-exception: AggregateException (via .Exception / Wait) --");
        Task a = Task.FromException(new InvalidOperationException("fault-A"));
        Task b = Task.FromException(new InvalidOperationException("fault-B"));
        Task all = Task.WhenAll(a, b);

        // await WhenAll rethrows only the first exception; full set is on Task.Exception.
        Exception? firstFromAwait = null;
        try
        {
            all.GetAwaiter().GetResult();
            Debug.Fail("expected fault");
        }
        catch (InvalidOperationException ex)
        {
            firstFromAwait = ex;
        }

        Debug.Assert(firstFromAwait is not null);
        Debug.Assert(all.IsFaulted);
        Debug.Assert(all.Exception is not null);

        AggregateException agg = all.Exception.Flatten();
        Debug.Assert(agg.InnerExceptions.Count == 2);
        Debug.Assert(agg.InnerExceptions.Any(e => e.Message == "fault-A"));
        Debug.Assert(agg.InnerExceptions.Any(e => e.Message == "fault-B"));

        // Explicit AggregateException catch path (e.g. task.Wait() wraps).
        try
        {
            Task.WaitAll(a, b);
            Debug.Fail("expected AggregateException");
        }
        catch (AggregateException ae)
        {
            AggregateException flat = ae.Flatten();
            Debug.Assert(flat.InnerExceptions.Count >= 2);
            Console.WriteLine($"  WaitAll AggregateException inners={flat.InnerExceptions.Count}");
        }

        Console.WriteLine($"  WhenAll.Exception inners={agg.InnerExceptions.Count}; await surfaces first only");
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

    /// <summary>Non-async wrapper: validation throws before any Task exists.</summary>
    private static Task ProcessValidatedAsync(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return ProcessCoreAsync(path);
    }

    private static async Task ProcessCoreAsync(string path)
    {
        await Task.Delay(5);
        _ = path.Length;
    }
}
