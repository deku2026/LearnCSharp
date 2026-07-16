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
        DemoSuppressFlow();
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

    private static void DemoSuppressFlow()
    {
        Console.WriteLine("-- SuppressFlow (advanced) --");
        s_traceId.Value = "parent";
        using (ExecutionContext.SuppressFlow())
        {
            // new async work started here may not flow ambient context
            Console.WriteLine($"  suppressed; current AsyncLocal still readable on this thread={s_traceId.Value}");
            Debug.Assert(s_traceId.Value == "parent");
        }

        Console.WriteLine("  Use rarely — libraries/hosting usually manage context correctly.");
        s_traceId.Value = null;
    }
}
