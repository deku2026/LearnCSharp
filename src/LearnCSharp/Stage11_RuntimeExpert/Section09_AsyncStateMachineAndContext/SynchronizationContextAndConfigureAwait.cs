// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第9部分-async状态机与上下文.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section09_AsyncStateMachineAndContext
// Item     : SynchronizationContextAndConfigureAwait
// Topic id : stage11/section09/synchronization_context_and_configureawait
//
// Lesson: SyncContext posts continuations (UI); ConfigureAwait(false) skips capture.

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
        await DemoConfigureAwaitAsync();
        DemoGuidance();
        return 0;
    }

    private static void DemoCurrent()
    {
        Console.WriteLine("-- current SynchronizationContext --");
        SynchronizationContext? ctx = SynchronizationContext.Current;
        Console.WriteLine($"  SynchronizationContext.Current={(ctx is null ? "null (typical console/ASP.NET Core)" : ctx.GetType().FullName)}");
        Console.WriteLine("  UI frameworks install a context that posts back to UI thread.");
    }

    private static async Task DemoConfigureAwaitAsync()
    {
        Console.WriteLine("-- ConfigureAwait --");
        await Task.Delay(1).ConfigureAwait(true);
        Console.WriteLine("  ConfigureAwait(true): capture SyncContext/EC (default await)");
        await Task.Delay(1).ConfigureAwait(false);
        Console.WriteLine("  ConfigureAwait(false): continue on thread-pool; library default advice");
        int tid = Environment.CurrentManagedThreadId;
        await Task.Yield();
        Console.WriteLine($"  after Yield, thread id={Environment.CurrentManagedThreadId} (was {tid})");
        Debug.Assert(true);
    }

    private static void DemoGuidance()
    {
        Console.WriteLine("-- guidance --");
        Console.WriteLine("  App code (UI): often default await to return to UI context.");
        Console.WriteLine("  Libraries: ConfigureAwait(false) to avoid forcing caller context.");
        Console.WriteLine("  ASP.NET Core: no SyncContext — deadlocks of classic ASP.NET are gone.");
        Console.WriteLine("  Still avoid sync-over-async (.Result/Wait) on request threads.");
    }
}
