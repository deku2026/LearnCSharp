// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第7部分-GC原理与堆结构.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section07_GCPrinciplesAndHeap
// Item     : WorkstationVsServerAndBackground
// Topic id : stage11/section07/workstation_vs_server_and_background
//
// Lesson: workstation vs server GC heaps/threads; background GC reduces long pauses.

using System.Diagnostics;
using System.Runtime;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section07;

internal static class WorkstationVsServerAndBackground
{
    [LearnTopic("stage11/section07/workstation_vs_server_and_background")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WorkstationVsServerAndBackground ===");
        DemoModes();
        DemoBackground();
        DemoConfig();
        return 0;
    }

    private static void DemoModes()
    {
        Console.WriteLine("-- workstation vs server --");
        Console.WriteLine($"  GCSettings.IsServerGC={GCSettings.IsServerGC}");
        Console.WriteLine("  Workstation: one heap, lower memory, UI-friendly");
        Console.WriteLine("  Server: heap per CPU, higher throughput, more memory");
        Console.WriteLine($"  LatencyMode={GCSettings.LatencyMode}");
        Console.WriteLine($"  IsServerGC process bitness={Environment.Is64BitProcess}");
        Debug.Assert(Enum.IsDefined(GCSettings.LatencyMode));
    }

    private static void DemoBackground()
    {
        Console.WriteLine("-- background GC --");
        Console.WriteLine("  Concurrent mark of Gen2 while app threads mostly run.");
        Console.WriteLine("  Still brief suspends; not hard real-time.");
        int g2 = GC.CollectionCount(2);
        // light allocation
        for (int i = 0; i < 1000; i++)
            _ = new byte[256];
        Console.WriteLine($"  Gen2 collections so far={GC.CollectionCount(2)} (was {g2})");
        Debug.Assert(GC.CollectionCount(2) >= g2);
    }

    private static void DemoConfig()
    {
        Console.WriteLine("-- configuration --");
        Console.WriteLine("  runtimeconfig: System.GC.Server, System.GC.Concurrent");
        Console.WriteLine("  env: DOTNET_gcServer, DOTNET_gcConcurrent");
        Console.WriteLine("  DATAS (next section) adapts server heap size to live data.");
        GCMemoryInfo info = GC.GetGCMemoryInfo();
        Console.WriteLine($"  HeapSizeBytes={info.HeapSizeBytes}, HighMemoryLoadThresholdBytes={info.HighMemoryLoadThresholdBytes}");
    }
}
