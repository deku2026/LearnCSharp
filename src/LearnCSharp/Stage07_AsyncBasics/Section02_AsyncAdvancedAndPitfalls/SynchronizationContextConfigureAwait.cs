// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第2部分-异步进阶与陷阱.md
// Stage    : Stage07_AsyncBasics
// Section  : Section02_AsyncAdvancedAndPitfalls
// Item     : SynchronizationContextConfigureAwait
// Topic id : stage07/section02/synchronization_context_configureawait
//
// 步骤 1：await 默认捕获 SynchronizationContext；ConfigureAwait(false) 库代码准则
// 安装自定义上下文：证明 default await 回帖；ConfigureAwait(false) 不回帖

using System.Collections.Concurrent;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section02;

internal static class SynchronizationContextConfigureAwait
{
    [LearnTopic("stage07/section02/synchronization_context_configureawait")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SynchronizationContextConfigureAwait ===");
        DemoCurrentContextOnConsole();
        DemoDefaultAwaitPostsBackToContext();
        DemoConfigureAwaitFalseDoesNotPostBack();
        DemoLibraryVsUiGuidance().GetAwaiter().GetResult();
        return 0;
    }

    private static void DemoCurrentContextOnConsole()
    {
        Console.WriteLine("-- console / ASP.NET Core: usually no SynchronizationContext --");
        SynchronizationContext? ctx = SynchronizationContext.Current;
        Console.WriteLine($"  SynchronizationContext.Current is null: {ctx is null}");
        Console.WriteLine("  UI (WPF/WinForms): single-thread context so await resumes on UI thread");
        Console.WriteLine("  classic ASP.NET: request context; ASP.NET Core: none by default");
    }

    private static void DemoDefaultAwaitPostsBackToContext()
    {
        Console.WriteLine("-- default await posts continuation back to captured SyncContext --");
        RecordingSynchronizationContext ctx = new RecordingSynchronizationContext();
        SynchronizationContext? previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(ctx);
        try
        {
            int postsBefore = ctx.PostCount;
            Task work = AwaitDefaultAsync(ctx);
            // Pump until the continuation runs (or timeout).
            bool ok = ctx.PumpUntil(() => work.IsCompleted, TimeSpan.FromSeconds(2));
            Debug.Assert(ok && work.IsCompletedSuccessfully);
            Debug.Assert(ctx.PostCount > postsBefore);
            Debug.Assert(ctx.LastPostedOnContext);
            Console.WriteLine($"  Post count increased: {postsBefore} → {ctx.PostCount}");
            Console.WriteLine("  continuation ran on context (posted back) ✓");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
            ctx.Complete();
        }
    }

    private static void DemoConfigureAwaitFalseDoesNotPostBack()
    {
        Console.WriteLine("-- ConfigureAwait(false): does NOT post back to SyncContext --");
        RecordingSynchronizationContext ctx = new RecordingSynchronizationContext();
        SynchronizationContext? previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(ctx);
        try
        {
            int postsBefore = ctx.PostCount;
            bool resumedOnContext = true;
            using ManualResetEventSlim done = new ManualResetEventSlim(false);

            async Task WorkAsync()
            {
                await Task.Delay(15).ConfigureAwait(false);
                // After ConfigureAwait(false), we should not be on the recording context.
                resumedOnContext = ReferenceEquals(SynchronizationContext.Current, ctx);
                done.Set();
            }

            Task work = WorkAsync();
            // Do not require posts: false path continues on thread-pool.
            bool signaled = done.Wait(TimeSpan.FromSeconds(2));
            Debug.Assert(signaled && work.IsCompletedSuccessfully);
            Debug.Assert(!resumedOnContext);
            // Post may still be 0 for this await path (no capture).
            Console.WriteLine($"  posts during ConfigureAwait(false) path: {ctx.PostCount - postsBefore} (expect 0 for that await)");
            Console.WriteLine($"  resumed with same SyncContext: {resumedOnContext} (expect false)");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
            ctx.Complete();
        }
    }

    private static async Task DemoLibraryVsUiGuidance()
    {
        Console.WriteLine("-- library vs UI guidance --");
        string data = await LibraryFetchAsync("payload").ConfigureAwait(false);
        Debug.Assert(data == "PAYLOAD");
        Console.WriteLine($"  library result: {data}");
        Console.WriteLine("  UI code that touches controls after await: keep default ConfigureAwait(true)");
        Console.WriteLine("  ASP.NET Core: ConfigureAwait(false) is a no-op for context, still good for libraries");
    }

    private static async Task AwaitDefaultAsync(RecordingSynchronizationContext expected)
    {
        await Task.Delay(15); // captures expected context
        Debug.Assert(ReferenceEquals(SynchronizationContext.Current, expected));
        expected.LastPostedOnContext = true;
    }

    private static async Task<string> LibraryFetchAsync(string input)
    {
        await Task.Delay(5).ConfigureAwait(false);
        return input.ToUpperInvariant();
    }

    private sealed class RecordingSynchronizationContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _queue = new();
        private int _postCount;
        private volatile bool _completed;

        public int PostCount => Volatile.Read(ref _postCount);
        public bool LastPostedOnContext { get; set; }

        public override void Post(SendOrPostCallback d, object? state)
        {
            Interlocked.Increment(ref _postCount);
            _queue.Enqueue((d, state));
        }

        public override void Send(SendOrPostCallback d, object? state) => d(state);

        public void Complete() => _completed = true;

        public bool PumpUntil(Func<bool> predicate, TimeSpan budget)
        {
            long deadline = Environment.TickCount64 + (long)budget.TotalMilliseconds;
            while (Environment.TickCount64 < deadline)
            {
                while (_queue.TryDequeue(out (SendOrPostCallback Callback, object? State) item))
                    item.Callback(item.State);

                if (predicate())
                    return true;

                if (_completed && _queue.IsEmpty)
                    break;

                Thread.Sleep(1);
            }

            return predicate();
        }
    }
}
