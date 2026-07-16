// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : SourceGeneratorIntro
// Topic id : stage13/section02/source_generator_intro
//
// Lesson: source generators = Roslyn compile-time codegen (additive, zero runtime cost).

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section02;

internal static class SourceGeneratorIntro
{
    [LearnTopic("stage13/section02/source_generator_intro")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SourceGeneratorIntro ===");
        DemoPipelinePosition();
        DemoReflectionVsGenerator();
        DemoPartialPattern();
        return 0;
    }

    private static void DemoPipelinePosition()
    {
        Console.WriteLine("-- where generators sit --");
        Console.WriteLine("  your sources → Roslyn → [Generator] reads Syntax/Semantic");
        Console.WriteLine("                → emits extra .cs → compile together → assembly");
        Console.WriteLine("  Additive only (cannot edit existing files; interceptors are a newer exception).");
        Console.WriteLine("  Generator project: netstandard2.0 + Microsoft.CodeAnalysis (analyzer ref).");
    }

    private static void DemoReflectionVsGenerator()
    {
        Console.WriteLine("-- reflection vs source generator --");
        Console.WriteLine("  Reflection: runtime metadata + dynamic ops → flexible, slow, AOT-hostile");
        Console.WriteLine("  Generator:  compile-time inspect + emit static C# → zero runtime magic, AOT-ok");
        Console.WriteLine("  Qt moc analogy: external tool reads Q_OBJECT headers, emits C++ — same idea.");
    }

    private static void DemoPartialPattern()
    {
        Console.WriteLine("-- partial class/method pattern (what generators fill) --");
        // Educational stand-in: hand-written "generated" half of a partial type.
        var greeter = new GeneratedGreeter();
        string msg = greeter.Hello();
        Debug.Assert(msg.Contains("GeneratedGreeter", StringComparison.Ordinal));
        Console.WriteLine($"  partial half (simulated generator output): {msg}");
        Console.WriteLine("  You declare partial; generator adds the other half at compile time.");
    }

    // Simulates what a generator would emit for: [GenerateHello] partial class GeneratedGreeter
    private sealed partial class GeneratedGreeter
    {
        public string Hello() => "Hello from GeneratedGreeter!";
    }
}
