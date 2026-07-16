// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第1部分-反射.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section01_Reflection
// Item     : ReflectionAotTrimLimits
// Topic id : stage13/section01/reflection_aot_trim_limits
//
// Lesson: reflection vs static analysis conflict; DAM annotations; prefer source generators.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section01;

internal static class ReflectionAotTrimLimits
{
    [LearnTopic("stage13/section01/reflection_aot_trim_limits")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReflectionAotTrimLimits ===");
        DemoConflict();
        DemoDynamicallyAccessedMembers();
        DemoPreferSourceGenerators();
        return 0;
    }

    private static void DemoConflict()
    {
        Console.WriteLine("-- reflection vs trimmer/AOT --");
        Console.WriteLine("  Trimmer/AOT: static analysis deletes unused types/members.");
        Console.WriteLine("  Reflection: accesses by string/Type at runtime — invisible to analysis.");
        Console.WriteLine("  Result: MissingMethodException etc. after publish/trim.");
        Console.WriteLine("  Worst: Type.GetType(string), GetTypes(), fully dynamic MakeGenericType.");
    }

    private static void DemoDynamicallyAccessedMembers()
    {
        Console.WriteLine("-- [DynamicallyAccessedMembers] keeps needed members --");
        object o = Create(typeof(Service));
        Debug.Assert(o is Service);
        Console.WriteLine($"  Create(typeof(Service)) => {o.GetType().Name}");
        Console.WriteLine("  Annotation propagates along call graph; be precise (too much = bloat).");
    }

    private static void DemoPreferSourceGenerators()
    {
        Console.WriteLine("-- preferred path for AOT --");
        Console.WriteLine("  1) DynamicallyAccessedMembers = patch (keep reflection, annotate)");
        Console.WriteLine("  2) Source generators / UnsafeAccessor = root fix (static code, zero reflect)");
        Console.WriteLine("  Built-ins: STJ source gen, LoggerMessage, GeneratedRegex, LibraryImport");
        Console.WriteLine("  Rule: if compile-time known, generate; reserve reflection for true dynamics.");

        // educational: attribute-driven "would-be generated" pattern
        string jsonish = ManualSerialize(new Point(3, 4));
        Debug.Assert(jsonish.Contains("X", StringComparison.Ordinal));
        Console.WriteLine($"  hand-written static serialize (what a generator emits): {jsonish}");
    }

    private static object Create(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type type)
        => Activator.CreateInstance(type)!;

    private static string ManualSerialize(Point p)
        => $"{{\"X\":{p.X},\"Y\":{p.Y}}}";

    private sealed class Service
    {
        public Service() { }
    }

    private readonly record struct Point(int X, int Y);
}
