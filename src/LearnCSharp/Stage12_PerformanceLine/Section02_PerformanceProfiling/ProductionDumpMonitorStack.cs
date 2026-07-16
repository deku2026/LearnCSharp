// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第2部分-性能分析与诊断.md
// Stage    : Stage12_PerformanceLine
// Section  : Section02_PerformanceProfiling
// Item     : ProductionDumpMonitorStack
// Topic id : stage12/section02/production_dump_monitor_stack
//
// Lesson: dump/monitor/stack for hangs, leaks, high CPU, frequent GC in production.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section02;

internal static class ProductionDumpMonitorStack
{
    [LearnTopic("stage12/section02/production_dump_monitor_stack")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ProductionDumpMonitorStack ===");
        DemoCommands();
        DemoScenarios();
        DemoLiveStacks();
        return 0;
    }

    private static void DemoCommands()
    {
        Console.WriteLine("-- production diagnostic commands --");
        Console.WriteLine("  dotnet-stack report -p <pid>          // all managed stacks now");
        Console.WriteLine("  dotnet-dump collect -p <pid>          // full dump");
        Console.WriteLine("  dotnet-dump analyze <dump>            // SOS: clrstack, dumpheap, gcroot");
        Console.WriteLine("  dotnet-monitor                         // HTTP API for dump/trace/metrics");
        Console.WriteLine("  Prefer non-invasive counters/trace first; dumps when stuck/crash.");
    }

    private static void DemoScenarios()
    {
        Console.WriteLine("-- when to use what --");
        Console.WriteLine("  High CPU:     trace sampling → flame graph");
        Console.WriteLine("  Memory leak:  gcdump pair + retention paths");
        Console.WriteLine("  GC thrash:    counters (alloc-rate, gen counts) + alloc profile");
        Console.WriteLine("  Hang/deadlock:dump + clrstack -a / syncblk; or live stack");
        Console.WriteLine("  Crash:        dump + SOS post-mortem");
        Console.WriteLine("  Always-on:    dotnet-monitor / OpenTelemetry metrics");
    }

    private static void DemoLiveStacks()
    {
        Console.WriteLine("-- managed stack snapshot (current thread) --");
        StackTrace st = new(fNeedFileInfo: false);
        string[] frames = st.GetFrames()?
            .Select(f => f.GetMethod()?.DeclaringType?.Name + "." + f.GetMethod()?.Name)
            .Where(s => s is not null)
            .Take(8)
            .ToArray() ?? [];
        Debug.Assert(frames.Length > 0);
        foreach (string? frame in frames)
            Console.WriteLine($"  {frame}");
        Console.WriteLine("  Production: capture ALL threads with dotnet-stack / dump.");
    }
}
