// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第1部分-BenchmarkDotNet.md
// Stage    : Stage12_PerformanceLine
// Section  : Section01_BenchmarkDotNet
// Item     : DiagnosersMemoryDisasm
// Topic id : stage12/section01/diagnosers_memory_disasm
//
// Lesson: MemoryDiagnoser / DisassemblyDiagnoser / hardware counters — shapes + local alloc demos.

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section01;

internal static class DiagnosersMemoryDisasm
{
    [LearnTopic("stage12/section01/diagnosers_memory_disasm")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DiagnosersMemoryDisasm ===");
        DemoDiagnoserAttributes();
        DemoAllocContrast();
        DemoDisasmNotes();
        return 0;
    }

    private static void DemoDiagnoserAttributes()
    {
        Console.WriteLine("-- BDN diagnoser attributes (package shapes) --");
        Console.WriteLine("  [MemoryDiagnoser]     // Gen0/1/2, Allocated bytes per op");
        Console.WriteLine("  [DisassemblyDiagnoser] // native asm for the hot method");
        Console.WriteLine("  [HardwareCounters(...)] // PMC if OS/CPU support (advanced)");
        Console.WriteLine("  Compare jobs: [SimpleJob(RuntimeMoniker.Net80)] vs Net10, etc.");
        Console.WriteLine("  MemoryDiagnoser is the default first stop for GC pressure.");
    }

    private static void DemoAllocContrast()
    {
        Console.WriteLine("-- local alloc contrast (what MemoryDiagnoser would rank) --");
        long beforeConcat = GC.GetTotalAllocatedBytes(precise: true);
        string concat = "";
        for (int i = 0; i < 50; i++)
            concat += i.ToString();
        long afterConcat = GC.GetTotalAllocatedBytes(precise: true);

        long beforeSb = GC.GetTotalAllocatedBytes(precise: true);
        StringBuilder sb = new(128);
        for (int i = 0; i < 50; i++)
            sb.Append(i);
        string built = sb.ToString();
        long afterSb = GC.GetTotalAllocatedBytes(precise: true);

        Debug.Assert(concat.Length > 0 && built.Length > 0);
        Console.WriteLine($"  string += loop Δalloc≈{afterConcat - beforeConcat} bytes");
        Console.WriteLine($"  StringBuilder   Δalloc≈{afterSb - beforeSb} bytes");
        Console.WriteLine("  BDN MemoryDiagnoser reports Gen collections + Allocated/op cleanly.");
    }

    private static void DemoDisasmNotes()
    {
        Console.WriteLine("-- DisassemblyDiagnoser purpose --");
        Console.WriteLine("  Verify: inlining, bounds-check elimination, SIMD codegen, dead branches.");
        Console.WriteLine("  Complements SharpLab / godbolt-style inspection for managed methods.");
        Console.WriteLine("  Always pair with Release + real workload; Debug asm is not representative.");
        int x = Environment.TickCount;
        Debug.Assert(x != int.MinValue || x == int.MinValue);
        Console.WriteLine($"  tick={x} (keep process alive; no real disasm dump in this demo)");
    }
}
