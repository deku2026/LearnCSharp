// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第2部分-性能分析与诊断.md
// Stage    : Stage12_PerformanceLine
// Section  : Section02_PerformanceProfiling
// Item     : CpuProfilingDotnetTrace
// Topic id : stage12/section02/cpu_profiling_dotnet_trace
//
// Lesson: dotnet-trace cpu-sampling → flame graph; wide frames = hot.

using System.Diagnostics;
using System.Diagnostics.Tracing;
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
        DemoCustomEventSourceDuringCpuBurn();
        return 0;
    }

    private static void DemoCommands()
    {
        Console.WriteLine("-- collect CPU sampling (run these yourself) --");
        Console.WriteLine("  dotnet-trace collect --process-id <pid> --providers Microsoft-DotNETCore-SampleProfiler");
        Console.WriteLine("  dotnet-trace convert trace.nettrace --format speedscope");
        Console.WriteLine("  open speedscope.app — wide frames = hot paths");
        Console.WriteLine("  Below: EventSource markers you can also filter in traces.");
    }

    private static void DemoCustomEventSourceDuringCpuBurn()
    {
        Console.WriteLine("-- EventSource markers around a CPU burn (in-process listener) --");
        using CountingListener listener = new CountingListener();
        listener.EnableEvents(CpuLabEventSource.Log, EventLevel.Informational);

        CpuLabEventSource.Log.BurnStart(200_000);
        Stopwatch sw = Stopwatch.StartNew();
        double sink = 0;
        for (int i = 0; i < 200_000; i++)
            sink += Math.Sin(i) * Math.Cos(i * 0.5);
        sw.Stop();
        CpuLabEventSource.Log.BurnStop(sw.Elapsed.TotalMilliseconds, sink);

        Debug.Assert(!double.IsNaN(sink));
        Debug.Assert(listener.BurnStartCount == 1);
        Debug.Assert(listener.BurnStopCount == 1);
        Console.WriteLine($"  burn≈{sw.Elapsed.TotalMilliseconds:F2} ms sink={sink:F3}");
        Console.WriteLine($"  EventSource BurnStart={listener.BurnStartCount}, BurnStop={listener.BurnStopCount}");
        Console.WriteLine("  In a lab: collect during burn, open flame graph, find this method + events.");
    }

    [EventSource(Name = "LearnCSharp-CpuLab")]
    private sealed class CpuLabEventSource : EventSource
    {
        public static readonly CpuLabEventSource Log = new();

        [Event(1, Level = EventLevel.Informational, Message = "BurnStart iterations={0}")]
        public void BurnStart(int iterations) => WriteEvent(1, iterations);

        [Event(2, Level = EventLevel.Informational, Message = "BurnStop ms={0} sink={1}")]
        public void BurnStop(double ms, double sink) => WriteEvent(2, ms, sink);
    }

    private sealed class CountingListener : EventListener
    {
        public int BurnStartCount { get; private set; }
        public int BurnStopCount { get; private set; }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name == "LearnCSharp-CpuLab")
                EnableEvents(eventSource, EventLevel.Informational);
        }

        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            if (e.EventName == "BurnStart") BurnStartCount++;
            else if (e.EventName == "BurnStop") BurnStopCount++;
        }
    }
}
