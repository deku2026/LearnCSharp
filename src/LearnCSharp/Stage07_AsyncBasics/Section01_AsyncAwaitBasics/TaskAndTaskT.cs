// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第1部分-async-await基础.md
// Stage    : Stage07_AsyncBasics
// Section  : Section01_AsyncAwaitBasics
// Item     : TaskAndTaskT
// Topic id : stage07/section01/task_and_task_t
//
// 步骤 1：Task / Task<T> 是“结果的承诺”，不是线程；状态与 await 取结果

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section01;

internal static class TaskAndTaskT
{
    [LearnTopic("stage07/section01/task_and_task_t")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TaskAndTaskT ===");
        DemoTaskTPromise().GetAwaiter().GetResult();
        DemoTaskWithoutResult().GetAwaiter().GetResult();
        DemoTaskStatusAndProperties().GetAwaiter().GetResult();
        DemoTaskIsNotThread();
        return 0;
    }

    private static async Task DemoTaskTPromise()
    {
        Console.WriteLine("-- Task<T>: promise of a result --");
        Task<int> pending = ProduceAsync(21);
        Console.WriteLine($"  just started: IsCompleted={pending.IsCompleted}");
        int value = await pending;
        Console.WriteLine($"  after await: value={value}, IsCompleted={pending.IsCompleted}");
        Debug.Assert(value == 42);
        Debug.Assert(pending.IsCompleted);
        Debug.Assert(pending.Status == TaskStatus.RanToCompletion);
    }

    private static async Task DemoTaskWithoutResult()
    {
        Console.WriteLine("-- Task: async work with no return value --");
        Task sideEffect = SideEffectAsync();
        await sideEffect;
        Debug.Assert(sideEffect.IsCompletedSuccessfully);
        Console.WriteLine("  Task completed with no result payload");
    }

    private static async Task DemoTaskStatusAndProperties()
    {
        Console.WriteLine("-- Task status: Completed / Faulted / Canceled --");
        Task ok = Task.FromResult(1);
        Debug.Assert(ok.IsCompleted && !ok.IsFaulted && !ok.IsCanceled);

        Task faulted = FailAsync();
        try
        {
            await faulted;
            Debug.Fail("expected fault");
        }
        catch (InvalidOperationException)
        {
            Debug.Assert(faulted.IsFaulted);
            Debug.Assert(faulted.Exception is not null);
            Console.WriteLine($"  faulted: IsFaulted={faulted.IsFaulted}, Exception={faulted.Exception!.InnerException!.GetType().Name}");
        }

        using CancellationTokenSource cts = new();
        cts.Cancel();
        Task canceled = Task.FromCanceled(cts.Token);
        Debug.Assert(canceled.IsCanceled);
        Console.WriteLine($"  canceled: IsCanceled={canceled.IsCanceled}");
    }

    private static void DemoTaskIsNotThread()
    {
        Console.WriteLine("-- Task ≠ Thread --");
        Thread thread = new(() => { });
        Task delay = Task.Delay(1);
        Console.WriteLine($"  Thread is OS execution unit (ManagedThreadId exists): {thread.ManagedThreadId >= 0}");
        Console.WriteLine("  Task is a promise of async work; I/O Task may use zero waiting threads");
        Debug.Assert(delay is not null);
        delay.GetAwaiter().GetResult();
    }

    private static async Task<int> ProduceAsync(int n)
    {
        await Task.Delay(5);
        return n * 2;
    }

    private static async Task SideEffectAsync()
    {
        await Task.Delay(5);
    }

    private static async Task FailAsync()
    {
        await Task.Delay(5);
        throw new InvalidOperationException("faulted demo");
    }
}
