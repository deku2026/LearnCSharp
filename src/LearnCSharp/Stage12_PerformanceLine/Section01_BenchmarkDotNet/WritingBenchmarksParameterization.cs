// LearnCSharp example (filled)
// Doc      : CSharp-阶段12-性能线-第1部分-BenchmarkDotNet.md
// Stage    : Stage12_PerformanceLine
// Section  : Section01_BenchmarkDotNet
// Item     : WritingBenchmarksParameterization
// Topic id : stage12/section01/writing_benchmarks_parameterization
//
// Lesson: [Benchmark]/[Params]/[GlobalSetup]/Baseline/return values (anti-DCE)/async.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage12.Section01;

internal static class WritingBenchmarksParameterization
{
    [LearnTopic("stage12/section01/writing_benchmarks_parameterization")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WritingBenchmarksParameterization ===");
        DemoApiAttributes();
        DemoParamsConcept();
        DemoAntiDceReturnValue();
        return 0;
    }

    private static void DemoApiAttributes()
    {
        Console.WriteLine("-- writing benchmarks (API shapes; package not referenced) --");
        Console.WriteLine("  [Params(100, 10_000)] public int N;");
        Console.WriteLine("  [GlobalSetup] public void Setup() { /* prepare _data, not timed */ }");
        Console.WriteLine("  [Benchmark(Baseline = true)] public int LinqSum() => _data.Sum();");
        Console.WriteLine("  [Benchmark] public int LoopSum() { int s=0; foreach(var x in _data) s+=x; return s; }");
        Console.WriteLine("  [Arguments(...)] for method args; [IterationSetup] only when needed (costly).");
        Console.WriteLine("  async: [Benchmark] public async Task<int> ComputeAsync() => await GetAsync();");
        Console.WriteLine("  Anti-DCE: return result OR Consumer.Consume(value) for multi-results.");
    }

    private static void DemoParamsConcept()
    {
        Console.WriteLine("-- Params idea: re-run matrix per N (algorithm scale sensitivity) --");
        int[] sizes = [100, 1_000, 10_000];
        foreach (int n in sizes)
        {
            int[] data = CreateData(n);
            Stopwatch sw = Stopwatch.StartNew();
            int sum = LoopSum(data);
            sw.Stop();
            Debug.Assert(sum == n * (n - 1) / 2);
            Console.WriteLine($"  N={n,6}: sum={sum}, loop≈{sw.Elapsed.TotalMilliseconds:F3} ms");
        }
    }

    private static void DemoAntiDceReturnValue()
    {
        Console.WriteLine("-- anti-DCE --");
        int[] data = CreateData(256);
        // BAD shape for BDN: void method with unused result → may be optimized away
        // GOOD: return the sum so harness keeps the value live
        int good = LoopSum(data);
        Debug.Assert(good == 256 * 255 / 2);
        Console.WriteLine($"  returned sum={good} (BDN keeps return values alive)");
        Console.WriteLine("  Many intermediate values: Consumer.Consume / DoNotOptimize equivalent.");
    }

    private static int[] CreateData(int n)
    {
        int[] data = new int[n];
        for (int i = 0; i < n; i++)
            data[i] = i;
        return data;
    }

    private static int LoopSum(int[] data)
    {
        int s = 0;
        foreach (int x in data)
            s += x;
        return s;
    }
}
