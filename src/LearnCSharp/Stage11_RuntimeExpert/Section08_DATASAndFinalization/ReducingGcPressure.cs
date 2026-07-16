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
        DemoArrayPool();
        DemoSpanStackalloc();
        DemoStructVsClass();
        return 0;
    }

    private static void DemoArrayPool()
    {
        Console.WriteLine("-- ArrayPool<T>.Shared --");
        byte[] rented = ArrayPool<byte>.Shared.Rent(4096);
        try
        {
            rented[0] = 1;
            rented[1] = 2;
            int sum = rented[0] + rented[1];
            Debug.Assert(sum == 3);
            Console.WriteLine($"  rented length≥4096 actual={rented.Length}, sum={sum}");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private static void DemoSpanStackalloc()
    {
        Console.WriteLine("-- stackalloc + Span (zero heap) --");
        Span<char> buf = stackalloc char[16];
        bool ok = 12345.TryFormat(buf, out int written);
        Debug.Assert(ok && written > 0);
        string s = new(buf[..written]);
        Debug.Assert(s == "12345");
        Console.WriteLine($"  TryFormat → '{s}'");
    }

    private static void DemoStructVsClass()
    {
        Console.WriteLine("-- prefer struct / value semantics on hot paths --");
        long allocish = GC.GetTotalAllocatedBytes(precise: false);
        int h = 0;
        for (int i = 0; i < 10_000; i++)
            h ^= new Point(i, i).GetHashCode();
        long after = GC.GetTotalAllocatedBytes(precise: false);
        Console.WriteLine($"  10k struct Point hashes={h}, allocated Δ≈{after - allocish} (may be 0)");
        // StringBuilder reuse pattern
        var sb = new StringBuilder(64);
        for (int i = 0; i < 10; i++)
        {
            sb.Clear();
            sb.Append("id=").Append(i);
            Debug.Assert(sb.Length > 0);
        }

        Console.WriteLine("  Also: reuse buffers, avoid LINQ on hot paths, pool sockets/encoders.");
        Console.WriteLine($"  AddMemoryPressure exists for unmanaged; use carefully. GC.AddMemoryPressure API available.");
    }

    private readonly struct Point(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
