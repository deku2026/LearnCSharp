// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第2部分-异步进阶与陷阱.md
// Stage    : Stage07_AsyncBasics
// Section  : Section02_AsyncAdvancedAndPitfalls
// Item     : AsyncPitfallsDeadlockVoid
// Topic id : stage07/section02/async_pitfalls_deadlock_void
//
// 步骤 2：sync-over-async 死锁机制、async void、未 await、async all the way
// 控制台无 UI SyncContext，.Result 通常不死锁——用说明 + 安全写法演示

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
        DemoSyncOverAsyncRiskExplained().GetAwaiter().GetResult();
        DemoAsyncAllTheWay().GetAwaiter().GetResult();
        DemoAsyncTaskVsAsyncVoidPattern().GetAwaiter().GetResult();
        DemoUnawaitedTaskObservation().GetAwaiter().GetResult();
        return 0;
    }

    private static async Task DemoSyncOverAsyncRiskExplained()
    {
        Console.WriteLine("-- sync-over-async deadlock (UI / classic ASP.NET) --");
        Console.WriteLine("  1) UI thread blocks on .Result / .Wait()");
        Console.WriteLine("  2) await inside captured UI SynchronizationContext");
        Console.WriteLine("  3) continuation needs UI thread, which is blocked → deadlock");
        Console.WriteLine("  console/ASP.NET Core usually do NOT deadlock this way (no single-thread context)");

        // Safe on console: still avoid in library/app code — prefer await.
        string viaAwait = await FetchAsync();
        Debug.Assert(viaAwait == "data");

        // ConfigureAwait(false) avoids needing the original context (library pattern).
        string viaCfg = await FetchWithConfigureAwaitFalseAsync();
        Debug.Assert(viaCfg == "data");
        Console.WriteLine($"  safe paths: await → {viaAwait}; ConfigureAwait(false) → {viaCfg}");
    }

    private static async Task DemoAsyncAllTheWay()
    {
        Console.WriteLine("-- iron rule: async all the way --");
        string result = await ControllerAsync();
        Debug.Assert(result == "row");
        Console.WriteLine($"  Controller → Service → Repository: {result}");
        Console.WriteLine("  do not sandwich .Result/.Wait() in the middle of an async chain");
    }

    private static async Task DemoAsyncTaskVsAsyncVoidPattern()
    {
        Console.WriteLine("-- async void vs async Task --");
        Exception? fromTask = null;
        try
        {
            await WorkAsTaskAsync();
        }
        catch (InvalidOperationException ex)
        {
            fromTask = ex;
        }

        Debug.Assert(fromTask is not null);
        Console.WriteLine($"  async Task exception catchable: {fromTask.Message}");
        Console.WriteLine("  async void: cannot await, hard to test, exceptions hit SyncContext (crash risk)");
        Console.WriteLine("  only legitimate async void: event handlers (with internal try/catch)");
    }

    private static async Task DemoUnawaitedTaskObservation()
    {
        Console.WriteLine("-- unawaited Task (fire-and-forget) risks --");
        // Observe completion and exceptions explicitly when fire-and-forget is intentional.
        Task orphan = ObservedBackgroundAsync();
        await orphan;
        Debug.Assert(orphan.IsCompletedSuccessfully);
        Console.WriteLine("  always await, or observe exceptions (ContinueWith / wrapper try/catch)");
        Console.WriteLine("  CS4014 warns on discarded async calls");
    }

    private static async Task<string> FetchAsync()
    {
        await Task.Delay(5);
        return "data";
    }

    private static async Task<string> FetchWithConfigureAwaitFalseAsync()
    {
        await Task.Delay(5).ConfigureAwait(false);
        return "data";
    }

    private static async Task<string> ControllerAsync() => await ServiceAsync();
    private static async Task<string> ServiceAsync() => await RepositoryAsync();
    private static async Task<string> RepositoryAsync()
    {
        await Task.Delay(5);
        return "row";
    }

    private static async Task WorkAsTaskAsync()
    {
        await Task.Delay(5);
        throw new InvalidOperationException("boom");
    }

    private static async Task ObservedBackgroundAsync()
    {
        try
        {
            await Task.Delay(5);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  background fault: {ex.Message}");
            throw;
        }
    }
}
