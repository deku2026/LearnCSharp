// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第7部分-GC原理与堆结构.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section07_GCPrinciplesAndHeap
// Item     : GenerationalHypothesis
// Topic id : stage11/section07/generational_hypothesis
//
// Lesson: most objects die young → Gen0 frequent cheap collections.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section07;

internal static class GenerationalHypothesis
{
    [LearnTopic("stage11/section07/generational_hypothesis")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GenerationalHypothesis ===");
        DemoGens();
        DemoYoungDeath();
        DemoPromotion();
        return 0;
    }

    private static void DemoGens()
    {
        Console.WriteLine("-- generations --");
        Console.WriteLine($"  MaxGeneration={GC.MaxGeneration} (0,1,2)");
        Debug.Assert(GC.MaxGeneration >= 2);
        object o = new byte[64];
        Console.WriteLine($"  new small object generation={GC.GetGeneration(o)} (usually 0)");
        Debug.Assert(GC.GetGeneration(o) >= 0);
    }

    private static void DemoYoungDeath()
    {
        Console.WriteLine("-- weak generational hypothesis: die young --");
        int g0Before = GC.CollectionCount(0);
        for (int i = 0; i < 50_000; i++)
        {
            byte[] tmp = new byte[128]; // short-lived
            _ = tmp.Length;
        }

        int g0After = GC.CollectionCount(0);
        Console.WriteLine($"  Gen0 collections during churn: {g0After - g0Before} (may be 0 if heap large)");
        Console.WriteLine("  Ephemeral GC scans a small region; full Gen2 is rarer/expensive.");
    }

    private static void DemoPromotion()
    {
        Console.WriteLine("-- survival promotes --");
        object longLived = new byte[256];
        int g1 = GC.GetGeneration(longLived);
        GC.Collect(0);
        int g2 = GC.GetGeneration(longLived);
        Console.WriteLine($"  generation before Gen0 collect={g1}, after={g2}");
        Debug.Assert(g2 >= g1);
        // keep longLived rooted
        GC.KeepAlive(longLived);
    }
}
