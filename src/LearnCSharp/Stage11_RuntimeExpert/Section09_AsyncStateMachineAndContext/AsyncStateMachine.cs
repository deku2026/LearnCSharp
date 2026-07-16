// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第9部分-async状态机与上下文.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section09_AsyncStateMachineAndContext
// Item     : AsyncStateMachine
// Topic id : stage11/section09/async_state_machine
//
// Lesson: async methods lower to stub + IAsyncStateMachine with lifted locals/state.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section09;

internal static class AsyncStateMachine
{
    [LearnTopic("stage11/section09/async_state_machine")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AsyncStateMachine ===");
        return RunAsync().GetAwaiter().GetResult();
    }

    private static async Task<int> RunAsync()
    {
        DemoExplain();
        await DemoLiftedLocalsAsync();
        DemoAttribute();
        return 0;
    }

    private static void DemoExplain()
    {
        Console.WriteLine("-- compiler transform --");
        Console.WriteLine("  stub method creates state machine, builder.Start → MoveNext");
        Console.WriteLine("  fields: <>1__state, builder, lifted locals/params, awaiters");
        Console.WriteLine("  Release: often struct SM; Debug: class SM");
    }

    private static async Task DemoLiftedLocalsAsync()
    {
        Console.WriteLine("-- locals live across await as fields --");
        int local = 10;
        string tag = "pre";
        await Task.Yield(); // forces incomplete await path often
        tag = "post";
        local += 5;
        Debug.Assert(local == 15 && tag == "post");
        Console.WriteLine($"  after await: local={local}, tag={tag}");
        Console.WriteLine("  Without lifting, stack frame would be gone after suspension.");
    }

    private static void DemoAttribute()
    {
        Console.WriteLine("-- AsyncStateMachineAttribute points at generated type --");
        var m = typeof(AsyncStateMachine).GetMethod(nameof(SampleAsync), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Debug.Assert(m is not null);
        var attr = m.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false)
            .OfType<AsyncStateMachineAttribute>()
            .FirstOrDefault();
        Console.WriteLine($"  SampleAsync AsyncStateMachineAttribute StateMachineType={attr?.StateMachineType.Name}");
        Debug.Assert(attr is not null);
    }

    private static async Task<int> SampleAsync()
    {
        await Task.CompletedTask;
        return 1;
    }
}
