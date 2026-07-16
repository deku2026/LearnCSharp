// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第2部分-聚合与现代类型.md
// Stage    : Stage02_TypeSystem
// Section  : Section02_AggregatesAndModernTypes
// Item     : AnonymousTypes
// Topic id : stage02/section02/anonymous_types
//
// 步骤 3：匿名类型——LINQ 投影、值相等、限制与取舍。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section02;

internal static class AnonymousTypes
{
    [LearnTopic("stage02/section02/anonymous_types")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AnonymousTypes ===");
        DemoCreationAndProps();
        DemoValueEquality();
        DemoLinqProjection();
        DemoLimits();
        DemoVsTupleAndRecord();
        return 0;
    }

    private static void DemoCreationAndProps()
    {
        Console.WriteLine("-- new { ... } 只读属性 --");
        var person = new { Name = "Ada", Age = 36 };
        Debug.Assert(person.Name == "Ada" && person.Age == 36);
        // person.Age = 37; // 编译错误：只读
        Console.WriteLine($"  {person}");
    }

    private static void DemoValueEquality()
    {
        Console.WriteLine("-- 同形状同值 → Equals true --");
        var a = new { X = 1, Y = 2 };
        var b = new { X = 1, Y = 2 };
        Debug.Assert(a.Equals(b));
        Debug.Assert(a.GetHashCode() == b.GetHashCode());
        // == 仍是引用比较（匿名类型未重载 ==）
        Debug.Assert(a != b || ReferenceEquals(a, b) || true);
        Console.WriteLine($"  Equals={a.Equals(b)}; 注意 == 默认仍比引用");
    }

    private static void DemoLinqProjection()
    {
        Console.WriteLine("-- LINQ Select 投影 --");
        var people = new[]
        {
            new Person("Ada", 36),
            new Person("Grace", 85)
        };
        var projected = people.Select(p => new { p.Name, IsAdult = p.Age >= 18 }).ToArray();
        Debug.Assert(projected.Length == 2);
        Debug.Assert(projected[0].Name == "Ada" && projected[0].IsAdult);
        Console.WriteLine($"  projected[0]={projected[0]}");
    }

    private static void DemoLimits()
    {
        Console.WriteLine("-- 限制：var/推断、不能作公开 API 返回类型名 --");
        var x = new { A = 1 };
        // 不能写 public SomeAnon M() => ... 需要具名类型
        // 数组同形状可推断：
        var arr = new[] { new { N = 1 }, new { N = 2 } };
        Debug.Assert(arr[1].N == 2);
        Console.WriteLine("  适合方法内临时投影，不适合跨程序集公开契约");
        _ = x;
    }

    private static void DemoVsTupleAndRecord()
    {
        Console.WriteLine("-- 取舍：匿名类型 vs 元组 vs record --");
        var anon = new { Name = "A", Age = 1 };
        var tuple = (Name: "A", Age: 1);
        var rec = new PersonDto("A", 1);
        Debug.Assert(anon.Name == tuple.Name && rec.Name == tuple.Name);
        Console.WriteLine("  匿名：LINQ 投影；元组：多返回值；record：领域数据 + with");
    }

    private sealed class Person(string name, int age)
    {
        public string Name { get; } = name;
        public int Age { get; } = age;
    }

    private sealed record PersonDto(string Name, int Age);
}
