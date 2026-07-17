// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第3部分-Span与栈分配实战.md
// Stage    : Stage12_PerformanceLine
// Section  : Section03_SpanAndStackAllocation
// Item     : RefStructAndScoped
// Topic id : stage12/section03/ref_struct_and_scoped
//
// Lesson: ref struct + ref fields + scoped = compile-time lifetime safety for views.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section03;

internal static class RefStructAndScoped
{
    [LearnTopic("stage12/section03/ref_struct_and_scoped")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== RefStructAndScoped ===");
        DemoCustomRefStruct();
        DemoScopedParameter();
        DemoParamsSpanAndAllows();
        DemoParamsSpanZeroAlloc();
        return 0;
    }

    private static void DemoCustomRefStruct()
    {
        Console.WriteLine("-- custom ref struct with ref field (Span-like) --");
        int[] data = [1, 2, 3, 4];
        IntWindow window = new(data.AsSpan(1, 2));
        Debug.Assert(window.Length == 2);
        Debug.Assert(window[0] == 2 && window[1] == 3);
        window[0] = 20;
        Debug.Assert(data[1] == 20);
        Console.WriteLine($"  window=[{window[0]},{window[1]}], data[1]={data[1]}");
        Console.WriteLine("  Span<T> itself is a ref struct with a ref field + length.");
    }

    private static void DemoScopedParameter()
    {
        Console.WriteLine("-- scoped: promise not to escape the span --");
        Span<char> stack = stackalloc char[6];
        "scoped".AsSpan().CopyTo(stack);
        int hash = HashScoped(stack);
        // The hash is deterministic for the fixed input "scoped"; verify stability.
        int hashAgain = HashScoped(stack);
        Debug.Assert(hash == hashAgain, "HashScoped must be deterministic");
        Console.WriteLine($"  HashScoped(stackalloc \"scoped\")={hash}");
        Console.WriteLine("  Without scoped, compiler may reject stackalloc → method that might store.");
        Console.WriteLine("  CS8352/CS8350: escapes that outlive the referent are compile errors.");
    }

    // C# 13 params ReadOnlySpan<T>: the compiler synthesizes a stackalloc'd span for the
    // call (no array allocation) when the callee takes params ReadOnlySpan<T>. Observable
    // via GetAllocatedBytesForCurrentThread — contrast with a params int[] overload.
    private static void DemoParamsSpanZeroAlloc()
    {
        Console.WriteLine("-- params ReadOnlySpan<T> zero-alloc vs params T[] --");
        // Warm up.
        for (int i = 0; i < 64; i++)
        {
            _ = SumParams(1, 2, 3, 4);
            _ = SumParamsArray(1, 2, 3, 4);
        }
        long beforeSpan = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100_000; i++)
        {
            _ = SumParams(1, 2, 3, 4);
        }
        long spanAlloc = GC.GetAllocatedBytesForCurrentThread() - beforeSpan;
        long beforeArr = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100_000; i++)
        {
            _ = SumParamsArray(1, 2, 3, 4);
        }
        long arrAlloc = GC.GetAllocatedBytesForCurrentThread() - beforeArr;
        Console.WriteLine($"  100k SumParams(params ReadOnlySpan<int>): Δalloc={spanAlloc} bytes");
        Console.WriteLine($"  100k SumParamsArray(params int[]):        Δalloc={arrAlloc} bytes");
        Console.WriteLine("  params ReadOnlySpan → stackalloc-backed (≈0); params T[] → heap array each call.");
        Debug.Assert(arrAlloc > 0, "params int[] must allocate a new array per call");
    }

    private static void DemoParamsSpanAndAllows()
    {
        Console.WriteLine("-- language evolution notes --");
        Console.WriteLine("  C# 13: where T : allows ref struct; params ReadOnlySpan<T> (no array alloc).");
        Console.WriteLine("  C# 14: richer array ↔ Span conversions.");
        int sum = SumParams(1, 2, 3, 4);
        Debug.Assert(sum == 10);
        Console.WriteLine($"  SumParams(1,2,3,4)={sum}");
    }

    private static int HashScoped(scoped ReadOnlySpan<char> data)
    {
        // scoped: we only read; we do not store data past this call
        int h = 17;
        foreach (char c in data)
            h = (h * 31) + c;
        return h;
    }

    private static int SumParams(params ReadOnlySpan<int> values)
    {
        int s = 0;
        foreach (int v in values)
            s += v;
        return s;
    }

    private static int SumParamsArray(params int[] values)
    {
        int s = 0;
        foreach (int v in values)
            s += v;
        return s;
    }

    private ref struct IntWindow
    {
        private readonly ref int _first;
        private readonly int _length;

        public IntWindow(Span<int> span)
        {
            if (span.IsEmpty)
            {
                this = default;
                return;
            }

            _first = ref span[0];
            _length = span.Length;
        }

        public int Length => _length;

        public ref int this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return ref UnsafeAdd(ref _first, index);
            }
        }

        private static ref int UnsafeAdd(ref int start, int index)
        {
            // System.Runtime.CompilerServices.Unsafe is in BCL
            return ref System.Runtime.CompilerServices.Unsafe.Add(ref start, index);
        }
    }
}
