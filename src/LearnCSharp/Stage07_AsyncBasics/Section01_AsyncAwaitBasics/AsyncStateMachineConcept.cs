// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第1部分-async-await基础.md
// Stage    : Stage07_AsyncBasics
// Section  : Section01_AsyncAwaitBasics
// Item     : AsyncStateMachineConcept
// Topic id : stage07/section01/async_state_machine_concept
//
// 步骤 4：编译器把 async 变成状态机；同步完成走快路径；概念对齐迭代器

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section01;

internal static class AsyncStateMachineConcept
{
    [LearnTopic("stage07/section01/async_state_machine_concept")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AsyncStateMachineConcept ===");
        DemoMultiAwaitStateMachine().GetAwaiter().GetResult();
        DemoSynchronousCompletionFastPath().GetAwaiter().GetResult();
        DemoLocalsSurviveAcrossAwaits().GetAwaiter().GetResult();
        DemoIteratorAnalogy();
        return 0;
    }

    private static async Task DemoMultiAwaitStateMachine()
    {
        Console.WriteLine("-- multi-await method ≈ state machine with states per await --");
        int sum = await ExampleAsync();
        Debug.Assert(sum == 30);
        Console.WriteLine($"  ExampleAsync: await GetX + await GetY → {sum}");
        Console.WriteLine("  compiler emits IAsyncStateMachine + MoveNext + state field (see SharpLab)");
    }

    private static async Task DemoSynchronousCompletionFastPath()
    {
        Console.WriteLine("-- already-completed await: fast path, no real pause --");
        Task<int> ready = Task.FromResult(7);
        Debug.Assert(ready.IsCompleted);
        int v = await ready;
        Debug.Assert(v == 7);

        // Completed task: await does not schedule a true async resume in the slow path sense.
        int w = await CompletedValueAsync(9);
        Debug.Assert(w == 9);
        Console.WriteLine("  sync-complete awaits stay cheap (builder fast path; often no heap box)");
    }

    private static async Task DemoLocalsSurviveAcrossAwaits()
    {
        Console.WriteLine("-- locals become state-machine fields across pause/resume --");
        string label = "local";
        int n = 1;
        await Task.Delay(5);
        n += 10;
        await Task.Delay(5);
        string message = $"{label}:{n}";
        Debug.Assert(message == "local:11");
        Console.WriteLine($"  survived across awaits: {message}");
    }

    private static void DemoIteratorAnalogy()
    {
        Console.WriteLine("-- same idea as iterator state machines (Stage 5) --");
        IEnumerable<int> Seq()
        {
            yield return 1;
            yield return 2;
        }

        Debug.Assert(Seq().Sum() == 3);
        Console.WriteLine("  iterators: state + fieldized locals + MoveNext");
        Console.WriteLine("  async:      state + fieldized locals + MoveNext (IAsyncStateMachine)");
        Console.WriteLine("  C++20 coroutines: co_await / co_yield share the pause-resume frame idea");
    }

    private static async Task<int> ExampleAsync()
    {
        int x = await GetXAsync();
        int y = await GetYAsync();
        return x + y;
    }

    private static async Task<int> GetXAsync()
    {
        await Task.Delay(5);
        return 10;
    }

    private static async Task<int> GetYAsync()
    {
        await Task.Delay(5);
        return 20;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Task<int> CompletedValueAsync(int value) => Task.FromResult(value);
}
