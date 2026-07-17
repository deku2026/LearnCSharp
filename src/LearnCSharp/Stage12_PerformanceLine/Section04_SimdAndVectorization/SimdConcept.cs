// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第4部分-SIMD与向量化.md
// Stage    : Stage12_PerformanceLine
// Section  : Section04_SimdAndVectorization
// Item     : SimdConcept
// Topic id : stage12/section04/simd_concept
//
// Lesson: SIMD = one instruction, many data elements; needs data-parallel independent work.

using System.Diagnostics;
using System.Numerics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section04;

internal static class SimdConcept
{
    [LearnTopic("stage12/section04/simd_concept")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SimdConcept ===");
        DemoWhatIsSimd();
        DemoWidths();
        DemoWhenItHelps();
        return 0;
    }

    private static void DemoWhatIsSimd()
    {
        Console.WriteLine("-- SIMD: Single Instruction, Multiple Data --");
        Console.WriteLine("  Scalar: one add per instruction on one float.");
        Console.WriteLine("  SIMD:   one add on a vector register holding many floats.");
        Console.WriteLine("  Great for: sum, scale, compare, search, convert on large arrays.");
        Console.WriteLine("  Hard when: each step depends on previous (recurrence).");
        Console.WriteLine("  CPU families: SSE/AVX/AVX-512 (x86), NEON/SVE (ARM).");
    }

    private static void DemoWidths()
    {
        Console.WriteLine("-- vector widths (elements per register) --");
        Console.WriteLine($"  Vector<float>.Count = {Vector<float>.Count} (this machine)");
        Console.WriteLine($"  Vector<int>.Count   = {Vector<int>.Count}");
        Console.WriteLine($"  Vector<double>.Count= {Vector<double>.Count}");
        Console.WriteLine($"  IsHardwareAccelerated={Vector.IsHardwareAccelerated}");
        Console.WriteLine("  128-bit: 4 float / 4 int / 2 double");
        Console.WriteLine("  256-bit: 8 float / 8 int / 4 double");
        Console.WriteLine("  512-bit: 16 float / 16 int / 8 double");
        Debug.Assert(Vector<float>.Count >= 1);
    }

    private static void DemoWhenItHelps()
    {
        Console.WriteLine("-- applicability --");
        float[] data = new float[1024];
        for (int i = 0; i < data.Length; i++)
            data[i] = i;

        float scalar = 0;
        for (int i = 0; i < data.Length; i++)
            scalar += data[i];

        // conceptual parallel add (full Vector demos in later topics)
        float check = data.Length * (data.Length - 1) / 2f;
        Debug.Assert(Math.Abs(scalar - check) < 0.1f);
        Console.WriteLine($"  scalar sum of 0..1023 = {scalar}");
        Console.WriteLine("  Use SIMD last: after profile + after reducing allocations.");
    }
}
