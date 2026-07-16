// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第2部分-函数成员与构造.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section02_FunctionMembersAndConstruction
// Item     : Constructors
// Topic id : stage03/section02/constructors
//
// 步骤 3：实例构造、构造链、对象初始化器、主构造(C#12)、partial 构造(C#14)。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section02;

internal static class Constructors
{
    [LearnTopic("stage03/section02/constructors")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Constructors ===");
        DemoCtorChainAndInitializers();
        DemoPrimaryConstructor();
        DemoPrimaryStructAndObjectInit();
        DemoPartialConstructor();
        return 0;
    }

    private static void DemoCtorChainAndInitializers()
    {
        Console.WriteLine("-- 构造链 this + 对象初始化器 --");
        var p = new Person("Ada", "Lovelace", 36);
        Debug.Assert(p.First == "Ada" && p.Last == "Lovelace" && p.Age == 36);
        var p2 = new Person("Grace", "Hopper") { Age = 85 };
        Debug.Assert(p2.Age == 85);
        Console.WriteLine($"  {p.First} {p.Last}, age={p.Age}");
        Console.WriteLine($"  {p2.First} age via initializer={p2.Age}");
    }

    private static void DemoPrimaryConstructor()
    {
        Console.WriteLine("-- 主构造(C#12)：参数全类型可见 --");
        var item = new NamedItem("widget");
        Debug.Assert(item.Name == "widget");
        Debug.Assert(item.Shout() == "WIDGET");
        var student = new Student("Lin", 42);
        Debug.Assert(student.Name == "Lin" && student.Id == 42);
        Console.WriteLine($"  NamedItem={item.Name}, Student={student.Name}#{student.Id}");
    }

    private static void DemoPrimaryStructAndObjectInit()
    {
        Console.WriteLine("-- 主构造 struct + 显式无参 : this --");
        var d = new Distance(3, 4);
        Debug.Assert(Math.Abs(d.Magnitude - 5) < 1e-9);
        var z = new Distance();
        Debug.Assert(Math.Abs(d.Magnitude - 5) < 1e-9);
        Debug.Assert(Math.Abs(z.Magnitude) < 1e-9);
        var r = new MutableRect { Width = 2, Height = 5 };
        Debug.Assert(r.Area == 10);
        Console.WriteLine($"  Distance(3,4).Magnitude={d.Magnitude}, empty={z.Magnitude}");
    }

    private static void DemoPartialConstructor()
    {
        Console.WriteLine("-- partial 实例构造(C#14) --");
        var g = new GeneratedReady("token-1");
        Debug.Assert(g.Token == "token-1");
        Debug.Assert(g.Ready);
        Console.WriteLine($"  GeneratedReady Token={g.Token}, Ready={g.Ready}");
    }

    private sealed class Person
    {
        public string First { get; }
        public string Last { get; }
        public int Age { get; set; }

        public Person(string first, string last)
        {
            First = first;
            Last = last;
        }

        public Person(string first, string last, int age) : this(first, last)
        {
            Age = age;
        }
    }

    private class NamedItem(string name)
    {
        public string Name => name;
        public string Shout() => name.ToUpperInvariant();
    }

    private sealed class Student(string name, int id) : NamedItem(name)
    {
        public int Id => id;
    }

    private readonly struct Distance(double dx, double dy)
    {
        public readonly double Magnitude => Math.Sqrt(dx * dx + dy * dy);
        public Distance() : this(0, 0) { }
    }

    private sealed class MutableRect
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Area => Width * Height;
    }

    private sealed partial class GeneratedReady
    {
        public string Token { get; }
        public bool Ready { get; private set; }
        public partial GeneratedReady(string token);
    }

    private sealed partial class GeneratedReady
    {
        public partial GeneratedReady(string token)
        {
            Token = token;
            Ready = true;
        }
    }
}
