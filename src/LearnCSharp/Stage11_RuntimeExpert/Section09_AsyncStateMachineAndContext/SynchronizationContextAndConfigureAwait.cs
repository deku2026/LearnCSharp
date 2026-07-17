// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第9部分-async状态机与上下文.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section09_AsyncStateMachineAndContext
// Item     : SynchronizationContextAndConfigureAwait
// Topic id : stage11/section09/synchronization_context_and_configureawait
//
// Lesson: SyncContext posts continuations (UI); ConfigureAwait(false) skips capture.

using System.Collections.Concurrent;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section09;

internal static class SynchronizationContextAndConfigureAwait
{
    [LearnTopic("stage11/section09/synchronization_context_and_configureawait")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SynchronizationContextAndConfigureAwait ===");
        return RunAsync().GetAwaiter().GetResult();
    }

    private static async Task<int> RunAsync()
    {
        DemoCurrent();
        await DemoCustomContextCaptureAsync();
        await DemoConfigureAwaitFalseSkipsContextAsync();
        DemoGuidance();
        return 0;
    }

    private static void DemoCurrent()
    {
        Console.WriteLine("-- current SynchronizationContext --");
        SynchronizationContext? ctx = SynchronizationContext.Current;
        Console.WriteLine($"  SynchronizationContext.Current={(ctx is null ? "null (console default)" : ctx.GetType().Name)}");
        Debug.Assert(ctx is null);
    }

    private static async Task DemoCustomContextCaptureAsync()
    {
        Console.WriteLine("-- custom SyncContext: await captures and resumes on it --");
        SingleThreadSyncContext ctx = new SingleThreadSyncContext();
        SynchronizationContext? previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(ctx);
        try
        {
            int pumpId = Environment.CurrentManagedThreadId;
            // Run the async work on the pump thread so Current is our context at await.
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            ctx.Post(_ =>
            {
                ResumeOnContextAsync(tcs).ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted) tcs.TrySetException(t.Exception!.InnerExceptions);
                    },
                    TaskContinuationOptions.ExecuteSynchronously);
            }, null);

            // Pump until completion (bounded)
            bool completed = ctx.RunUntil(() => tcs.Task.IsCompleted, TimeSpan.FromSeconds(5));
            Debug.Assert(completed, "custom context pump timed out");
            int resumeThread = await tcs.Task.ConfigureAwait(false);
            Console.WriteLine($"  pump/resume thread id={resumeThread} (same single-thread context)");
            Debug.Assert(resumeThread != 0);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
            ctx.Complete();
        }
    }

    private static async Task ResumeOnContextAsync(TaskCompletionSource<int> tcs)
    {
        Debug.Assert(SynchronizationContext.Current is SingleThreadSyncContext);
        await Task.Delay(10); // default ConfigureAwait(true) → capture our SyncContext
        Debug.Assert(SynchronizationContext.Current is SingleThreadSyncContext,
            "after await with ConfigureAwait(true), should resume on custom SyncContext");
        int id = Environment.CurrentManagedThreadId;
        Console.WriteLine($"  resumed on SyncContext type={SynchronizationContext.Current?.GetType().Name}, tid={id}");
        tcs.TrySetResult(id);
    }

    private static async Task DemoConfigureAwaitFalseSkipsContextAsync()
    {
        Console.WriteLine("-- ConfigureAwait(false): skip captured SyncContext --");
        SingleThreadSyncContext ctx = new SingleThreadSyncContext();
        SynchronizationContext? previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(ctx);
        try
        {
            TaskCompletionSource<(bool hadContext, string? typeName)> tcs = new TaskCompletionSource<(bool hadContext, string? typeName)>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            ctx.Post(_ =>
            {
                SkipContextAsync(tcs).ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted) tcs.TrySetException(t.Exception!.InnerExceptions);
                    },
                    TaskContinuationOptions.ExecuteSynchronously);
            }, null);

            bool completed = ctx.RunUntil(() => tcs.Task.IsCompleted, TimeSpan.FromSeconds(5));
            Debug.Assert(completed);
            (bool hadContext, string? typeName) = await tcs.Task.ConfigureAwait(false);
            Console.WriteLine($"  after ConfigureAwait(false): has SyncContext={hadContext}, type={typeName ?? "null"}");
            Debug.Assert(!hadContext, "ConfigureAwait(false) should not restore custom SyncContext");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
            ctx.Complete();
        }
    }

    private static async Task SkipContextAsync(TaskCompletionSource<(bool, string?)> tcs)
    {
        Debug.Assert(SynchronizationContext.Current is SingleThreadSyncContext);
        await Task.Delay(10).ConfigureAwait(false);
        SynchronizationContext? cur = SynchronizationContext.Current;
        tcs.TrySetResult((cur is not null, cur?.GetType().Name));
    }

    private static void DemoGuidance()
    {
        Console.WriteLine("-- guidance --");
        Console.WriteLine("  UI app code: default await to return to UI SyncContext.");
        Console.WriteLine("  Libraries: ConfigureAwait(false) to avoid forcing caller context.");
        Console.WriteLine("  ASP.NET Core: no SyncContext; still avoid sync-over-async.");
    }

    /// <summary>Minimal single-thread SynchronizationContext with an explicit pump (no hang).</summary>
    private sealed class SingleThreadSyncContext : SynchronizationContext
    {
        private readonly BlockingCollection<(SendOrPostCallback cb, object? state)> _queue = new();

        public override void Post(SendOrPostCallback d, object? state)
            => _queue.Add((d, state));

        public override void Send(SendOrPostCallback d, object? state)
        {
            if (Current == this)
            {
                d(state);
                return;
            }

            using ManualResetEventSlim done = new ManualResetEventSlim(false);
            Exception? error = null;
            Post(_ =>
            {
                try { d(state); }
                catch (Exception ex) { error = ex; }
                finally { done.Set(); }
            }, null);
            done.Wait(TimeSpan.FromSeconds(5));
            if (error is not null) throw error;
        }

        public bool RunUntil(Func<bool> predicate, TimeSpan timeout)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (!predicate())
            {
                if (sw.Elapsed > timeout)
                    return false;
                if (_queue.TryTake(out (SendOrPostCallback cb, object? state) item, TimeSpan.FromMilliseconds(50)))
                {
                    SynchronizationContext? prev = Current;
                    SetSynchronizationContext(this);
                    try { item.cb(item.state); }
                    finally { SetSynchronizationContext(prev); }
                }
            }

            // Drain remaining work briefly
            while (_queue.TryTake(out (SendOrPostCallback cb, object? state) item, TimeSpan.FromMilliseconds(20)))
            {
                SynchronizationContext? prev = Current;
                SetSynchronizationContext(this);
                try { item.cb(item.state); }
                finally { SetSynchronizationContext(prev); }
            }

            return true;
        }

        public void Complete() => _queue.CompleteAdding();
    }
}
