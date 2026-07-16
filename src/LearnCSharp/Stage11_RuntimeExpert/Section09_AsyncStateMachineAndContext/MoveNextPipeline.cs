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
}
