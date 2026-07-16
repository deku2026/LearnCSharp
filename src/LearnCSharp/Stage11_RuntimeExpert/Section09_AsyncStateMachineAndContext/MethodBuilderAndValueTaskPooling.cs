// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第9部分-async状态机与上下文.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section09_AsyncStateMachineAndContext
// Item     : MethodBuilderAndValueTaskPooling
// Topic id : stage11/section09/method_builder_and_valuetask_pooling
//
// Lesson: AsyncTaskMethodBuilder creates Task; ValueTask can avoid alloc on sync complete.

using System.Diagnostics;
using System.Threading.Tasks.Sources;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section09;

internal static class MethodBuilderAndValueTaskPooling
{
    [LearnTopic("stage11/section09/method_builder_and_valuetask_pooling")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MethodBuilderAndValueTaskPooling ===");
        return RunAsync().GetAwaiter().GetResult();
    }

    private static async Task<int> RunAsync()
    {
        DemoBuilders();
        await DemoValueTaskSyncAsync();
        DemoPoolingNote();
        return 0;
    }

    private static void DemoBuilders()
    {
        Console.WriteLine("-- method builders --");
        Console.WriteLine("  AsyncTaskMethodBuilder / AsyncTaskMethodBuilder<T>");
        Console.WriteLine("  AsyncValueTaskMethodBuilder / pooling variants");
        Console.WriteLine("  AsyncVoidMethodBuilder (events only — avoid elsewhere)");
        Task<int> t = Task.FromResult(1);
        Debug.Assert(t.IsCompletedSuccessfully && t.Result == 1);
        Console.WriteLine($"  sample Task.FromResult(1) status={t.Status}");
    }

    private static async Task DemoValueTaskSyncAsync()
    {
        Console.WriteLine("-- ValueTask for sync completion --");
        ValueTask<int> vt = ReadCachedAsync(42);
        Debug.Assert(vt.IsCompletedSuccessfully);
        int v = await vt;
        Debug.Assert(v == 42);
        Console.WriteLine($"  ValueTask completed synchronously: {v}");
        Console.WriteLine("  Do not await same ValueTask twice; do not use after GetResult.");
    }

    private static void DemoPoolingNote()
    {
        Console.WriteLine("-- pooling --");
        Console.WriteLine("  IValueTaskSource enables pooled async ops (pipes, sockets).");
        Console.WriteLine("  [AsyncMethodBuilder(...)] can select custom builders.");
        Console.WriteLine($"  IValueTaskSource is loaded: {typeof(IValueTaskSource).FullName}");
        Debug.Assert(typeof(IValueTaskSource).IsInterface);
    }

    private static ValueTask<int> ReadCachedAsync(int value) => new(value);
}
