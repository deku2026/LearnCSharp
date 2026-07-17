// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第4部分-SIMD与向量化.md
// Stage    : Stage12_PerformanceLine
// Section  : Section04_SimdAndVectorization
// Item     : VectorTVariableWidth
// Topic id : stage12/section04/vector_t_variable_width
//
// Lesson: Vector<T> variable width + main loop / reduce / scalar tail pattern.

using System.Diagnostics;
using System.Numerics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section04;

internal static class VectorTVariableWidth
{
    [LearnTopic("stage12/section04/vector_t_variable_width")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== VectorTVariableWidth ===");
        DemoWidth();
        DemoSum();
        DemoScale();
        return 0;
    }

    private static void DemoWidth()
    {
        Console.WriteLine("-- Vector<T>.Count on this machine --");
        Console.WriteLine($"  Count float={Vector<float>.Count}, accelerated={Vector.IsHardwareAccelerated}");
        Debug.Assert(Vector<float>.Count >= 1);
    }

    private static void DemoSum()
    {
        Console.WriteLine("-- sum: scalar vs Vector<T> (main + tail) --");
        float[] data = new float[1000];
        for (int i = 0; i < data.Length; i++)
            data[i] = 1f;

        float scalar = SumScalar(data);
        float vector = SumVector(data);
        Debug.Assert(Math.Abs(scalar - vector) < 0.01f);
        Debug.Assert(Math.Abs(scalar - 1000f) < 0.01f);
        Console.WriteLine($"  scalar={scalar}, vector={vector}");
    }

    private static void DemoScale()
    {
        Console.WriteLine("-- scale in place with Vector<T> --");
        float[] data = new float[17];
        for (int i = 0; i < data.Length; i++)
            data[i] = i;
        Scale(data, 2f);
        Debug.Assert(Math.Abs(data[0] - 0) < 1e-5f);
        Debug.Assert(Math.Abs(data[16] - 32f) < 1e-5f);
        Console.WriteLine($"  data[16]={data[16]} (17 not multiple of Count → tail handled)");
    }

    private static float SumScalar(ReadOnlySpan<float> data)
    {
        float s = 0;
        for (int i = 0; i < data.Length; i++)
            s += data[i];
        return s;
    }

    private static float SumVector(ReadOnlySpan<float> data)
    {
        int width = Vector<float>.Count;
        Vector<float> acc = Vector<float>.Zero;
        int i = 0;
        for (; i <= data.Length - width; i += width)
        {
            Vector<float> v = new(data.Slice(i, width));
            acc += v;
        }

        float sum = Vector.Sum(acc);
        for (; i < data.Length; i++)
            sum += data[i];
        return sum;
    }

    private static void Scale(Span<float> data, float factor)
    {
        int width = Vector<float>.Count;
        Vector<float> f = new(factor);
        int i = 0;
        for (; i <= data.Length - width; i += width)
        {
            Vector<float> v = new(data.Slice(i, width));
            (v * f).CopyTo(data.Slice(i, width));
        }

        for (; i < data.Length; i++)
            data[i] *= factor;
    }
}
