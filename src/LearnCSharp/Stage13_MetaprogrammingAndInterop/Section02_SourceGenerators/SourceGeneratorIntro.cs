// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : SourceGeneratorIntro
// Topic id : stage13/section02/source_generator_intro
//
// Lesson: source generators = Roslyn compile-time codegen (additive, zero runtime cost).

using System.Diagnostics;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section02;

internal static partial class SourceGeneratorIntro
{
    [LearnTopic("stage13/section02/source_generator_intro")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SourceGeneratorIntro ===");
        DemoPipelinePosition();
        DemoRealJsonSourceGen();
        DemoCompileTimePartialVsRuntimeEmit();
        return 0;
    }

    private static void DemoPipelinePosition()
    {
        Console.WriteLine("-- where generators sit --");
        Console.WriteLine("  sources → Roslyn → [Generator] → emit .cs → compile together");
        Console.WriteLine("  Additive only; generator project: netstandard2.0 + CodeAnalysis.");
    }

    private static void DemoRealJsonSourceGen()
    {
        Console.WriteLine("-- real System.Text.Json source generation --");
        IntroDto dto = new IntroDto { Id = 7, Name = "Ada" };
        string json = JsonSerializer.Serialize(dto, IntroJsonContext.Default.IntroDto);
        IntroDto? back = JsonSerializer.Deserialize(json, IntroJsonContext.Default.IntroDto);
        Debug.Assert(back is { Id: 7, Name: "Ada" });
        Console.WriteLine($"  JsonSerializerContext: {json}");
        Console.WriteLine("  Metadata generated at compile time (AOT-friendly).");
    }

    private static void DemoCompileTimePartialVsRuntimeEmit()
    {
        Console.WriteLine("-- compile-time partial vs runtime Reflection.Emit --");
        // Compile-time: hand-authored partial stand-in (what a generator would emit)
        string compileTime = new GeneratedGreeter().Hello();
        Debug.Assert(compileTime.Contains("GeneratedGreeter", StringComparison.Ordinal));
        Console.WriteLine($"  compile-time partial: {compileTime}");

        // Runtime: emit a method that returns 42
        DynamicMethod dm = new DynamicMethod("FortyTwo", typeof(int), Type.EmptyTypes, typeof(SourceGeneratorIntro).Module, true);
        ILGenerator il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldc_I4_S, (byte)42);
        il.Emit(OpCodes.Ret);
        Func<int> fortyTwo = dm.CreateDelegate<Func<int>>();
        int runtime = fortyTwo();
        Debug.Assert(runtime == 42);
        Console.WriteLine($"  runtime Reflection.Emit: FortyTwo()={runtime}");
        Console.WriteLine("  Generators: zero runtime emit cost; Emit: flexible but AOT/trim hostile.");
    }

    private sealed partial class GeneratedGreeter
    {
        public string Hello() => "Hello from GeneratedGreeter!";
    }

    private sealed class IntroDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    [JsonSerializable(typeof(IntroDto))]
    private partial class IntroJsonContext : JsonSerializerContext;
}
