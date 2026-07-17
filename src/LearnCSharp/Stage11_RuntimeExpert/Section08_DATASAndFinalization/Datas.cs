// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第8部分-DATAS与终结化调优.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section08_DATASAndFinalization
// Item     : Datas
// Topic id : stage11/section08/datas
//
// Lesson: DATAS adapts heap to live data size — observe via GC.GetGCMemoryInfo.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section08;

internal static class Datas
{
    [LearnTopic("stage11/section08/datas")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Datas ===");
        DemoExplain();
        DemoObserveHeap();
        DemoConfig();
        return 0;
    }

    private static void DemoExplain()
    {
        Console.WriteLine("-- DATAS: Dynamic Adaptation To Application Sizes --");
        Console.WriteLine("  Server GC historically sized heaps aggressively (per CPU).");
        Console.WriteLine("  DATAS targets live data size (LDS): grow on burst, shrink when idle.");
        Console.WriteLine("  .NET 8 opt-in; .NET 9+ server GC default on.");
    }

    private static void DemoObserveHeap()
    {
        Console.WriteLine("-- observe heap via managed APIs (no dotnet-counters required) --");
        GCMemoryInfo before = GC.GetGCMemoryInfo();
        Console.WriteLine($"  before HeapSize={before.HeapSizeBytes}, Committed={before.TotalCommittedBytes}");
        List<byte[]> keep = new List<byte[]>(200);
        for (int i = 0; i < 200; i++)
            keep.Add(new byte[50_000]);
        GCMemoryInfo mid = GC.GetGCMemoryInfo();
        Console.WriteLine($"  after alloc HeapSize={mid.HeapSizeBytes}, Committed={mid.TotalCommittedBytes}");
        keep.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GCMemoryInfo after = GC.GetGCMemoryInfo();
        Console.WriteLine($"  after release+GC HeapSize={after.HeapSizeBytes}, Committed={after.TotalCommittedBytes}");
        // "grow on burst": after 200×50KB allocations the managed heap must have grown.
        // (The shrink half is observational only — DATAS shrink needs sustained idle + server GC.)
        Debug.Assert(mid.HeapSizeBytes > before.HeapSizeBytes, "DATAS: heap must grow under allocation burst");
        Console.WriteLine($"  grow-on-burst ΔHeap={mid.HeapSizeBytes - before.HeapSizeBytes} bytes (asserted > 0)");
    }

    private static void DemoConfig()
    {
        Console.WriteLine("-- config --");
        Console.WriteLine("  System.GC.DynamicAdaptationMode=1 (on) / 0 (off)");
        Console.WriteLine("  DOTNET_GCDynamicAdaptationMode=1");
        Console.WriteLine($"  IsServerGC={System.Runtime.GCSettings.IsServerGC}");
        string? mode = Environment.GetEnvironmentVariable("DOTNET_GCDynamicAdaptationMode");
        Console.WriteLine($"  env DOTNET_GCDynamicAdaptationMode={mode ?? "(unset)"}");
    }
}
