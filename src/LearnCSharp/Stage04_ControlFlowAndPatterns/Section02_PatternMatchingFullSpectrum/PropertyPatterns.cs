// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第2部分-模式匹配全谱.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section02_PatternMatchingFullSpectrum
// Item     : PropertyPatterns
// Topic id : stage04/section02/property_patterns
//
// 步骤 5：属性模式 + C# 10 扩展点记法。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section02;

internal static class PropertyPatterns
{
    [LearnTopic("stage04/section02/property_patterns")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== PropertyPatterns ===");
        DemoBasicProperty();
        DemoExtendedDotNotation();
        DemoMultiProperty();
        DemoExtractAndType();
        DemoNullSafe();
        return 0;
    }

    private static void DemoBasicProperty()
    {
        Console.WriteLine("-- { Prop: 模式 } --");
        Person p = new Person("Ada", 30, new Address("北京", "中国"));
        Debug.Assert(p is { Age: >= 18 });
        Debug.Assert(!(p is { Age: < 18 }));
        Console.WriteLine($"  Age>=18 → {p is { Age: >= 18 }}");
    }

    private static void DemoExtendedDotNotation()
    {
        Console.WriteLine("-- C# 10 嵌套点记法 --");
        Person p = new Person("Bob", 40, new Address("上海", "中国"));
        Debug.Assert(p is { Name.Length: > 0 });
        Debug.Assert(p is { Address.City: "上海" });
        // 旧写法等价
        Debug.Assert(p is { Address: { City: "上海" } });
        Console.WriteLine("  { Address.City: \"上海\" } 等价嵌套属性模式");
    }

    private static void DemoMultiProperty()
    {
        Console.WriteLine("-- 多属性逗号 = and --");
        Person p = new Person("Chen", 70, new Address("广州", "中国"));
        Debug.Assert(p is { Age: >= 65, Address.Country: "中国" });
        Console.WriteLine("  { Age: >=65, Address.Country: \"中国\" }");
    }

    private static void DemoExtractAndType()
    {
        Console.WriteLine("-- 提取 + 类型 + switch --");
        static string Describe(Person p) => p switch
        {
            { Age: < 18 } => "未成年",
            { Age: >= 65, Address.Country: "中国" } => "中国老年人",
            { Name: var n, Age: var a } => $"{n}, {a} 岁",
        };

        Debug.Assert(Describe(new Person("Kid", 10, new Address("x", "y"))) == "未成年");
        Debug.Assert(Describe(new Person("Li", 70, new Address("北京", "中国"))) == "中国老年人");
        Debug.Assert(Describe(new Person("Ada", 30, new Address("Seattle", "US"))) == "Ada, 30 岁");
        Console.WriteLine($"  Ada → {Describe(new Person("Ada", 30, new Address("Seattle", "US")))}");
    }

    private static void DemoNullSafe()
    {
        Console.WriteLine("-- 属性模式对 null 安全 --");
        Person? p = null;
        Debug.Assert(!(p is { Age: > 18 }));
        Console.WriteLine("  null is { Age: >18 } → false，不抛 NRE");
    }

    private sealed record Person(string Name, int Age, Address Address);
    private sealed record Address(string City, string Country);
}
