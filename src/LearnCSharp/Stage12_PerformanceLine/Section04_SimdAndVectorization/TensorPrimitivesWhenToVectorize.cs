// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第4部分-SIMD与向量化.md
// Stage    : Stage12_PerformanceLine
// Section  : Section04_SimdAndVectorization
// Item     : TensorPrimitivesWhenToVectorize
// Topic id : stage12/section04/tensor_primitives_when_to_vectorize
//
// Lesson: TensorPrimitives (package) for high-level math; when to vectorize; performance-line wrap-up.
// Note: System.Numerics.Tensors is not in Directory.Packages.props — API shown as shapes + Vector demos.

using System.Diagnostics;
using System.Numerics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section04;

internal static class TensorPrimitivesWhenToVectorize
{
    [LearnTopic("stage12/section04/tensor_primitives_when_to_vectorize")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TensorPrimitivesWhenToVectorize ===");
        DemoTensorPrimitivesApiShape();
        DemoWhenToVectorize();
        DemoPerformanceLineWrapUp();
        return 0;
    }

    private static void DemoTensorPrimitivesApiShape()
    {
        Console.WriteLine("-- TensorPrimitives API shape (NuGet: System.Numerics.Tensors) --");
        Console.WriteLine("  // TensorPrimitives.Add(x, y, destination);");
        Console.WriteLine("  // TensorPrimitives.Sum(span);");
        Console.WriteLine("  // TensorPrimitives.Cos/Sin/Exp/Log/... vectorized math helpers");
        Console.WriteLine("  Prefer library primitives before hand-rolled intrinsics.");
        Console.WriteLine("  This repo has empty Directory.Packages.props — no package reference.");

        // Local stand-in using Vector for educational sum/add
        float[] a = [1, 2, 3, 4, 5, 6, 7, 8];
        float[] b = [8, 7, 6, 5, 4, 3, 2, 1];
        float[] dest = new float[a.Length];
        AddPortable(a, b, dest);
        float sum = SumPortable(dest);
        Debug.Assert(Math.Abs(sum - 72f) < 0.01f); // each pair sums to 9, 8 pairs
        Console.WriteLine($"  portable Add+Sum stand-in → sum={sum}");
    }

    private static void DemoWhenToVectorize()
    {
        Console.WriteLine("-- when to vectorize --");
        Console.WriteLine("  YES: large arrays, element-independent ops, confirmed hot in profile.");
        Console.WriteLine("  NO:  tiny N, branchy logic, already library-vectorized, not a hotspot.");
        Console.WriteLine("  Order: profile → cut allocations (Span/pool) → BDN → then SIMD.");
        Console.WriteLine("  Always keep scalar fallback; check IsHardwareAccelerated/IsSupported.");
        Console.WriteLine("  Portability: Vector/Vector128/256 first; hardware intrinsics last.");
    }

    private static void DemoPerformanceLineWrapUp()
    {
        Console.WriteLine("-- Stage12 performance line wrap-up --");
        Console.WriteLine("  §1 BDN: measure correctly (not naive Stopwatch).");
        Console.WriteLine("  §2 Profile: find real hotspots (EventPipe tools).");
        Console.WriteLine("  §3 Span/stackalloc/pool: kill allocation hotspots.");
        Console.WriteLine("  §4 SIMD: last-mile CPU throughput on data-parallel loops.");
        Console.WriteLine("  Mantra: measure → locate → reduce alloc → vectorize → verify.");
        Debug.Assert(Vector.IsHardwareAccelerated || !Vector.IsHardwareAccelerated);
    }

    private static void AddPortable(ReadOnlySpan<float> x, ReadOnlySpan<float> y, Span<float> dest)
    {
        Debug.Assert(x.Length == y.Length && x.Length == dest.Length);
        int width = Vector<float>.Count;
        int i = 0;
        for (; i <= x.Length - width; i += width)
        {
            Vector<float> vx = new(x.Slice(i, width));
            Vector<float> vy = new(y.Slice(i, width));
            (vx + vy).CopyTo(dest.Slice(i, width));
        }

        for (; i < x.Length; i++)
            dest[i] = x[i] + y[i];
    }

    private static float SumPortable(ReadOnlySpan<float> data)
    {
        int width = Vector<float>.Count;
        Vector<float> acc = Vector<float>.Zero;
        int i = 0;
        for (; i <= data.Length - width; i += width)
            acc += new Vector<float>(data.Slice(i, width));
        float sum = Vector.Sum(acc);
        for (; i < data.Length; i++)
            sum += data[i];
        return sum;
    }
}
