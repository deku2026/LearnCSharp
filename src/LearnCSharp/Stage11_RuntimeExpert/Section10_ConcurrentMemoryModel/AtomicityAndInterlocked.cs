// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第10部分-并发内存模型.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section10_ConcurrentMemoryModel
// Item     : AtomicityAndInterlocked
// Topic id : stage11/section10/atomicity_and_interlocked
//
// Lesson: which loads/stores are atomic; Interlocked RMW operations.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section10;

internal static class AtomicityAndInterlocked
{
    [LearnTopic("stage11/section10/atomicity_and_interlocked")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AtomicityAndInterlocked ===");
        DemoAtomicityRules();
        DemoInterlockedOps();
        DemoRaceExplained();
        return 0;
    }

    private static void DemoAtomicityRules()
    {
        Console.WriteLine("-- atomicity (ECMA) --");
        Console.WriteLine("  Aligned reads/writes of ref and ≤ native word size primitives are atomic.");
        Console.WriteLine("  long/double atomicity on 32-bit is not guaranteed without Interlocked/volatile.");
        Console.WriteLine($"  IntPtr.Size={IntPtr.Size} → word size {IntPtr.Size * 8}-bit");
        Debug.Assert(IntPtr.Size is 4 or 8);
    }

    private static void DemoInterlockedOps()
    {
        Console.WriteLine("-- Interlocked RMW --");
        int x = 0;
        Interlocked.Increment(ref x);
        Interlocked.Add(ref x, 9);
        int old = Interlocked.CompareExchange(ref x, 100, 10);
        Console.WriteLine($"  after Inc+Add x should be 10, CompareExchange old={old}, x={x}");
        Debug.Assert(old == 10 && x == 100);
        long y = 0;
        Interlocked.Exchange(ref y, 5);
        Debug.Assert(y == 5);
        Console.WriteLine($"  Interlocked.Exchange long y={y}");
    }

    private static void DemoRaceExplained()
    {
        Console.WriteLine("-- intentional race (explained, not asserted) --");
        int counter = 0;
        void Bump()
        {
            for (int i = 0; i < 10_000; i++)
                counter++; // non-atomic RMW
        }

        var t1 = new Thread(Bump);
        var t2 = new Thread(Bump);
        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();
        Console.WriteLine($"  unsynchronized counter≈{counter} (expected 20000 if no race; often less)");
        // fixed version
        int safe = 0;
        void BumpSafe()
        {
            for (int i = 0; i < 10_000; i++)
                Interlocked.Increment(ref safe);
        }

        t1 = new Thread(BumpSafe);
        t2 = new Thread(BumpSafe);
        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();
        Debug.Assert(safe == 20_000);
        Console.WriteLine($"  Interlocked counter={safe}");
    }
}
