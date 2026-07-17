// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第1部分-反射.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section01_Reflection
// Item     : ReflectionBasics
// Topic id : stage13/section01/reflection_basics
//
// Lesson: Reflection = runtime API over assembly metadata (Type/MemberInfo).

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section01;

internal static class ReflectionBasics
{
    [LearnTopic("stage13/section01/reflection_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReflectionBasics ===");
        DemoMetadataEntryPoints();
        DemoIntrospection();
        DemoWhatReflectionEnables();
        return 0;
    }

    private static void DemoMetadataEntryPoints()
    {
        Console.WriteLine("-- Type / Assembly / Module (metadata entry) --");
        Type t = typeof(string);
        Assembly asm = t.Assembly;
        Module mod = t.Module;
        Debug.Assert(t.FullName == "System.String");
        Debug.Assert(asm.GetName().Name is not null);
        Console.WriteLine($"  Type={t.FullName}");
        Console.WriteLine($"  Assembly={asm.GetName().Name}");
        Console.WriteLine($"  Module={mod.Name}");
        Console.WriteLine("  Reflection reads TypeDef/MethodDef tables that CLR keeps at runtime.");
    }

    private static void DemoIntrospection()
    {
        Console.WriteLine("-- introspection: list members --");
        Type t = typeof(string);
        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        Debug.Assert(methods.Length > 0 && props.Length > 0);
        Console.WriteLine($"  string public instance methods ≈ {methods.Length}");
        Console.WriteLine($"  string public instance properties ≈ {props.Length}");
        Console.WriteLine($"  sample methods: {string.Join(", ", methods.Take(5).Select(m => m.Name))}");
        Console.WriteLine("  C++ RTTI cannot enumerate members; .NET metadata makes this free.");
    }

    private static void DemoWhatReflectionEnables()
    {
        Console.WriteLine("-- what reflection enables --");
        Console.WriteLine("  1) inspect types/members/attributes at runtime");
        Console.WriteLine("  2) create instances without a compile-time type name");
        Console.WriteLine("  3) invoke methods / get-set fields by name");
        Console.WriteLine("  4) build closed generics (MakeGenericType)");
        Console.WriteLine("  → serializers, DI, ORM, test discovery, plugins");
    }
}
