// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第3部分-Span与栈分配实战.md
// Stage    : Stage12_PerformanceLine
// Section  : Section03_SpanAndStackAllocation
// Item     : SpanAndReadOnlySpan
// Topic id : stage12/section03/span_and_readonly_span
//
// Lesson: Span/ReadOnlySpan = non-owning zero-copy views; ref struct limits.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section03;

internal static class SpanAndReadOnlySpan
{
    [LearnTopic("stage12/section03/span_and_readonly_span")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SpanAndReadOnlySpan ===");
        DemoZeroCopySlice();
        DemoStringAsSpan();
        DemoRefStructLimits();
        return 0;
    }

    private static void DemoZeroCopySlice()
    {
        Console.WriteLine("-- zero-copy array slice --");
        int[] numbers = [10, 20, 30, 40, 50];
        Span<int> slice = numbers.AsSpan(1, 3); // 20,30,40 — view, not copy
        slice[0] = 99;
        Debug.Assert(numbers[1] == 99);
        Debug.Assert(slice.Length == 3);
        Console.WriteLine($"  slice={string.Join(',', slice.ToArray())}, original[1]={numbers[1]}");
        Console.WriteLine("  Mutating the view mutates the underlying array.");
    }

    private static void DemoStringAsSpan()
    {
        Console.WriteLine("-- ReadOnlySpan<char> vs Substring --");
        string text = "hello world";
        long beforeSub = GC.GetTotalAllocatedBytes(precise: true);
        string sub = text.Substring(0, 5);
        long afterSub = GC.GetTotalAllocatedBytes(precise: true);

        long beforeSpan = GC.GetTotalAllocatedBytes(precise: true);
        ReadOnlySpan<char> hello = text.AsSpan(0, 5);
        bool eq = hello.SequenceEqual("hello");
        long afterSpan = GC.GetTotalAllocatedBytes(precise: true);

        Debug.Assert(sub == "hello" && eq);
        Console.WriteLine($"  Substring alloc Δ≈{afterSub - beforeSub} (new string)");
        Console.WriteLine($"  AsSpan alloc Δ≈{afterSpan - beforeSpan} (typically 0 for the view)");
        Console.WriteLine($"  hello equals 'hello'? {eq}");
    }

    private static void DemoRefStructLimits()
    {
        Console.WriteLine("-- Span is ref struct (limits) --");
        Console.WriteLine("  Cannot be a field of a class; cannot box; cannot cross await/yield.");
        Console.WriteLine("  Can be local / parameter / field of another ref struct.");
        Console.WriteLine("  Unifies: arrays, strings, stackalloc, unmanaged memory views.");
        Span<byte> bytes = stackalloc byte[4] { 1, 2, 3, 4 };
        int sum = 0;
        foreach (byte b in bytes)
            sum += b;
        Debug.Assert(sum == 10);
        Console.WriteLine($"  stackalloc Span sum={sum}");
    }
}
