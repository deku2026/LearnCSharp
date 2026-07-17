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
        Console.WriteLine("-- background GC + observe a real Gen2 collection --");
        Console.WriteLine("  Concurrent mark of Gen2 while app threads mostly run; brief suspends, not hard real-time.");
        GCMemoryInfo info = GC.GetGCMemoryInfo();
        Console.WriteLine($"  HeapSizeBytes={info.HeapSizeBytes}, Committed={info.TotalCommittedBytes}, PinnedObjects={info.PinnedObjectsCount}");
        int g2Before = GC.CollectionCount(2);
        // Allocate enough large arrays (each > 85KB → LOH → eventually Gen2) to force a Gen2 collection.
        List<byte[]> keep = new List<byte[]>(256);
        for (int i = 0; i < 256; i++)
            keep.Add(new byte[1 << 20]); // 1 MiB each → LOH, promoted to Gen2
        GC.Collect(2);
        GC.WaitForPendingFinalizers();
        GC.Collect(2);
        int g2After = GC.CollectionCount(2);
        Console.WriteLine($"  Gen2 collections before={g2Before}, after={g2After}");
        Debug.Assert(g2After > g2Before, "a Gen2 collection must have occurred after large allocations + GC.Collect(2)");
        keep.Clear();
        GC.Collect(2);
        Console.WriteLine($"  IsServerGC={GCSettings.IsServerGC} → {(GCSettings.IsServerGC ? "server: heap per CPU" : "workstation: single heap")}");
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
