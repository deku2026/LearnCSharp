// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第2部分-性能分析与诊断.md
// Stage    : Stage12_PerformanceLine
// Section  : Section02_PerformanceProfiling
// Item     : CpuProfilingDotnetTrace
// Topic id : stage12/section02/cpu_profiling_dotnet_trace
//
// Lesson: dotnet-trace cpu-sampling → flame graph; wide frames = hot.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section02;

internal static class CpuProfilingDotnetTrace
{
    [LearnTopic("stage12/section02/cpu_profiling_dotnet_trace")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CpuProfilingDotnetTrace ===");
        DemoCommands();
        DemoFlameGraphReading();
        DemoCpuBurnForPractice();
        return 0;
    }

    private static void DemoCommands()
    {
        Console.WriteLine("-- collect CPU sampling (run these yourself) --");
        Console.WriteLine("  dotnet-trace collect --process-id <pid>");
        Console.WriteLine("  dotnet-trace collect --name LearnCSharp");
        Console.WriteLine("  # providers e.g. Microsoft-DotNETCore-SampleProfiler + DotNETRuntime");
        Console.WriteLine("  dotnet-trace convert trace.nettrace --format speedscope");
        Console.WriteLine("  # open trace.speedscope.json at https://www.speedscope.app/");
        Console.WriteLine("  Alternatives: VS CPU Usage, PerfView, JetBrains dotTrace.");
    }

    private static void DemoFlameGraphReading()
    {
        Console.WriteLine("-- flame graph reading --");
        Console.WriteLine("  X-axis width ∝ time in that stack frame (and callees).");
        Console.WriteLine("  Y-axis = stack depth (root at bottom or top depending on tool).");
        Console.WriteLine("  Wide flat tops are hotspots → BDN + optimize those methods.");
        Console.WriteLine("  dotnet-trace resolves managed frames via JIT maps; native needs ETW/perf.");
    }

    private static void DemoCpuBurnForPractice()
    {
        Console.WriteLine("-- short CPU burn (attach profiler here in real labs) --");
        Stopwatch sw = Stopwatch.StartNew();
        double sink = 0;
        for (int i = 0; i < 200_000; i++)
            sink += Math.Sin(i) * Math.Cos(i * 0.5);
        sw.Stop();
        Debug.Assert(!double.IsNaN(sink));
        Console.WriteLine($"  burn≈{sw.Elapsed.TotalMilliseconds:F2} ms sink={sink:F3}");
        Console.WriteLine("  In a lab: collect during burn, open flame graph, find this method.");
    }
}
