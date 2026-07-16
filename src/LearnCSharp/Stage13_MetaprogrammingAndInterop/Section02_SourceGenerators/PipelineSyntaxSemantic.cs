// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : PipelineSyntaxSemantic
// Topic id : stage13/section02/pipeline_syntax_semantic
//
// Lesson: SyntaxTree vs SemanticModel; cheap predicate → expensive transform; cache iron rules.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section02;

internal static partial class PipelineSyntaxSemantic
{
    [LearnTopic("stage13/section02/pipeline_syntax_semantic")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== PipelineSyntaxSemantic ===");
        DemoSyntaxVsSemantic();
        DemoTwoPhasePipelineMeasured();
        DemoCacheIronRulesAndJson();
        return 0;
    }

    private static void DemoSyntaxVsSemantic()
    {
        Console.WriteLine("-- SyntaxTree vs SemanticModel --");
        Console.WriteLine("  SyntaxTree: structure only; SemanticModel: resolved symbols.");
        Console.WriteLine("  Pipeline: cheap syntax filter → expensive semantic extract.");
    }

    private static void DemoTwoPhasePipelineMeasured()
    {
        Console.WriteLine("-- two-phase pipeline with measured filter savings --");
        string[] files =
        [
            "public class Untagged { }",
            "[GenerateHello] public class Greeter { }",
            "public struct Point { }",
            "[GenerateHello] public class Logger { }",
            "namespace X { class Y { } }",
            "[GenerateHello] public class Metrics { }",
        ];

        int syntaxChecks = 0;
        int semanticExtracts = 0;
        var extracted = new List<string>();

        long t0 = Stopwatch.GetTimestamp();
        foreach (string file in files)
        {
            syntaxChecks++;
            if (!file.Contains("[GenerateHello]", StringComparison.Ordinal) ||
                !file.Contains("class ", StringComparison.Ordinal))
                continue;

            semanticExtracts++;
            Match m = Regex.Match(file, @"class\s+(\w+)");
            if (m.Success)
                extracted.Add(m.Groups[1].Value);
        }

        double us = Stopwatch.GetElapsedTime(t0).TotalMicroseconds;
        Debug.Assert(extracted is ["Greeter", "Logger", "Metrics"]);
        Console.WriteLine($"  files={files.Length}, syntax={syntaxChecks}, semantic={semanticExtracts}, {us:F1}µs");
        Console.WriteLine($"  extracted: {string.Join(", ", extracted)}");
        Debug.Assert(semanticExtracts < syntaxChecks);
    }

    private static void DemoCacheIronRulesAndJson()
    {
        Console.WriteLine("-- cache iron rules + real STJ context --");
        Console.WriteLine("  ✅ IIncrementalGenerator, ForAttributeWithMetadataName, record models");
        Console.WriteLine("  ❌ never capture Compilation/ISymbol in transform output");

        GoodInfo g1 = new("A", "N");
        GoodInfo g2 = new("A", "N");
        Debug.Assert(g1.Equals(g2));
        Console.WriteLine($"  record equality: {g1.Equals(g2)}");

        var node = new PipelineNode { Name = "Greeter", Namespace = "App" };
        string json = JsonSerializer.Serialize(node, PipeJsonContext.Default.PipelineNode);
        PipelineNode? back = JsonSerializer.Deserialize(json, PipeJsonContext.Default.PipelineNode);
        Debug.Assert(back is { Name: "Greeter", Namespace: "App" });
        Console.WriteLine($"  model as STJ source-gen DTO: {json}");
    }

    private readonly record struct GoodInfo(string Name, string Namespace);

    private sealed class PipelineNode
    {
        public string Name { get; set; } = "";
        public string Namespace { get; set; } = "";
    }

    [JsonSerializable(typeof(PipelineNode))]
    private partial class PipeJsonContext : JsonSerializerContext;
}
