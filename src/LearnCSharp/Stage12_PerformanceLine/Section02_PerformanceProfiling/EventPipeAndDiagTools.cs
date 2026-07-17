// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第2部分-性能分析与诊断.md
// Stage    : Stage12_PerformanceLine
// Section  : Section02_PerformanceProfiling
// Item     : EventPipeAndDiagTools
// Topic id : stage12/section02/eventpipe_and_diag_tools
//
// Lesson: EventPipe vs ETW/perf; diagnostic tool map; custom EventSource + EventListener.

using System.Diagnostics;
using System.Diagnostics.Tracing;
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
        DemoEventSourceListener();
        DemoProcessSnapshot();
        return 0;
    }

    private static void DemoEventPipeVsOs()
    {
        Console.WriteLine("-- EventPipe vs ETW/perf --");
        Console.WriteLine("  EventPipe: in-process, cross-platform, no admin, managed-aware.");
        Console.WriteLine("  ETW/perf: OS-level, native+kernel, often needs elevation.");
        Console.WriteLine("  Rule: managed hotspots → EventPipe; P/Invoke/native → ETW/perf.");
    }

    private static void DemoToolMap()
    {
        Console.WriteLine("-- diagnostic tool map --");
        Console.WriteLine("  dotnet-counters / dotnet-trace / dotnet-gcdump / dotnet-dump / dotnet-monitor");
        Console.WriteLine("  Flow: counters anomaly → trace/gcdump while hot → dump if crash/hang.");
    }

    private static void DemoEventSourceListener()
    {
        Console.WriteLine("-- custom EventSource + EventListener (real EventPipe-family API) --");
        using DiagCountingListener listener = new DiagCountingListener();
        // Enable via OnEventSourceCreated and explicit call
        listener.EnableEvents(DiagLabEventSource.Log, EventLevel.Verbose);

        DiagLabEventSource.Log.WorkItem("parse", 3);
        DiagLabEventSource.Log.WorkItem("serialize", 7);
        DiagLabEventSource.Log.WorkItem("parse", 2);
        DiagLabEventSource.Log.Heartbeat(42);

        Debug.Assert(listener.WorkItemCount == 3);
        Debug.Assert(listener.HeartbeatCount == 1);
        Debug.Assert(listener.PayloadSum == 3 + 7 + 2);
        Console.WriteLine($"  WorkItem events={listener.WorkItemCount}, payload sum={listener.PayloadSum}");
        Console.WriteLine($"  Heartbeat events={listener.HeartbeatCount}, last value={listener.LastHeartbeat}");
        Console.WriteLine("  Same EventSource appears in dotnet-trace / EventPipe sessions by name.");
    }

    private static void DemoProcessSnapshot()
    {
        Console.WriteLine("-- local process snapshot --");
        Process p = Process.GetCurrentProcess();
        Console.WriteLine($"  PID={p.Id} WorkingSet={p.WorkingSet64 / 1024.0 / 1024.0:F1} MB");
        Console.WriteLine($"  Threads={p.Threads.Count} GC heap≈{GC.GetTotalMemory(false) / 1024.0:F1} KB");
        Debug.Assert(p.Id > 0);
    }

    [EventSource(Name = "LearnCSharp-DiagLab")]
    private sealed class DiagLabEventSource : EventSource
    {
        public static readonly DiagLabEventSource Log = new();

        [Event(1, Level = EventLevel.Informational)]
        public void WorkItem(string name, int weight) => WriteEvent(1, name, weight);

        [Event(2, Level = EventLevel.Verbose)]
        public void Heartbeat(int value) => WriteEvent(2, value);
    }

    private sealed class DiagCountingListener : EventListener
    {
        public int WorkItemCount { get; private set; }
        public int HeartbeatCount { get; private set; }
        public int PayloadSum { get; private set; }
        public int LastHeartbeat { get; private set; }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name == "LearnCSharp-DiagLab")
                EnableEvents(eventSource, EventLevel.Verbose);
        }

        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            if (e.EventName == "WorkItem")
            {
                WorkItemCount++;
                if (e.Payload is { Count: >= 2 } && e.Payload[1] is int w)
                    PayloadSum += w;
            }
            else if (e.EventName == "Heartbeat")
            {
                HeartbeatCount++;
                if (e.Payload is { Count: >= 1 } && e.Payload[0] is int v)
                    LastHeartbeat = v;
            }
        }
    }
}
