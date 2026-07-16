// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第3部分-Span与栈分配实战.md
// Stage    : Stage12_PerformanceLine
// Section  : Section03_SpanAndStackAllocation
// Item     : ArrayPoolStringZeroAlloc
// Topic id : stage12/section03/array_pool_string_zero_alloc
//
// Lesson: ArrayPool/MemoryPool + string.Create/AsSpan for low-allocation paths.

using System.Buffers;
using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section03;

internal static class ArrayPoolStringZeroAlloc
{
    [LearnTopic("stage12/section03/array_pool_string_zero_alloc")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ArrayPoolStringZeroAlloc ===");
        DemoArrayPoolRentReturn();
        DemoStringCreateAndAsSpan();
        DemoPitfalls();
        return 0;
    }

    private static void DemoArrayPoolRentReturn()
    {
        Console.WriteLine("-- ArrayPool<T>.Shared Rent/Return --");
        ArrayPool<byte> pool = ArrayPool<byte>.Shared;
        byte[] rented = pool.Rent(1024);
        try
        {
            Debug.Assert(rented.Length >= 1024);
            Span<byte> used = rented.AsSpan(0, 1024);
            used.Clear();
            used[0] = 1;
            used[1] = 2;
            int sum = used[0] + used[1];
            Debug.Assert(sum == 3);
            Console.WriteLine($"  rented.Length={rented.Length} (≥1024), sum={sum}");
        }
        finally
        {
            // clearArray: true when buffer held secrets
            pool.Return(rented, clearArray: true);
        }

        Console.WriteLine("  MemoryPool<T> / IMemoryOwner for Memory-based pipelines.");
    }

    private static void DemoStringCreateAndAsSpan()
    {
        Console.WriteLine("-- string zero-alloc helpers --");
        string built = string.Create(5, 42, static (span, state) =>
        {
            "id=".AsSpan().CopyTo(span);
            bool ok = state.TryFormat(span[3..], out int w);
            Debug.Assert(ok && w == 2);
        });
        Debug.Assert(built.StartsWith("id=", StringComparison.Ordinal));
        Console.WriteLine($"  string.Create → '{built}'");

        string csv = "a,b,c";
        int parts = 0;
        ReadOnlySpan<char> rest = csv.AsSpan();
        while (true)
        {
            int comma = rest.IndexOf(',');
            ReadOnlySpan<char> token = comma < 0 ? rest : rest[..comma];
            parts++;
            if (comma < 0)
                break;
            rest = rest[(comma + 1)..];
            _ = token;
        }

        Debug.Assert(parts == 3);
        Console.WriteLine($"  AsSpan split tokens={parts} (no Substring allocs per token)");
    }

    private static void DemoPitfalls()
    {
        Console.WriteLine("-- ArrayPool pitfalls --");
        Console.WriteLine("  Rent may return larger array — always track your logical length.");
        Console.WriteLine("  Always Return (try/finally); double-Return is dangerous.");
        Console.WriteLine("  Do not use rented array after Return (data may be reused/cleared).");
        Console.WriteLine("  LOH: large arrays benefit from pooling to avoid LOH churn.");
        StringBuilder sb = new(32);
        sb.Append("reuse");
        sb.Clear();
        sb.Append("ok");
        Debug.Assert(sb.ToString() == "ok");
        Console.WriteLine("  Also reuse StringBuilder / encoders on hot paths.");
    }
}
