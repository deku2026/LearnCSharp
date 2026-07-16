// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第2部分-性能分析与诊断.md
// Stage    : Stage12_PerformanceLine
// Section  : Section02_PerformanceProfiling
// Item     : EventPipeAndDiagTools
// Topic id : stage12/section02/eventpipe_and_diag_tools
//
// Lesson: EventPipe vs ETW/perf; diagnostic tool map (no external tools required at runtime).

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section02;

internal static class EventPipeAndDiagTools
{
    [LearnTopic("stage12/section02/eventpipe_and_diag_tools")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== EventPipeAndDiagTools ===");
        DemoEventPipeVsOs();
        DemoToolMap();
        DemoProcessSnapshot();
        return 0;
    }

    private static void DemoEventPipeVsOs()
    {
        Console.WriteLine("-- EventPipe vs ETW/perf --");
        Console.WriteLine("  EventPipe: in-process, cross-platform, no admin, managed-aware stacks.");
        Console.WriteLine("    Output: .nettrace; IPC via diagnostic port.");
        Console.WriteLine("    Limit: managed + runtime events; native/kernel frames incomplete.");
        Console.WriteLine("  ETW (Windows) / perf (Linux): OS-level, native+kernel, usually needs elevation.");
        Console.WriteLine("  Rule: managed hotspots → EventPipe tools; P/Invoke/native → ETW/perf.");
    }

    private static void DemoToolMap()
    {
        Console.WriteLine("-- diagnostic tool map --");
        Console.WriteLine("  dotnet-counters : live metrics (CPU, GC, alloc rate, threadpool)");
        Console.WriteLine("  dotnet-trace    : collect CPU/GC/events → .nettrace → flame graphs");
        Console.WriteLine("  dotnet-gcdump   : managed heap snapshot (leaks / retention paths)");
        Console.WriteLine("  dotnet-dump     : full dump + SOS for post-mortem");
        Console.WriteLine("  dotnet-stack    : all managed stacks now");
        Console.WriteLine("  dotnet-monitor  : always-on collection sidecar for production");
        Console.WriteLine("  Flow: counters anomaly → trace/gcdump while hot → dump if crash/hang.");
    }

    private static void DemoProcessSnapshot()
    {
        Console.WriteLine("-- local process snapshot (no external tool) --");
        Process p = Process.GetCurrentProcess();
        Console.WriteLine($"  PID={p.Id} Name={p.ProcessName}");
        Console.WriteLine($"  WorkingSet={p.WorkingSet64 / 1024.0 / 1024.0:F1} MB");
        Console.WriteLine($"  Threads={p.Threads.Count} GC heap≈{GC.GetTotalMemory(false) / 1024.0:F1} KB");
        Console.WriteLine("  Install tools: dotnet tool install -g dotnet-counters / dotnet-trace / ...");
        Debug.Assert(p.Id > 0);
    }
}
