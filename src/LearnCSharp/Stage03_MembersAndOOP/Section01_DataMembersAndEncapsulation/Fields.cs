// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第1部分-数据成员与封装.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section01_DataMembersAndEncapsulation
// Item     : Fields
// Topic id : stage03/section01/fields
//
// 步骤 1：实例字段、字段初始化器、readonly 字段、默认值与封装惯例。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section01;

internal static class Fields
{
    [LearnTopic("stage03/section01/fields")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Fields ===");
        DemoInstanceFieldsAndDefaults();
        DemoFieldInitializers();
        DemoReadonlyFields();
        DemoEncapsulationConvention();
        return 0;
    }

    private static void DemoInstanceFieldsAndDefaults()
    {
        Console.WriteLine("-- 实例字段与默认值 --");
        BlankPerson blank = new BlankPerson();
        Debug.Assert(blank.Age == 0);
        Debug.Assert(blank.Name is null);
        Debug.Assert(blank.Active == false);
        Console.WriteLine($"  未初始化: Age={blank.Age}, Name={blank.Name ?? "null"}, Active={blank.Active}");
    }

    private static void DemoFieldInitializers()
    {
        Console.WriteLine("-- 字段初始化器(构造体之前) --");
        TrackedPerson a = new TrackedPerson("Ada");
        TrackedPerson b = new TrackedPerson("Bob");
        Debug.Assert(a.Id > 0);
        Debug.Assert(b.Id > a.Id);
        Console.WriteLine($"  a.Id={a.Id}, b.Id={b.Id}, Name={a.Name}");
    }

    private static void DemoReadonlyFields()
    {
        Console.WriteLine("-- readonly 字段：声明处或构造内赋值 --");
        Circle c = new Circle(2.5);
        Debug.Assert(Math.Abs(c.Radius - 2.5) < 1e-9);
        Debug.Assert(Math.Abs(c.Area - Math.PI * 2.5 * 2.5) < 1e-9);
        Console.WriteLine($"  radius={c.Radius}, area={c.Area:F4}");
        // c.Grow(); // 若取消注释：无法对 readonly 字段赋值
    }

    private static void DemoEncapsulationConvention()
    {
        Console.WriteLine("-- 封装：字段私有 + 属性/方法公开 --");
        EncapsulatedPerson p = new EncapsulatedPerson("Grace");
        p.Rename("Hopper");
        Debug.Assert(p.Name == "Hopper");
        Console.WriteLine($"  Name via property={p.Name}");
    }

#pragma warning disable CS0649 // 演示字段默认值：字段故意不赋值
    private sealed class BlankPerson
    {
        public int Age;       // 默认 0
        public string? Name;  // 默认 null
        public bool Active;   // 默认 false
    }
#pragma warning restore CS0649

    private sealed class TrackedPerson
    {
        private static int s_next = 1000;
        private readonly int _id = ++s_next; // 字段初始化器
        private string _name;

        public TrackedPerson(string name) => _name = name;

        public int Id => _id;
        public string Name => _name;
    }

    private sealed class Circle
    {
        private readonly double _radius;
        public Circle(double r) => _radius = r;
        public double Radius => _radius;
        public double Area => Math.PI * _radius * _radius;
        // void Grow() => _radius *= 2; // ❌ 构造后不能改 readonly
    }

    private sealed class EncapsulatedPerson
    {
        private string _name; // 惯例 _camelCase 私有字段
        public EncapsulatedPerson(string name) => _name = name;
        public string Name => _name;
        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("empty", nameof(name));
            _name = name;
        }
    }
}
