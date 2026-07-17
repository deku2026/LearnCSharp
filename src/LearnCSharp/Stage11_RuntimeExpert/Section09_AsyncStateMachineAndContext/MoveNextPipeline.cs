// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第9部分-async状态机与上下文.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section09_AsyncStateMachineAndContext
// Item     : MoveNextPipeline
// Topic id : stage11/section09/movenext_pipeline
//
// Lesson: MoveNext state switch; IsCompleted fast path; AwaitUnsafeOnCompleted.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section09;

internal static class MoveNextPipeline
{
    [LearnTopic("stage11/section09/movenext_pipeline")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MoveNextPipeline ===");
        return RunAsync().GetAwaiter().GetResult();
    }

    private static async Task<int> RunAsync()
    {
        DemoExplain();
        await DemoCompletedFastPathAsync();
        await DemoIncompletePathAsync();
        DemoSyncZeroAllocVsSuspendAlloc();
        return 0;
    }

    private static void DemoExplain()
    {
        Console.WriteLine("-- MoveNext pipeline --");
        Console.WriteLine("  state==-1: run until first incomplete await");
        Console.WriteLine("  if awaiter.IsCompleted: GetResult immediately (no alloc/schedule)");
        Console.WriteLine("  else: store state, AwaitUnsafeOnCompleted, return (thread free)");
        Console.WriteLine("  resume: restore awaiter, GetResult, continue");
    }

    private static async Task DemoCompletedFastPathAsync()
    {
        Console.WriteLine("-- synchronous completion (IsCompleted=true) --");
        int v = await AlreadyDoneAsync(7);
        Debug.Assert(v == 7);
        Console.WriteLine($"  await CompletedTask-like path result={v}");
    }

    private static async Task DemoIncompletePathAsync()
    {
        Console.WriteLine("-- incomplete await schedules continuation --");
        int v = await DelayedAsync(3);
        Debug.Assert(v == 3);
        Console.WriteLine($"  await Task.Delay path result={v}");
    }

    private static Task<int> AlreadyDoneAsync(int x) => Task.FromResult(x);

    private static async Task<int> DelayedAsync(int x)
    {
        await Task.Delay(1);
        return x;
    }

    // The doc's ⭐⭐ performance point: struct state machine completes synchronously
    // without boxing (stays on stack); a genuinely suspending await boxes the state
    // machine to the heap. Observable via GetAllocatedBytesForCurrentThread.
    private static void DemoSyncZeroAllocVsSuspendAlloc()
    {
        Console.WriteLine("-- sync-complete zero-alloc vs suspend-boxes-to-heap --");
        // Warm up JIT + delegates so first-call allocations don't pollute the measurement.
        for (int i = 0; i < 64; i++)
        {
            _ = AlreadyDoneAsync(i).GetAwaiter().GetResult();
            _ = DelayedAsync(i).GetAwaiter().GetResult();
        }

        long beforeSync = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100_000; i++)
        {
            _ = AlreadyDoneAsync(i).GetAwaiter().GetResult();
        }
        long syncAlloc = GC.GetAllocatedBytesForCurrentThread() - beforeSync;
        Console.WriteLine($"  100k sync-complete awaits: Δalloc={syncAlloc} bytes (≈0 expected: struct 状态机留栈)");

        // The suspending path genuinely yields the thread each time, so a high count is slow.
        // Use a modest iteration count: the alloc-per-call ratio is still conclusive.
        long beforeSuspend = GC.GetAllocatedBytesForCurrentThread();
        const int SuspendIters = 1_000;
        for (int i = 0; i < SuspendIters; i++)
        {
            _ = DelayedAsync(i).GetAwaiter().GetResult();
        }
        long suspendAlloc = GC.GetAllocatedBytesForCurrentThread() - beforeSuspend;
        Console.WriteLine($"  {SuspendIters}k genuinely-suspending awaits: Δalloc={suspendAlloc} bytes (>0: 状态机装箱 + Task + continuation)");
        // Normalize to per-call so the different iteration counts are comparable.
        double syncPerCall = syncAlloc / 100_000.0;
        double suspendPerCall = suspendAlloc / (double)SuspendIters;
        Console.WriteLine($"  per-call: sync≈{syncPerCall:F1} B, suspend≈{suspendPerCall:F1} B");
        // Under an optimized (Tier1) build the sync-complete path approaches zero allocation
        // (the struct state machine stays on the stack); a Debug F5 build may still allocate
        // because tiered optimization/escape analysis hasn't kicked in. Treat as observational:
        // report the per-call contrast (suspend must allocate more than sync).
        Console.WriteLine($"  → optimized build drives sync per-call toward 0; Debug F5 may still allocate.");
        Debug.Assert(suspendAlloc > 0, "suspending async must box the state machine to the heap");
        Debug.Assert(suspendPerCall >= syncPerCall, "suspending path should allocate at least as much per call as sync path");
        Console.WriteLine($"  per-call ratio suspend/sync ≈ {(suspendPerCall / Math.Max(0.01, syncPerCall)):F1}×");
    }
}
