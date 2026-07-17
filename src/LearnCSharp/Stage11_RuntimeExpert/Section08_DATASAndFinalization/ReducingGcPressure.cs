// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第8部分-DATAS与终结化调优.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section08_DATASAndFinalization
// Item     : ReducingGcPressure
// Topic id : stage11/section08/reducing_gc_pressure
//
// Lesson: Span, stackalloc, ArrayPool, structs reduce allocations / gen0 churn.

using System.Buffers;
using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section08;

internal static class ReducingGcPressure
{
    [LearnTopic("stage11/section08/reducing_gc_pressure")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReducingGcPressure ===");
        DemoAllocContrast();
        DemoArrayPool();
        DemoSpanStackalloc();
        DemoGen0Pressure();
        return 0;
    }

    private static void DemoAllocContrast()
    {
        Console.WriteLine("-- class vs struct allocation --");
        // Warm
        _ = new PointClass(0, 0);
        _ = new PointStruct(0, 0).GetHashCode();

        long beforeClass = GC.GetAllocatedBytesForCurrentThread();
        int hc = 0;
        for (int i = 0; i < 5_000; i++)
            hc ^= new PointClass(i, i).GetHashCode();
        long afterClass = GC.GetAllocatedBytesForCurrentThread();

        long beforeStruct = GC.GetAllocatedBytesForCurrentThread();
        int hs = 0;
        for (int i = 0; i < 5_000; i++)
            hs ^= new PointStruct(i, i).GetHashCode();
        long afterStruct = GC.GetAllocatedBytesForCurrentThread();

        long classDelta = afterClass - beforeClass;
        long structDelta = afterStruct - beforeStruct;
        Console.WriteLine($"  5k PointClass  Δalloc={classDelta}, hash={hc}");
        Console.WriteLine($"  5k PointStruct Δalloc={structDelta}, hash={hs}");
        Debug.Assert(classDelta > 0, "class loop must allocate");
        Debug.Assert(structDelta < classDelta, "struct loop should allocate less than class loop");
    }

    private static void DemoArrayPool()
    {
        Console.WriteLine("-- ArrayPool vs new byte[] --");
        long beforeNew = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100; i++)
            _ = new byte[4096];
        long afterNew = GC.GetAllocatedBytesForCurrentThread();

        long beforePool = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100; i++)
        {
            byte[] rented = ArrayPool<byte>.Shared.Rent(4096);
            rented[0] = 1;
            ArrayPool<byte>.Shared.Return(rented);
        }

        long afterPool = GC.GetAllocatedBytesForCurrentThread();
        Console.WriteLine($"  100× new byte[4096] Δ≈{afterNew - beforeNew}");
        Console.WriteLine($"  100× ArrayPool rent/return Δ≈{afterPool - beforePool}");
        Debug.Assert(afterNew - beforeNew > afterPool - beforePool);
    }

    private static void DemoSpanStackalloc()
    {
        Console.WriteLine("-- stackalloc + Span (zero heap for buffer) --");
        long before = GC.GetAllocatedBytesForCurrentThread();
        int sum = StackSum();
        long after = GC.GetAllocatedBytesForCurrentThread();
        Debug.Assert(sum == 10);
        Console.WriteLine($"  StackSum={sum}, Δalloc={after - before} (expect 0 for buffer)");
        Debug.Assert(after - before == 0);

        Span<char> buf = stackalloc char[16];
        bool ok = 12345.TryFormat(buf, out int written);
        Debug.Assert(ok && written == 5);
        Console.WriteLine($"  TryFormat → '{new string(buf[..written])}'");
    }

    private static void DemoGen0Pressure()
    {
        Console.WriteLine("-- Gen0 collection count under allocation pressure --");
        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        int before = GC.CollectionCount(0);
        for (int i = 0; i < 2_000; i++)
            _ = new byte[8_000];
        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        int after = GC.CollectionCount(0);
        Console.WriteLine($"  CollectionCount(0): {before} → {after}");
        Debug.Assert(after > before);

        StringBuilder sb = new StringBuilder(64);
        for (int i = 0; i < 10; i++)
        {
            sb.Clear();
            sb.Append("id=").Append(i);
            Debug.Assert(sb.Length > 0);
        }

        Console.WriteLine("  Prefer: pool, Span, struct, reuse StringBuilder on hot paths.");
    }

    private static int StackSum()
    {
        Span<int> buf = stackalloc int[4];
        buf[0] = 1;
        buf[1] = 2;
        buf[2] = 3;
        buf[3] = 4;
        int sum = 0;
        foreach (int v in buf)
            sum += v;
        return sum;
    }

    private sealed class PointClass(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }

    private readonly struct PointStruct(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
