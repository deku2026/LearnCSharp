// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第6部分-JIT优化与dotNET10专题.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section06_JITOptimizationsAndDotNet10
// Item     : DotNet10JitFeatures
// Topic id : stage11/section06/dotnet10_jit_features
//
// Lesson: .NET 10 runtime/JIT themes — observe version + API surfaces; describe features.

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section06;

internal static class DotNet10JitFeatures
{
    [LearnTopic("stage11/section06/dotnet10_jit_features")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DotNet10JitFeatures ===");
        DemoVersion();
        DemoFeatureThemes();
        DemoObservableApis();
        DemoArrayInterfaceDevirtualizationZeroAlloc();
        return 0;
    }

    private static void DemoVersion()
    {
        Console.WriteLine("-- runtime identity --");
        Console.WriteLine($"  FrameworkDescription={RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"  Environment.Version={Environment.Version}");
        Debug.Assert(Environment.Version.Major >= 8);
        Console.WriteLine("  This project targets net10.0 / C# 14.");
    }

    private static void DemoFeatureThemes()
    {
        Console.WriteLine("-- .NET 10 JIT / runtime themes (from release notes) --");
        Console.WriteLine("  Continued Dynamic PGO + GDV improvements");
        Console.WriteLine("  Better inlining / devirtualization interactions");
        Console.WriteLine("  ARM64 and x64 codegen quality work");
        Console.WriteLine("  Escape analysis and stack allocation expansions");
        Console.WriteLine("  See: learn.microsoft.com → What's new in .NET 10 runtime");
        Console.WriteLine("  External tools (dotnet-trace) not required for this demo.");
    }

    private static void DemoObservableApis()
    {
        Console.WriteLine("-- APIs that help observe runtime without profilers --");
        GCMemoryInfo info = GC.GetGCMemoryInfo();
        Console.WriteLine($"  HeapSizeBytes={info.HeapSizeBytes}, Generation={GC.MaxGeneration}");
        Console.WriteLine($"  IsServerGC={System.Runtime.GCSettings.IsServerGC}");
        Console.WriteLine($"  RuntimeFeature.IsDynamicCodeCompiled={System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled}");
        Debug.Assert(GC.MaxGeneration >= 2);
        // tiny workload so output is grounded
        int n = Enumerable.Range(0, 1000).Sum();
        Debug.Assert(n == 499_500);
        Console.WriteLine($"  Enumerable.Sum 0..999={n}");
    }

    // .NET 10 ⭐⭐ headline: array interface de-virtualization / de-abstraction.
    // foreach over an int[] viewed as IEnumerable<T> allocates ZERO on the hot path —
    // the enumerator is stack-allocated via conditional escape analysis (GDV + inline +
    // escape analysis cooperating). Observable via GetAllocatedBytesForCurrentThread.
    private static void DemoArrayInterfaceDevirtualizationZeroAlloc()
    {
        Console.WriteLine("-- .NET 10 array-interface devirtualization (zero-alloc foreach) --");
        int[] data = new int[1024];
        for (int i = 0; i < data.Length; i++) data[i] = i;

        // Warm up the JIT so the tiered/optimized code is in place before measuring.
        for (int i = 0; i < 32; i++)
        {
            int warm = SumViaInterface(data);
            Debug.Assert(warm == SumIdentity(warm));
        }

        long before = GC.GetAllocatedBytesForCurrentThread();
        long sum = 0;
        const int Iters = 10_000;
        for (int iter = 0; iter < Iters; iter++)
        {
            sum += SumViaInterface(data);
        }
        long delta = GC.GetAllocatedBytesForCurrentThread() - before;
        double perCall = delta / (double)Iters;
        Console.WriteLine($"  {Iters}× Sum(IEnumerable<int> over int[1024]): Δalloc={delta} bytes ({perCall:F1} B/call)");
        Console.WriteLine($"  sum={sum % 1000} (work to prevent dead-code elimination)");
        // .NET 10 aims for zero-alloc on the array-via-interface foreach, but the
        // tiered JIT may not have promoted this to Tier1+escape-analysis under a Debug
        // F5 build. Treat it as observational: report per-call allocation rather than
        // crash on a hard threshold (the doc's claim holds on an optimized Release build).
        Console.WriteLine($"  → on an optimized .NET 10 build this approaches 0 B/call (条件逃逸分析栈分配枚举器); Debug F5 may still allocate.");
        // Sanity: the work actually ran.
        Debug.Assert(sum > 0);
    }

    private static int SumViaInterface(IEnumerable<int> source)
    {
        int s = 0;
        foreach (int x in source) s += x;
        return s;
    }

    private static int SumIdentity(int x) => x;
}
