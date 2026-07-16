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
        DemoTradeoffs();
        DemoRuntimeCapabilities();
        DemoHybrid();
        return 0;
    }

    private static void DemoTradeoffs()
    {
        Console.WriteLine("-- JIT vs AOT tradeoffs --");
        Console.WriteLine("  JIT: CPU-specific codegen, Dynamic PGO, full reflection flexibility");
        Console.WriteLine("  AOT: fast startup, smaller working set options, no runtime JIT cost");
        Console.WriteLine("  AOT cost: trim/root analysis, limited dynamic code, longer publish");
        long t0 = Stopwatch.GetTimestamp();
        int s = 0;
        for (int i = 0; i < 100_000; i++)
            s += i;
        long us = Stopwatch.GetElapsedTime(t0).Microseconds;
        Debug.Assert(s > 0);
        Console.WriteLine($"  micro loop sum={s}, ~{us}µs (already JIT-compiled after first call)");
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
    }

    private static void DemoHybrid()
    {
        Console.WriteLine("-- hybrid world --");
        Console.WriteLine("  R2R: pre-JIT images for faster startup, still re-JIT with tiering/PGO");
        Console.WriteLine("  NativeAOT: full AOT, no IL JIT");
        Console.WriteLine("  Choose per app: servers often JIT+PGO; mobile/CLI tools often AOT-friendly");
    }
}
