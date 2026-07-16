// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第10部分-并发内存模型.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section10_ConcurrentMemoryModel
// Item     : ClrMemoryModel
// Topic id : stage11/section10/clr_memory_model
//
// Lesson: ECMA CLI memory model; reordering; what lock/volatile/Interlocked guarantee.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section10;

internal static class ClrMemoryModel
{
    [LearnTopic("stage11/section10/clr_memory_model")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ClrMemoryModel ===");
        DemoRules();
        DemoPublication();
        DemoTools();
        return 0;
    }

    private static void DemoRules()
    {
        Console.WriteLine("-- CLR / ECMA memory model (simplified) --");
        Console.WriteLine("  Compiler + CPU may reorder independent memory ops.");
        Console.WriteLine("  Reads/writes can be stale without synchronization.");
        Console.WriteLine("  lock, volatile, Interlocked, Thread.MemoryBarrier establish order.");
        Console.WriteLine("  C# volatile ≠ C++ volatile; it is acquire/release style fences on accesses.");
    }

    private static void DemoPublication()
    {
        Console.WriteLine("-- safe publication patterns --");
        var box = new Published(42);
        // static initialization is safe publication
        Debug.Assert(Published.StaticValue == 7);
        Console.WriteLine($"  static Published.StaticValue={Published.StaticValue}");
        Console.WriteLine($"  instance Value={box.Value}");
        Console.WriteLine("  After lock release / volatile write, other threads see prior writes.");
        Debug.Assert(box.Value == 42);
    }

    private static void DemoTools()
    {
        Console.WriteLine("-- primitives map --");
        Console.WriteLine("  lock / Monitor: mutual exclusion + full fence barriers");
        Console.WriteLine("  volatile fields: prevent some reordering on that field");
        Console.WriteLine("  Interlocked: atomic RMW + barriers");
        Console.WriteLine("  Lazy<T> / ThreadLocal / Concurrent* collections: higher-level safety");
        int x = 0;
        Interlocked.Exchange(ref x, 1);
        Debug.Assert(x == 1);
        Console.WriteLine($"  Interlocked.Exchange demo x={x}");
    }

    private sealed class Published
    {
        public static int StaticValue { get; } = 7;
        public int Value { get; }
        public Published(int v) => Value = v;
    }
}
