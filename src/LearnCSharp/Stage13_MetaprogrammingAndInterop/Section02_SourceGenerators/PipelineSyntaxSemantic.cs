// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : PipelineSyntaxSemantic
// Topic id : stage13/section02/pipeline_syntax_semantic
//
// Lesson: SyntaxTree vs SemanticModel; cheap predicate → expensive transform; cache iron rules.

using System.Diagnostics;
using System.Text.RegularExpressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section02;

internal static class PipelineSyntaxSemantic
{
    [LearnTopic("stage13/section02/pipeline_syntax_semantic")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== PipelineSyntaxSemantic ===");
        DemoSyntaxVsSemantic();
        DemoTwoPhasePipeline();
        DemoCacheIronRules();
        return 0;
    }

    private static void DemoSyntaxVsSemantic()
    {
        Console.WriteLine("-- SyntaxTree vs SemanticModel --");
        Console.WriteLine("  SyntaxTree: pure structure (ClassDeclarationSyntax, tokens, trivia)");
        Console.WriteLine("    knows \"there is a class named Foo\" — not base types or symbol identity");
        Console.WriteLine("  SemanticModel: symbols (INamedTypeSymbol, IMethodSymbol) — resolved meaning");
        Console.WriteLine("    GetDeclaredSymbol(node) → full type graph, attributes, members");
        Console.WriteLine("  Clang AST + Sema / libTooling ≈ same split.");
    }

    private static void DemoTwoPhasePipeline()
    {
        Console.WriteLine("-- two-phase: cheap syntax filter → semantic extract --");
        // Simulate SyntaxProvider: scan "source files" as strings.
        string[] files =
        [
            "public class Untagged { }",
            "[GenerateHello] public class Greeter { }",
            "public struct Point { }",
            "[GenerateHello] public class Logger { }",
        ];

        int syntaxChecks = 0;
        int semanticExtracts = 0;
        var extracted = new List<string>();

        foreach (string file in files)
        {
            syntaxChecks++;
            // predicate: only class declarations that look attribute-marked (cheap)
            if (!file.Contains("[GenerateHello]", StringComparison.Ordinal) ||
                !file.Contains("class ", StringComparison.Ordinal))
                continue;

            semanticExtracts++;
            Match m = Regex.Match(file, @"class\s+(\w+)");
            if (m.Success)
                extracted.Add(m.Groups[1].Value);
        }

        Debug.Assert(extracted is ["Greeter", "Logger"]);
        Console.WriteLine($"  files={files.Length}, syntaxChecks={syntaxChecks}, semanticExtracts={semanticExtracts}");
        Console.WriteLine($"  extracted: {string.Join(", ", extracted)}");
        Console.WriteLine("  ForAttributeWithMetadataName: skip whole files without the attribute (fast path).");
    }

    private static void DemoCacheIronRules()
    {
        Console.WriteLine("-- cache correctness iron rules --");
        Console.WriteLine("  ✅ IIncrementalGenerator (not classic ISourceGenerator)");
        Console.WriteLine("  ✅ attribute-driven → ForAttributeWithMetadataName");
        Console.WriteLine("  ✅ pipeline models: IEquatable / record (value equality)");
        Console.WriteLine("  ❌ never capture Compilation / ISymbol in transform output");
        Console.WriteLine("     (they change every compile → cache always misses)");
        Console.WriteLine("  ✅ extract minimal strings/names only; static lambdas");

        // Bad model: identity equality on a "symbol-like" wrapper → always miss
        var bad1 = new NonEquatableBag("A");
        var bad2 = new NonEquatableBag("A");
        Debug.Assert(!ReferenceEquals(bad1, bad2));
        Console.WriteLine($"  non-equatable models: equal content but not equal → {bad1.Equals(bad2)} (cache thrash)");

        GoodInfo g1 = new("A", "N");
        GoodInfo g2 = new("A", "N");
        Debug.Assert(g1.Equals(g2));
        Console.WriteLine($"  record models: value equal → {g1.Equals(g2)} (cache can hit)");
    }

    private sealed class NonEquatableBag(string name)
    {
        public string Name { get; } = name;
    }

    private readonly record struct GoodInfo(string Name, string Namespace);
}
