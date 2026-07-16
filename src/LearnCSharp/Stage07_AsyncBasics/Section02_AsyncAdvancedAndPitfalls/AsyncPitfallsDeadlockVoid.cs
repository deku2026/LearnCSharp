// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第2部分-异步进阶与陷阱.md
// Stage    : Stage07_AsyncBasics
// Section  : Section02_AsyncAdvancedAndPitfalls
// Item     : AsyncPitfallsDeadlockVoid
// Topic id : stage07/section02/async_pitfalls_deadlock_void
//
// 步骤 2：sync-over-async 死锁机制、async void、未 await、async all the way
// 控制台无 UI SyncContext——用自定义单线程上下文 + 超时保证不挂死

using System.Collections.Concurrent;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section02;

internal static class AsyncPitfallsDeadlockVoid
{
    [LearnTopic("stage07/section02/async_pitfalls_deadlock_void")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AsyncPitfallsDeadlockVoid ===");
        DemoSyncOverAsyncCompletedPath();
        DemoSyncOverAsyncDeadlockWithTimeout();
        DemoAsyncAllTheWay().GetAwaiter().GetResult();
        DemoAsyncVoidOuterCannotCatch();
        DemoFireAndForgetObserveWithoutAwaitAsSuccess();
        return 0;
    }

    private static void DemoSyncOverAsyncCompletedPath()
    {
        Console.WriteLine("-- sync-over-async: GetResult on already-completed Task is safe --");
        Task<string> done = Task.FromResult("data");
        string viaGetResult = done.GetAwaiter().GetResult();
        Debug.Assert(viaGetResult == "data");
        Console.WriteLine($"  completed Task.GetAwaiter().GetResult() → {viaGetResult}");
        Console.WriteLine("  UI deadlock recipe: block UI thread with .Result/.Wait while await needs that same thread");
    }

    private static void DemoSyncOverAsyncDeadlockWithTimeout()
    {
        Console.WriteLine("-- sync-over-async deadlock (custom single-thread SyncContext + timeout) --");
        // UI-like: one thread owns the context queue. Blocking that thread with Wait prevents pumping.
        var ctx = new SingleThreadSynchronizationContext();
        SynchronizationContext? previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(ctx);
        try
        {
            async Task WorkAsync()
            {
                await Task.Delay(30); // continuation posts back to ctx
            }

            Task work = WorkAsync();
            // Block the context thread: continuation never runs → would hang forever without timeout.
            bool finished = work.Wait(TimeSpan.FromMilliseconds(250));
            Debug.Assert(!finished);
            Console.WriteLine($"  Wait(250ms) returned finished={finished} (timeout = would-be deadlock)");
            Console.WriteLine("  fix: async all the way, or ConfigureAwait(false) in libraries");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
            ctx.Complete();
        }
    }

    private static async Task DemoAsyncAllTheWay()
    {
        Console.WriteLine("-- iron rule: async all the way --");
        string result = await ControllerAsync();
        Debug.Assert(result == "row");
        Console.WriteLine($"  Controller → Service → Repository: {result}");
        Console.WriteLine("  do not sandwich .Result/.Wait() in the middle of an async chain");
    }

    private static void DemoAsyncVoidOuterCannotCatch()
    {
        Console.WriteLine("-- async void: catch INSIDE method; outer try cannot catch --");
        bool innerCaught = false;
        bool outerCaught = false;
        var done = new ManualResetEventSlim(false);

        async void EventHandlerStyle()
        {
            try
            {
                await Task.Delay(10);
                throw new InvalidOperationException("async-void boom");
            }
            catch (InvalidOperationException ex)
            {
                // Required: without this, exception hits SyncContext / process (not caller's catch).
                innerCaught = true;
                Console.WriteLine($"  inner catch (must be inside async void): {ex.Message}");
            }
            finally
            {
                done.Set();
            }
        }

        try
        {
            EventHandlerStyle(); // returns immediately; post-await faults never reach here
        }
        catch (Exception)
        {
            outerCaught = true;
        }

        bool signaled = done.Wait(TimeSpan.FromSeconds(2));
        Debug.Assert(signaled);
        Debug.Assert(innerCaught);
        Debug.Assert(!outerCaught);
        Console.WriteLine($"  outerCaught={outerCaught} (caller try never sees async void faults)");
        Console.WriteLine("  only legitimate async void: event handlers with internal try/catch");
    }

    private static void DemoFireAndForgetObserveWithoutAwaitAsSuccess()
    {
        Console.WriteLine("-- fire-and-forget: do not treat start as success; observe via ContinueWith --");
        bool observed = false;
        bool completedOk = false;
        Exception? fault = null;
        var gate = new ManualResetEventSlim(false);

        Task background = BackgroundWorkAsync();
        // Intentionally not: await background;  — that would be the "success path"
        _ = background.ContinueWith(
            t =>
            {
                observed = true;
                completedOk = t.IsCompletedSuccessfully;
                fault = t.Exception?.GetBaseException();
                gate.Set();
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

        Console.WriteLine("  started background Task (not awaited as success)");
        bool signaled = gate.Wait(TimeSpan.FromSeconds(2));
        Debug.Assert(signaled && observed);
        Debug.Assert(completedOk);
        Debug.Assert(fault is null);
        Console.WriteLine($"  observed via ContinueWith: completedOk={completedOk}");
        Console.WriteLine("  CS4014 warns on discarded async calls; always observe exceptions");
    }

    private static async Task BackgroundWorkAsync()
    {
        await Task.Delay(15);
    }

    private static async Task<string> ControllerAsync() => await ServiceAsync();
    private static async Task<string> ServiceAsync() => await RepositoryAsync();
    private static async Task<string> RepositoryAsync()
    {
        await Task.Delay(5);
        return "row";
    }

    /// <summary>Minimal single-thread context: Post enqueues; nothing pumps while the owner blocks.</summary>
    private sealed class SingleThreadSynchronizationContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _queue = new();
        private volatile bool _completed;

        public override void Post(SendOrPostCallback d, object? state) => _queue.Enqueue((d, state));

        public override void Send(SendOrPostCallback d, object? state)
        {
            // Same-thread send would re-enter; for the deadlock demo we only need Post.
            d(state);
        }

        public void Complete() => _completed = true;

        public void PumpUntilEmpty(TimeSpan budget)
        {
            long deadline = Environment.TickCount64 + (long)budget.TotalMilliseconds;
            while (Environment.TickCount64 < deadline)
            {
                if (_queue.TryDequeue(out (SendOrPostCallback Callback, object? State) item))
                    item.Callback(item.State);
                else if (_completed)
                    break;
                else
                    Thread.Sleep(1);
            }
        }
    }
}
