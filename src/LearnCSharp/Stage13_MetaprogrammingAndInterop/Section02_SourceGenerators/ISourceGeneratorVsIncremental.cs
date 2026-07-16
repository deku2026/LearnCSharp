// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : ISourceGeneratorVsIncremental
// Topic id : stage13/section02/isourcegenerator_vs_incremental
//
// Lesson: classic full recompute vs incremental pipeline + caching (IDE keystroke cost).

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section02;

internal static class ISourceGeneratorVsIncremental
{
    [LearnTopic("stage13/section02/isourcegenerator_vs_incremental")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ISourceGeneratorVsIncremental ===");
        DemoApiComparison();
        DemoWhyIncremental();
        DemoSkeletonConcept();
        return 0;
    }

    private static void DemoApiComparison()
    {
        Console.WriteLine("-- two generations of API --");
        Console.WriteLine("  ISourceGenerator (.NET 5, obsolete):");
        Console.WriteLine("    single Execute(full Compilation) — full recompute every compile");
        Console.WriteLine("    IDE keystrokes re-run everything → lag on large solutions");
        Console.WriteLine("  IIncrementalGenerator (Roslyn 4+, required):");
        Console.WriteLine("    declare pipeline of providers + transforms; Roslyn caches stages");
        Console.WriteLine("    only re-run stages whose inputs actually changed");
    }

    private static void DemoWhyIncremental()
    {
        Console.WriteLine("-- why incremental (simulated cache) --");
        var cache = new Dictionary<string, string>(StringComparer.Ordinal);
        string inputA = "class A";
        string inputB = "class B";

        string RunStage(string key, string input)
        {
            if (cache.TryGetValue(key, out string? hit) && hit == input)
            {
                Console.WriteLine($"  cache HIT for {key}");
                return "cached-output";
            }

            cache[key] = input;
            Console.WriteLine($"  cache MISS for {key} → recompute");
            return "fresh-output";
        }

        string r1 = RunStage("A", inputA);
        string r2 = RunStage("A", inputA); // hit
        string r3 = RunStage("B", inputB);
        string r4 = RunStage("A", "class A changed"); // miss
        Debug.Assert(r1 == "fresh-output" && r2 == "cached-output");
        Debug.Assert(r3 == "fresh-output" && r4 == "fresh-output");
        Console.WriteLine("  Unrelated edits leave other stages cached (ccache/Ninja intuition).");
    }

    private static void DemoSkeletonConcept()
    {
        Console.WriteLine("-- minimal IIncrementalGenerator shape (educational, not a real generator project) --");
        Console.WriteLine("  [Generator] class X : IIncrementalGenerator {");
        Console.WriteLine("    void Initialize(IncrementalGeneratorInitializationContext ctx) {");
        Console.WriteLine("      // 1) RegisterPostInitializationOutput → emit marker attribute");
        Console.WriteLine("      // 2) SyntaxProvider.ForAttributeWithMetadataName → ClassInfo records");
        Console.WriteLine("      // 3) RegisterSourceOutput → AddSource(\"{Name}.g.cs\", ...)");
        Console.WriteLine("    }");
        Console.WriteLine("  }");
        Console.WriteLine("  ClassInfo must be value-equal (record) so cache comparisons work.");

        ClassInfo a = new("Foo", "MyApp");
        ClassInfo b = new("Foo", "MyApp");
        Debug.Assert(a.Equals(b));
        Console.WriteLine($"  sample ClassInfo equality: {a.Equals(b)}");
    }

    private readonly record struct ClassInfo(string Name, string Namespace);
}
