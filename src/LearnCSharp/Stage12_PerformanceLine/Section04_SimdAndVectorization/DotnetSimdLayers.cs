// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第4部分-SIMD与向量化.md
// Stage    : Stage12_PerformanceLine
// Section  : Section04_SimdAndVectorization
// Item     : DotnetSimdLayers
// Topic id : stage12/section04/dotnet_simd_layers
//
// Lesson: portable Vector/Vector128 layers vs hardware-specific Sse/Avx/AdvSimd; JIT folds IsSupported.

using System.Diagnostics;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section04;

internal static class DotnetSimdLayers
{
    [LearnTopic("stage12/section04/dotnet_simd_layers")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DotnetSimdLayers ===");
        DemoLayerMap();
        DemoIsSupportedFolding();
        DemoPortableVsHardware();
        return 0;
    }

    private static void DemoLayerMap()
    {
        Console.WriteLine("-- .NET SIMD layers --");
        Console.WriteLine("  1) System.Numerics.Vector<T> — variable width, portable");
        Console.WriteLine("  2) System.Runtime.Intrinsics.Vector64/128/256/512 — fixed width, portable ops");
        Console.WriteLine("  3) Hardware-specific: Sse/Avx/Avx2/Avx512F (x86), AdvSimd/Sve (ARM)");
        Console.WriteLine("  Prefer 1–2 for maintainability; drop to 3 for rare instructions.");
        Console.WriteLine($"  Vector.IsHardwareAccelerated={Vector.IsHardwareAccelerated}");
        Console.WriteLine($"  Vector128.IsHardwareAccelerated={Vector128.IsHardwareAccelerated}");
        Console.WriteLine($"  Vector256.IsHardwareAccelerated={Vector256.IsHardwareAccelerated}");
        Console.WriteLine($"  Vector512.IsHardwareAccelerated={Vector512.IsHardwareAccelerated}");
    }

    private static void DemoIsSupportedFolding()
    {
        Console.WriteLine("-- IsSupported / IsHardwareAccelerated --");
        Console.WriteLine("  JIT knows the exact CPU → folds checks to true/false, deletes dead paths.");
        Console.WriteLine("  One assembly adapts: old CPU keeps SSE path, new CPU keeps AVX-512 path.");
        Console.WriteLine($"  Sse2.IsSupported={Sse2.IsSupported}");
        Console.WriteLine($"  Avx2.IsSupported={Avx2.IsSupported}");
        Console.WriteLine($"  Avx512F.IsSupported={Avx512F.IsSupported}");
        // ARM AdvSimd would report on ARM hosts; safe to reference type name in text only if needed
        Console.WriteLine("  NamedIntrinsic: Avx2.Add becomes a single CPU instruction, not a call.");
    }

    private static void DemoPortableVsHardware()
    {
        Console.WriteLine("-- portable add demo (Vector128) --");
        Vector128<int> a = Vector128.Create(1, 2, 3, 4);
        Vector128<int> b = Vector128.Create(10, 20, 30, 40);
        Vector128<int> c = Vector128.Add(a, b);
        Debug.Assert(c[0] == 11 && c[3] == 44);
        Console.WriteLine($"  Vector128.Add → [{c[0]},{c[1]},{c[2]},{c[3]}]");
        if (Avx2.IsSupported)
        {
            Vector256<int> x = Vector256.Create(1);
            Vector256<int> y = Vector256.Create(2);
            Vector256<int> z = Avx2.Add(x, y);
            Debug.Assert(z[0] == 3);
            Console.WriteLine($"  Avx2.Add hardware path sample first lane={z[0]}");
        }
        else
        {
            Console.WriteLine("  Avx2 not supported on this CPU — portable path still works.");
        }
    }
}
