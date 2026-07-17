// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : ISourceGeneratorVsIncremental
// Topic id : stage13/section02/isourcegenerator_vs_incremental
//
// Lesson: classic full recompute vs incremental pipeline + caching (IDE keystroke cost).

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section02;

internal static partial class ISourceGeneratorVsIncremental
{
    [LearnTopic("stage13/section02/isourcegenerator_vs_incremental")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ISourceGeneratorVsIncremental ===");
        DemoApiComparison();
        DemoWhyIncrementalMeasured();
        DemoRealJsonContextAsGeneratedArtifact();
        return 0;
    }

    private static void DemoApiComparison()
    {
        Console.WriteLine("-- two generations of API --");
        Console.WriteLine("  ISourceGenerator: full recompute every compile (obsolete)");
        Console.WriteLine("  IIncrementalGenerator: cached pipeline stages (required)");
    }

    private static void DemoWhyIncrementalMeasured()
    {
        Console.WriteLine("-- simulated cache: miss cost vs hit --");
        Dictionary<string, (string input, string output)> cache = new Dictionary<string, (string input, string output)>(StringComparer.Ordinal);
        int misses = 0, hits = 0;

        string RunStage(string key, string input)
        {
            if (cache.TryGetValue(key, out (string input, string output) hit) && hit.input == input)
            {
                hits++;
                return hit.output;
            }

            misses++;
            // Fake "semantic work"
            string output = "gen:" + input.GetHashCode(StringComparison.Ordinal);
            cache[key] = (input, output);
            return output;
        }

        long t0 = Stopwatch.GetTimestamp();
        for (int i = 0; i < 1_000; i++)
        {
            _ = RunStage("A", "class A"); // mostly hits after first
            _ = RunStage("B", "class B");
            if (i % 100 == 0)
                _ = RunStage("A", "class A v" + i); // occasional invalidation
        }

        double ms = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
        Console.WriteLine($"  1000 iterations: hits={hits}, misses={misses}, {ms:F3}ms");
        Debug.Assert(hits > misses);
        Debug.Assert(misses >= 2);
        Console.WriteLine("  Unrelated edits leave other stages cached.");
    }

    private static void DemoRealJsonContextAsGeneratedArtifact()
    {
        Console.WriteLine("-- real generated artifact: JsonSerializerContext --");
        CacheRow row = new CacheRow { Key = "A", Value = 1 };
        string json = JsonSerializer.Serialize(row, IncJsonContext.Default.CacheRow);
        CacheRow? back = JsonSerializer.Deserialize(json, IncJsonContext.Default.CacheRow);
        Debug.Assert(back is { Key: "A", Value: 1 });
        Console.WriteLine($"  {json}");
        Console.WriteLine("  STJ source gen is an incremental generator in the SDK.");
    }

    private sealed class CacheRow
    {
        public string Key { get; set; } = "";
        public int Value { get; set; }
    }

    [JsonSerializable(typeof(CacheRow))]
    private partial class IncJsonContext : JsonSerializerContext;
}
