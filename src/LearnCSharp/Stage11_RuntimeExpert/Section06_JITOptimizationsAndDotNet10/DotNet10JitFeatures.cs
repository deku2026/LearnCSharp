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
        var info = GC.GetGCMemoryInfo();
        Console.WriteLine($"  HeapSizeBytes={info.HeapSizeBytes}, Generation={GC.MaxGeneration}");
        Console.WriteLine($"  IsServerGC={System.Runtime.GCSettings.IsServerGC}");
        Console.WriteLine($"  RuntimeFeature.IsDynamicCodeCompiled={System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled}");
        Debug.Assert(GC.MaxGeneration >= 2);
        // tiny workload so output is grounded
        int n = Enumerable.Range(0, 1000).Sum();
        Debug.Assert(n == 499_500);
        Console.WriteLine($"  Enumerable.Sum 0..999={n}");
    }
}
