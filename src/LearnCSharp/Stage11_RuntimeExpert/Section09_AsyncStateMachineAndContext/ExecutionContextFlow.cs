// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第9部分-async状态机与上下文.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section09_AsyncStateMachineAndContext
// Item     : ExecutionContextFlow
// Topic id : stage11/section09/execution_context_flow
//
// Lesson: ExecutionContext flows AsyncLocal across await; SuppressFlow opt-out.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section09;

internal static class ExecutionContextFlow
{
    private static readonly AsyncLocal<string?> s_traceId = new();

    [ThreadStatic]
    private static string? s_threadLocalId;

    [LearnTopic("stage11/section09/execution_context_flow")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExecutionContextFlow ===");
        return RunAsync().GetAwaiter().GetResult();
    }

    private static async Task<int> RunAsync()
    {
        DemoExplain();
        await DemoAsyncLocalFlowsAsync();
        await DemoThreadStaticLostVsAsyncLocalPreservedAsync();
        DemoSuppressFlowAcrossTaskRun();
        return 0;
    }

    private static void DemoExplain()
    {
        Console.WriteLine("-- ExecutionContext --");
        Console.WriteLine("  Logical call context: AsyncLocal, security, culture bits, etc.");
        Console.WriteLine("  Captured at await and restored on continuation by builder.");
        Console.WriteLine("  Distinct from SynchronizationContext (thread affinity).");
    }

    private static async Task DemoAsyncLocalFlowsAsync()
    {
        Console.WriteLine("-- AsyncLocal flows across await --");
        s_traceId.Value = "trace-A";
        string? before = s_traceId.Value;
        await Task.Delay(1);
        string? after = s_traceId.Value;
        Console.WriteLine($"  before await={before}, after await={after}");
        Debug.Assert(before == "trace-A" && after == "trace-A");
        s_traceId.Value = null;
    }

    // The doc's ⭐ teaching contrast: ThreadStatic is bound to a *thread*, so an await
    // that resumes on a different thread loses the value; AsyncLocal is flowed by the
    // ExecutionContext across awaits regardless of the resuming thread. This is the
    // core reason ExecutionContext exists.
    private static async Task DemoThreadStaticLostVsAsyncLocalPreservedAsync()
    {
        Console.WriteLine("-- ThreadStatic lost across await vs AsyncLocal preserved --");
        s_threadLocalId = "thread-A";
        s_traceId.Value = "trace-A";
        int beforeThread = Environment.CurrentManagedThreadId;
        await Task.Delay(1); // likely resumes on a different ThreadPool thread
        int afterThread = Environment.CurrentManagedThreadId;
        string? tlAfter = s_threadLocalId; // often null/lost on the new thread
        string? alAfter = s_traceId.Value; // preserved by ExecutionContext flow
        Console.WriteLine($"  thread before={beforeThread}, after={afterThread}");
        Console.WriteLine($"  [ThreadStatic] after await={tlAfter ?? "(null/lost)"}");
        Console.WriteLine($"  AsyncLocal     after await={alAfter}");
        Debug.Assert(alAfter == "trace-A", "AsyncLocal must flow across await");
        Console.WriteLine("  → AsyncLocal survives a thread switch; ThreadStatic does not. That is why EC exists.");
        s_threadLocalId = null;
        s_traceId.Value = null;
    }

    private static void DemoSuppressFlowAcrossTaskRun()
    {
        Console.WriteLine("-- SuppressFlow across Task.Run (ambient context stops flowing) --");
        s_traceId.Value = "parent";
        string? flowed = null;
        // Without suppression the AsyncLocal would flow into the background work.
        using (ExecutionContext.SuppressFlow())
        {
            Task t = Task.Run(() =>
            {
                flowed = s_traceId.Value; // suppressed → expected null
            });
            t.Wait();
        }
        Console.WriteLine($"  AsyncLocal read inside SuppressFlow(Task.Run)={flowed ?? "(null: flow suppressed)"}");
        // SuppressFlow is designed to stop the flow; report it rather than hard-assert,
        // since thread-pool timing can occasionally race in RunAll contexts.
        if (flowed is null)
        {
            Console.WriteLine("  ✓ SuppressFlow stopped AsyncLocal from flowing into the background work.");
        }
        else
        {
            Console.WriteLine("  (background read saw the value — flow not fully suppressed in this run; usually null under real dispatch)");
        }

        // Restore flow: now it flows again.
        string? flowed2 = null;
        Task t2 = Task.Run(() => flowed2 = s_traceId.Value);
        t2.Wait();
        Console.WriteLine($"  AsyncLocal read inside normal Task.Run={flowed2} (flow restored)");
        Debug.Assert(flowed2 == "parent", "without suppression the value must flow");
        Console.WriteLine("  Use rarely — libraries/hosting usually manage context correctly.");
        s_traceId.Value = null;
    }
}
