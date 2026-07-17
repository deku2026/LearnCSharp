// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第3部分-Span与栈分配实战.md
// Stage    : Stage12_PerformanceLine
// Section  : Section03_SpanAndStackAllocation
// Item     : MemoryT
// Topic id : stage12/section03/memory_t
//
// Lesson: Memory<T> is storable / awaitable; use .Span only in synchronous scopes.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section03;

internal static class MemoryT
{
    [LearnTopic("stage12/section03/memory_t")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MemoryT ===");
        DemoMemoryVsSpan();
        DemoAsyncWithMemory();
        DemoLocalMethodPattern();
        return 0;
    }

    private static void DemoMemoryVsSpan()
    {
        Console.WriteLine("-- Memory<T> vs Span<T> --");
        byte[] buffer = new byte[16];
        Memory<byte> mem = buffer.AsMemory(0, 8);
        Span<byte> span = mem.Span;
        span.Fill(0x11);
        Debug.Assert(buffer[0] == 0x11 && buffer[7] == 0x11);
        Console.WriteLine($"  Memory length={mem.Length}, buffer[0]=0x{buffer[0]:X2}");
        Console.WriteLine("  Memory: field-ok, await-ok, heap-ok. Span: stack-only ref struct.");
        Console.WriteLine("  Pattern: pass Memory across async; operate via memory.Span sync.");
    }

    private static void DemoAsyncWithMemory()
    {
        Console.WriteLine("-- async API shape with Memory --");
        byte[] data = new byte[32];
        int written = FillAsync(data.AsMemory(0, 16)).GetAwaiter().GetResult();
        Debug.Assert(written == 16);
        Debug.Assert(data[0] == 7 && data[15] == 7);
        Console.WriteLine($"  FillAsync wrote {written} bytes (Stream.ReadAsync uses Memory<byte>).");
    }

    private static void DemoLocalMethodPattern()
    {
        Console.WriteLine("-- local method: use Span inside async without crossing await --");
        int result = ProcessWithLocalAsync().GetAwaiter().GetResult();
        Debug.Assert(result == 6);
        Console.WriteLine($"  ProcessWithLocalAsync → {result}");
        Console.WriteLine("  Keep Span in a non-async local function called between awaits.");
    }

    private static async Task<int> FillAsync(Memory<byte> buffer)
    {
        await Task.Yield();
        buffer.Span.Fill(7);
        return buffer.Length;
    }

    private static async Task<int> ProcessWithLocalAsync()
    {
        byte[] owned = [1, 2, 3];
        Memory<byte> mem = owned;
        await Task.Yield();

        // Span cannot cross await; local function is fine
        return Sum(mem.Span);

        static int Sum(ReadOnlySpan<byte> s)
        {
            int t = 0;
            foreach (byte b in s)
                t += b;
            return t;
        }
    }
}
