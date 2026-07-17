// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第1部分-BenchmarkDotNet.md
// Stage    : Stage12_PerformanceLine
// Section  : Section01_BenchmarkDotNet
// Item     : DiagnosersMemoryDisasm
// Topic id : stage12/section01/diagnosers_memory_disasm
//
// Lesson: MemoryDiagnoser / DisassemblyDiagnoser / hardware counters — shapes + local alloc demos.

using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        DemoAllocContrastMeasured();
        DemoDisasmNotes();
        return 0;
    }

    private static void DemoDiagnoserAttributes()
    {
        Console.WriteLine("-- BDN diagnoser attributes (package shapes) --");
        Console.WriteLine("  [MemoryDiagnoser] [DisassemblyDiagnoser] [HardwareCounters(...)]");
        Console.WriteLine("  MemoryDiagnoser: Gen0/1/2 + Allocated bytes per op.");
    }

    private static void DemoAllocContrastMeasured()
    {
        Console.WriteLine("-- alloc contrast with multi-sample means (MemoryDiagnoser-like) --");
        // Warm
        _ = ConcatPath();
        _ = StringBuilderPath();

        long[] concatSamples = new long[5];
        long[] sbSamples = new long[5];
        for (int s = 0; s < 5; s++)
        {
            long b = GC.GetTotalAllocatedBytes(precise: true);
            string c = ConcatPath();
            concatSamples[s] = GC.GetTotalAllocatedBytes(precise: true) - b;
            Debug.Assert(c.Length > 0);

            b = GC.GetTotalAllocatedBytes(precise: true);
            string built = StringBuilderPath();
            sbSamples[s] = GC.GetTotalAllocatedBytes(precise: true) - b;
            Debug.Assert(built.Length > 0);
        }

        long concatMean = (long)concatSamples.Average();
        long sbMean = (long)sbSamples.Average();
        Console.WriteLine($"  string += mean Δalloc≈{concatMean} bytes");
        Console.WriteLine($"  StringBuilder mean Δalloc≈{sbMean} bytes");
        Debug.Assert(concatMean > sbMean, "string += should allocate more than StringBuilder");
        Console.WriteLine("  BDN MemoryDiagnoser reports Gen collections + Allocated/op cleanly.");
    }

    private static void DemoDisasmNotes()
    {
        Console.WriteLine("-- DisassemblyDiagnoser purpose --");
        Console.WriteLine("  Verify inlining, BCE, SIMD, dead branches — Release only.");
        int x = NoInlineTick();
        Console.WriteLine($"  NoInlineTick={x}");
        Debug.Assert(x != 0 || Environment.TickCount >= 0); // TickCount is a real value; just confirm it ran
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string ConcatPath()
    {
        string concat = "";
        for (int i = 0; i < 50; i++)
            concat += i.ToString();
        return concat;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string StringBuilderPath()
    {
        StringBuilder sb = new(128);
        for (int i = 0; i < 50; i++)
            sb.Append(i);
        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NoInlineTick() => Environment.TickCount;
}
