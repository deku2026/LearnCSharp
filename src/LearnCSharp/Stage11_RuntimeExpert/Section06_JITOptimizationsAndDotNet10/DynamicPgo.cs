// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第6部分-JIT优化与dotNET10专题.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section06_JITOptimizationsAndDotNet10
// Item     : DynamicPgo
// Topic id : stage11/section06/dynamic_pgo
//
// Lesson: Dynamic PGO collects edge/type profiles at Tier0 to guide Tier1 opts.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section06;

internal static class DynamicPgo
{
    [LearnTopic("stage11/section06/dynamic_pgo")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DynamicPgo ===");
        DemoWhatIsPgo();
        DemoBiasedBranch();
        DemoConfig();
        return 0;
    }

    private static void DemoWhatIsPgo()
    {
        Console.WriteLine("-- Dynamic Profile-Guided Optimization --");
        Console.WriteLine("  Instrumentation records: branch likelihood, virtual targets, block counts.");
        Console.WriteLine("  Tier1 uses profile for better layout, GDV, inlining budget.");
        Console.WriteLine("  Default on recent .NET; DOTNET_TieredPGO=1");
    }

    private static void DemoBiasedBranch()
    {
        Console.WriteLine("-- biased branch (profile should favor true path) --");
        int hits = 0;
        for (int i = 0; i < 20_000; i++)
        {
            if (Likely(i))
                hits++;
        }

        Debug.Assert(hits > 19_000);
        Console.WriteLine($"  Likely() true hits={hits}/20000");
        Console.WriteLine("  JIT can place hot path fall-through after PGO.");
    }

    private static void DemoConfig()
    {
        Console.WriteLine("-- related knobs --");
        Console.WriteLine("  DOTNET_TieredPGO, DOTNET_ReadyToRun, DOTNET_TC_CallCounting");
        string? pgo = Environment.GetEnvironmentVariable("DOTNET_TieredPGO");
        Console.WriteLine($"  DOTNET_TieredPGO={pgo ?? "(default)"}");
        // Keep a hot polymorphic-ish call for documentation
        long sum = 0;
        for (int i = 0; i < 5000; i++)
            sum += Work(i % 2 == 0 ? "a" : "bb");
        Debug.Assert(sum > 0);
        Console.WriteLine($"  Work sum lengths={sum}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool Likely(int i) => i != 12345; // almost always true

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Work(string s) => s.Length;
}
