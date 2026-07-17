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
using System.Reflection;
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
        // The "worst" fully-dynamic reflection the doc flags: type loaded from a string.
        // This works now (non-trimmed) but would emit IL2xxx warnings / throw under AOT publish.
        Type? t = Type.GetType("System.Text.StringBuilder, System.Runtime");
        Debug.Assert(t is not null, "Type.GetType(string) works in a non-trimmed build");
        object? sb = Activator.CreateInstance(t);
        Debug.Assert(sb is not null);
        Console.WriteLine($"  Type.GetType(\"...StringBuilder...\") → {t!.Name}; Activator.CreateInstance → {sb!.GetType().Name}");
        Console.WriteLine("  Under PublishAot/trim this would warn IL2075 / throw MissingMethodException.");
    }

    private static void DemoDynamicallyAccessedMembers()
    {
        Console.WriteLine("-- [DynamicallyAccessedMembers] keeps needed members --");
        object o = Create(typeof(Service));
        Debug.Assert(o is Service);
        Console.WriteLine($"  Create(typeof(Service)) => {o.GetType().Name}");

        // Observable proof the annotation is present: reflect-read the attribute off the
        // parameter so the otherwise-invisible trimmer contract becomes visible at runtime.
        MethodInfo create = typeof(ReflectionAotTrimLimits).GetMethod(nameof(Create), BindingFlags.NonPublic | BindingFlags.Static)!;
        ParameterInfo p = create.GetParameters()[0];
        IList<CustomAttributeData> attrs = p.GetCustomAttributesData();
        bool hasDam = attrs.Any(a => a.AttributeType == typeof(DynamicallyAccessedMembersAttribute));
        Console.WriteLine($"  Create parameter has [DynamicallyAccessedMembers]: {hasDam}");
        Debug.Assert(hasDam, "the parameter must carry the DAM annotation");
        if (hasDam)
        {
            DynamicallyAccessedMembersAttribute dam = (DynamicallyAccessedMembersAttribute)p.GetCustomAttributes(typeof(DynamicallyAccessedMembersAttribute), false)[0];
            Console.WriteLine($"  MemberTypes requested: {dam.MemberTypes}");
        }

        // Contrast: an un-annotated Create also works now, but the trimmer would NOT keep
        // the constructor → MissingMethodException after trimming. Both succeed identically
        // here; the annotation only matters at publish time (made concrete as text).
        object o2 = CreateUnannotated(typeof(Service));
        Debug.Assert(o2 is Service);
        Console.WriteLine($"  CreateUnannotated(typeof(Service)) => {o2.GetType().Name} (works now; trimmer would drop the ctor)");
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

    // No DAM annotation → the trimmer is free to delete the public ctor under AOT/trim.
    private static object CreateUnannotated(Type type) => Activator.CreateInstance(type)!;

    private static string ManualSerialize(Point p)
        => $"{{\"X\":{p.X},\"Y\":{p.Y}}}";

    private sealed class Service
    {
        public Service() { }
    }

    private readonly record struct Point(int X, int Y);
}
