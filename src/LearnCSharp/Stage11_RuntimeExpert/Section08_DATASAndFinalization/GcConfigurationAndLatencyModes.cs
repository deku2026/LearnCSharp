// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第8部分-DATAS与终结化调优.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section08_DATASAndFinalization
// Item     : GcConfigurationAndLatencyModes
// Topic id : stage11/section08/gc_configuration_and_latency_modes
//
// Lesson: GCLatencyMode, NoGC region, config knobs for latency-sensitive code.

using System.Diagnostics;
using System.Runtime;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section08;

internal static class GcConfigurationAndLatencyModes
{
    [LearnTopic("stage11/section08/gc_configuration_and_latency_modes")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GcConfigurationAndLatencyModes ===");
        DemoLatencyModes();
        DemoNoGcRegion();
        DemoConfigSurface();
        return 0;
    }

    private static void DemoLatencyModes()
    {
        Console.WriteLine("-- GCLatencyMode --");
        GCLatencyMode original = GCSettings.LatencyMode;
        Console.WriteLine($"  original={original}");
        try
        {
            GCSettings.LatencyMode = GCLatencyMode.LowLatency;
            Console.WriteLine($"  set LowLatency → {GCSettings.LatencyMode}");
            Debug.Assert(GCSettings.LatencyMode == GCLatencyMode.LowLatency);
            // do minimal work
            _ = new byte[128];
        }
        finally
        {
            GCSettings.LatencyMode = original;
        }

        Console.WriteLine($"  restored={GCSettings.LatencyMode}");
        Console.WriteLine("  Modes: Interactive, Batch, LowLatency, SustainedLowLatency");
    }

    private static void DemoNoGcRegion()
    {
        Console.WriteLine("-- TryStartNoGCRegion --");
        // Small region request; may fail if memory tight — handle both outcomes
        bool started = false;
        try
        {
            started = GC.TryStartNoGCRegion(1_000_000);
            Console.WriteLine($"  TryStartNoGCRegion(1MB)={started}");
            if (started)
            {
                byte[] buf = new byte[4096];
                Debug.Assert(buf.Length == 4096);
            }
        }
        finally
        {
            if (started)
            {
                try { GC.EndNoGCRegion(); }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"  EndNoGCRegion: {ex.Message}");
                }
            }
        }
    }

    private static void DemoConfigSurface()
    {
        Console.WriteLine("-- config surface --");
        Console.WriteLine("  runtimeconfig.json configProperties / DOTNET_gc* env vars");
        Console.WriteLine("  Heap count, server/workstation, concurrent, DATAS, LOH threshold");
        Console.WriteLine($"  IsServerGC={GCSettings.IsServerGC}, LatencyMode={GCSettings.LatencyMode}");
        Debug.Assert(GC.MaxGeneration >= 2);
    }
}
