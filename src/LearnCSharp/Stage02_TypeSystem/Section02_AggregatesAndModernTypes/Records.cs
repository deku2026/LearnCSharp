// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第2部分-聚合与现代类型.md
// Stage    : Stage02_TypeSystem
// Section  : Section02_AggregatesAndModernTypes
// Item     : Records
// Topic id : stage02/section02/records
//
// 步骤 4：record class/struct、值相等、with、positional、init。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section02;

internal static class Records
{
    [LearnTopic("stage02/section02/records")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Records ===");
        DemoValueEquality();
        DemoWithNonDestructive();
        DemoPositionalAndDeconstruct();
        DemoRecordStruct();
        DemoInheritanceNondestructive();
        return 0;
    }

    private static void DemoValueEquality()
    {
        Console.WriteLine("-- record 默认值相等 --");
        var a = new Person("Ada", 36);
        var b = new Person("Ada", 36);
        var c = new Person("Grace", 85);
        Debug.Assert(a == b);
        Debug.Assert(a.Equals(b));
        Debug.Assert(a != c);
        Debug.Assert(!ReferenceEquals(a, b));
        Console.WriteLine($"  a==b={a == b}; ToString={a}");
    }

    private static void DemoWithNonDestructive()
    {
        Console.WriteLine("-- with 非破坏性修改 --");
        var p = new Person("Ada", 36);
        var older = p with { Age = 37 };
        Debug.Assert(p.Age == 36 && older.Age == 37);
        Debug.Assert(p.Name == older.Name);
        Console.WriteLine($"  p={p}; older={older}");
    }

    private static void DemoPositionalAndDeconstruct()
    {
        Console.WriteLine("-- positional record 自动 Deconstruct --");
        var p = new Person("Ada", 36);
        var (name, age) = p;
        Debug.Assert(name == "Ada" && age == 36);

        var mutableInit = new MutablePoint { X = 1, Y = 2 };
        Debug.Assert(mutableInit.X == 1);
        // mutableInit.X = 3; // init-only 之后只读
        Console.WriteLine($"  deconstruct=({name},{age})");
    }

    private static void DemoRecordStruct()
    {
        Console.WriteLine("-- record struct：值类型 + 值相等 --");
        var a = new Point(1, 2);
        var b = new Point(1, 2);
        Debug.Assert(a == b);
        var c = a with { X = 9 };
        Debug.Assert(a.X == 1 && c.X == 9);
        Console.WriteLine($"  Point record struct a==b={a == b}");
    }

    private static void DemoInheritanceNondestructive()
    {
        Console.WriteLine("-- record 继承与 with 保持运行时类型 --");
        Person p = new Student("Ada", 36, "MIT");
        Person q = p with { Age = 37 };
        Debug.Assert(q is Student s && s.School == "MIT" && s.Age == 37);
        Console.WriteLine($"  with 后类型={q.GetType().Name}");
    }

    private record Person(string Name, int Age);

    private record Student(string Name, int Age, string School) : Person(Name, Age);

    private readonly record struct Point(int X, int Y);

    private sealed class MutablePoint
    {
        public int X { get; init; }
        public int Y { get; init; }
    }
}
