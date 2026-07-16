// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第5部分-JIT编译与分层编译.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section05_JITAndTieredCompilation
// Item     : WhyJitVsAot
// Topic id : stage11/section05/why_jit_vs_aot
//
// Lesson: JIT adapts to CPU/profile; AOT wins startup/size/self-contained native.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section05;

internal static class WhyJitVsAot
{
    [LearnTopic("stage11/section05/why_jit_vs_aot")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WhyJitVsAot ===");
        DemoTradeoffsWithTiming();
        DemoRuntimeCapabilities();
        DemoHybrid();
        return 0;
    }

    private static void DemoTradeoffsWithTiming()
    {
        Console.WriteLine("-- JIT vs AOT tradeoffs + multi-run microbench --");
        Console.WriteLine("  JIT: CPU-specific codegen, Dynamic PGO, full reflection flexibility");
        Console.WriteLine("  AOT: fast startup, smaller options, no runtime JIT cost");

        // Warmup
        long warm = 0;
        for (int i = 0; i < 20_000; i++)
            warm += Work(i);
        Debug.Assert(warm != 0);

        double[] samples = new double[9];
        long sink = 0;
        for (int s = 0; s < samples.Length; s++)
        {
            long t0 = Stopwatch.GetTimestamp();
            for (int i = 0; i < 100_000; i++)
                sink += Work(i);
            samples[s] = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
        }

        Array.Sort(samples);
        Console.WriteLine($"  Work×100k median={samples[4]:F3}ms min={samples[0]:F3} max={samples[^1]:F3} sink={sink}");
        Debug.Assert(sink != 0);
        Debug.Assert(samples[4] >= 0);
    }

    private static void DemoRuntimeCapabilities()
    {
        Console.WriteLine("-- this process runtime --");
        Console.WriteLine($"  Framework={RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"  OS/Arch={RuntimeInformation.OSDescription} / {RuntimeInformation.ProcessArchitecture}");
        bool dynCode = RuntimeFeature.IsDynamicCodeSupported;
        bool dynCompiled = RuntimeFeature.IsDynamicCodeCompiled;
        Console.WriteLine($"  IsDynamicCodeSupported={dynCode}");
        Console.WriteLine($"  IsDynamicCodeCompiled={dynCompiled}");
        Debug.Assert(RuntimeInformation.FrameworkDescription.Length > 0);
        // On normal CoreCLR JIT host, dynamic code is supported/compiled.
        Debug.Assert(dynCode);
        Console.WriteLine($"  IsReferenceOrContainsReferences<Guid>={RuntimeHelpers.IsReferenceOrContainsReferences<Guid>()}");
        Debug.Assert(!RuntimeHelpers.IsReferenceOrContainsReferences<Guid>());
    }

    private static void DemoHybrid()
    {
        Console.WriteLine("-- hybrid world --");
        Console.WriteLine("  R2R: pre-JIT images for faster startup, still re-JIT with tiering/PGO");
        Console.WriteLine("  NativeAOT: full AOT, no IL JIT (IsDynamicCodeCompiled often false)");
        Console.WriteLine("  Choose per app: servers often JIT+PGO; tools/mobile often AOT-friendly");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Work(int i) => (i * 31) ^ (i + 7);
}
