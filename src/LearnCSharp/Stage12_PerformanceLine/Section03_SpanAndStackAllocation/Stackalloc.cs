// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第3部分-Span与栈分配实战.md
// Stage    : Stage12_PerformanceLine
// Section  : Section03_SpanAndStackAllocation
// Item     : Stackalloc
// Topic id : stage12/section03/stackalloc
//
// Lesson: stackalloc → Span for small buffers; scope-bound; large → heap/pool.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section03;

internal static class Stackalloc
{
    [LearnTopic("stage12/section03/stackalloc")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Stackalloc ===");
        DemoTryFormatOnStack();
        DemoSmallVsLargePattern();
        DemoScopeRules();
        return 0;
    }

    private static void DemoTryFormatOnStack()
    {
        Console.WriteLine("-- stackalloc + TryFormat (zero heap for buffer) --");
        long before = GC.GetTotalAllocatedBytes(precise: true);
        Span<char> buf = stackalloc char[32];
        bool ok = 12345.TryFormat(buf, out int written);
        Debug.Assert(ok && written == 5);
        ReadOnlySpan<char> digits = buf[..written];
        Debug.Assert(digits.SequenceEqual("12345"));
        long after = GC.GetTotalAllocatedBytes(precise: true);
        Console.WriteLine($"  formatted '{digits}' written={written}, alloc Δ≈{after - before}");
        Console.WriteLine("  Prefer Span over unsafe raw pointers for bounds safety.");
    }

    private static void DemoSmallVsLargePattern()
    {
        Console.WriteLine("-- small stack / large heap pattern --");
        int sumSmall = FillAndSum(64);
        int sumLarge = FillAndSum(4096);
        Debug.Assert(sumSmall == 64 && sumLarge == 4096);
        Console.WriteLine($"  sumSmall={sumSmall}, sumLarge={sumLarge}");
        Console.WriteLine("  stack is limited (~1MB/thread); large stackalloc → StackOverflow risk.");
    }

    private static void DemoScopeRules()
    {
        Console.WriteLine("-- scope rules --");
        Console.WriteLine("  stackalloc memory lives only for the current stack frame.");
        Console.WriteLine("  Cannot return Span over stackalloc (CS8352) — compiler ref-safety.");
        Console.WriteLine("  Experience: keep stack buffers small (≤256–1024 bytes common).");
        Span<byte> local = stackalloc byte[8];
        local.Fill(0xAB);
        Debug.Assert(local[0] == 0xAB);
        Console.WriteLine($"  local[0]=0x{local[0]:X2}");
    }

    private static int FillAndSum(int len)
    {
        // stack for small, heap for large
        Span<byte> buf = len <= 256 ? stackalloc byte[256] : new byte[len];
        Span<byte> used = buf[..len];
        used.Fill(1);
        int sum = 0;
        foreach (byte b in used)
            sum += b;
        return sum;
    }
}
