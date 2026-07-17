// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第4部分-SIMD与向量化.md
// Stage    : Stage12_PerformanceLine
// Section  : Section04_SimdAndVectorization
// Item     : Vector128_256_512Intrinsics
// Topic id : stage12/section04/vector128_256_512_intrinsics
//
// Lesson: fixed-width Vector128/256/512 + IsHardwareAccelerated + hardware multi-path fallback.

using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section04;

internal static class Vector128_256_512Intrinsics
{
    [LearnTopic("stage12/section04/vector128_256_512_intrinsics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Vector128_256_512Intrinsics ===");
        DemoCapability();
        DemoSumDispatch();
        DemoHardwareSpecific();
        return 0;
    }

    private static void DemoCapability()
    {
        Console.WriteLine("-- fixed-width capabilities --");
        Console.WriteLine($"  Vector128 accel={Vector128.IsHardwareAccelerated} count float={Vector128<float>.Count}");
        Console.WriteLine($"  Vector256 accel={Vector256.IsHardwareAccelerated} count float={Vector256<float>.Count}");
        Console.WriteLine($"  Vector512 accel={Vector512.IsHardwareAccelerated} count float={Vector512<float>.Count}");
        Console.WriteLine($"  Avx2={Avx2.IsSupported} Avx512F={Avx512F.IsSupported}");
    }

    private static void DemoSumDispatch()
    {
        Console.WriteLine("-- sum with best available fixed-width path --");
        float[] data = new float[1000];
        for (int i = 0; i < data.Length; i++)
            data[i] = 1f;

        float sum = SumBest(data);
        Debug.Assert(Math.Abs(sum - 1000f) < 0.01f);
        Console.WriteLine($"  SumBest={sum}");
    }

    private static void DemoHardwareSpecific()
    {
        Console.WriteLine("-- multi-path sketch (JIT folds dead branches) --");
        if (Avx512F.IsSupported)
            Console.WriteLine("  path: Avx512F / Vector512");
        else if (Avx2.IsSupported)
            Console.WriteLine("  path: Avx2 / Vector256");
        else if (Sse2.IsSupported)
            Console.WriteLine("  path: Sse2 / Vector128");
        else
            Console.WriteLine("  path: scalar fallback");

        Vector128<float> a = Vector128.Create(1f, 2f, 3f, 4f);
        Vector128<float> b = Vector128.Create(1f);
        Vector128<float> c = Vector128.Add(a, b);
        Debug.Assert(Math.Abs(c[0] - 2f) < 1e-5f);
        Console.WriteLine($"  Vector128.Add sample lane0={c[0]}");
    }

    private static float SumBest(ReadOnlySpan<float> data)
    {
        if (Vector512.IsHardwareAccelerated && data.Length >= Vector512<float>.Count)
            return Sum512(data);
        if (Vector256.IsHardwareAccelerated && data.Length >= Vector256<float>.Count)
            return Sum256(data);
        if (Vector128.IsHardwareAccelerated && data.Length >= Vector128<float>.Count)
            return Sum128(data);
        return SumScalar(data);
    }

    private static float SumScalar(ReadOnlySpan<float> data)
    {
        float s = 0;
        for (int i = 0; i < data.Length; i++)
            s += data[i];
        return s;
    }

    private static float Sum128(ReadOnlySpan<float> data)
    {
        int width = Vector128<float>.Count;
        Vector128<float> acc = Vector128<float>.Zero;
        int i = 0;
        for (; i <= data.Length - width; i += width)
        {
            Vector128<float> v = Vector128.LoadUnsafe(in data[i]);
            acc = Vector128.Add(acc, v);
        }

        float sum = Vector128.Sum(acc);
        for (; i < data.Length; i++)
            sum += data[i];
        return sum;
    }

    private static float Sum256(ReadOnlySpan<float> data)
    {
        int width = Vector256<float>.Count;
        Vector256<float> acc = Vector256<float>.Zero;
        int i = 0;
        for (; i <= data.Length - width; i += width)
        {
            Vector256<float> v = Vector256.LoadUnsafe(in data[i]);
            acc = Vector256.Add(acc, v);
        }

        float sum = Vector256.Sum(acc);
        for (; i < data.Length; i++)
            sum += data[i];
        return sum;
    }

    private static float Sum512(ReadOnlySpan<float> data)
    {
        int width = Vector512<float>.Count;
        Vector512<float> acc = Vector512<float>.Zero;
        int i = 0;
        for (; i <= data.Length - width; i += width)
        {
            Vector512<float> v = Vector512.LoadUnsafe(in data[i]);
            acc = Vector512.Add(acc, v);
        }

        float sum = Vector512.Sum(acc);
        for (; i < data.Length; i++)
            sum += data[i];
        return sum;
    }
}
