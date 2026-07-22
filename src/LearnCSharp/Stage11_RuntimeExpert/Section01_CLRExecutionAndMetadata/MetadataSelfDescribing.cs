// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第1部分-CLR执行模型与元数据.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section01_CLRExecutionAndMetadata
// Item     : MetadataSelfDescribing
// Topic id : stage11/section01/metadata_self_describing
//
// Lesson: assemblies are self-describing via metadata tables (TypeDef, Method, Field, CustomAttribute).

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section01;

internal static class MetadataSelfDescribing
{
    [LearnTopic("stage11/section01/metadata_self_describing")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MetadataSelfDescribing ===");
        DemoTypeMetadata();
        DemoMethodAndCustomAttribute();
        DemoTokenStyleLookup();
        return 0;
    }

    private static void DemoTypeMetadata()
    {
        Console.WriteLine("-- Type metadata (reflection API over ECMA-335 tables) --");
        Type t = typeof(SampleWidget);
        Console.WriteLine($"  FullName={t.FullName}");
        Console.WriteLine($"  IsClass={t.IsClass}, IsValueType={t.IsValueType}, IsInterface={t.IsInterface}");
        Console.WriteLine($"  BaseType={t.BaseType?.Name}");
        Console.WriteLine($"  Interfaces=[{string.Join(", ", t.GetInterfaces().Select(i => i.Name))}]");
        FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Console.WriteLine($"  Fields: {string.Join(", ", fields.Select(f => $"{f.FieldType.Name} {f.Name}"))}");
        Debug.Assert(t.IsClass);
        Debug.Assert(fields.Any(f => f.Name.Contains("count", StringComparison.OrdinalIgnoreCase)
            || f.Name.Contains("_count", StringComparison.Ordinal)));
    }

    private static void DemoMethodAndCustomAttribute()
    {
        Console.WriteLine("-- Method + CustomAttribute metadata --");
        MethodInfo? m = typeof(SampleWidget).GetMethod(nameof(SampleWidget.Describe));
        Debug.Assert(m is not null);
        Console.WriteLine($"  Method: {m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})");
        object[] attrs = m.GetCustomAttributes(inherit: false);
        Console.WriteLine($"  Custom attributes on Describe: {attrs.Length}");
        foreach (object a in attrs)
            Console.WriteLine($"    [{a.GetType().Name}]");
        Debug.Assert(attrs.OfType<ObsoleteAttribute>().Any());
        ObsoleteAttribute obs = attrs.OfType<ObsoleteAttribute>().First();
        Console.WriteLine($"  Obsolete message: {obs.Message}");
    }

    private static void DemoTokenStyleLookup()
    {
        Console.WriteLine("-- MetadataToken (table row id used by IL) --");
        Type t = typeof(SampleWidget);
        MethodInfo m = t.GetMethod(nameof(SampleWidget.Describe))!;
        Console.WriteLine($"  Type.MetadataToken=0x{t.MetadataToken:X8}");
        Console.WriteLine($"  Method.MetadataToken=0x{m.MetadataToken:X8}");
        Debug.Assert(t.MetadataToken != 0);
        Debug.Assert(m.MetadataToken != 0);
        Console.WriteLine("  IL call/callvirt operands are metadata tokens resolved via these tables.");
    }

    private sealed class SampleWidget : IFormattable
    {
        private readonly int _count;

        public SampleWidget(int count) => _count = count;

        [Obsolete("demo attribute for metadata walk")]
        public string Describe() => $"count={_count}";

#pragma warning disable CS0618 // 演示元数据遍历：故意调用标记 Obsolete 的成员
        public string ToString(string? format, IFormatProvider? formatProvider) => Describe();
#pragma warning restore CS0618
    }
}
