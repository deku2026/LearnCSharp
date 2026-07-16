// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第3部分-继承多态接口.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section03_InheritancePolymorphismInterfaces
// Item     : InheritanceBasics
// Topic id : stage03/section03/inheritance_basics
//
// 步骤 1：单继承、base 构造/成员、构造顺序、不继承构造函数。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section03;

internal static class InheritanceBasics
{
    [LearnTopic("stage03/section03/inheritance_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== InheritanceBasics ===");
        DemoSingleInheritance();
        DemoBaseCtorOrder();
        DemoBaseMemberAccess();
        DemoEverythingIsObject();
        return 0;
    }

    private static void DemoSingleInheritance()
    {
        Console.WriteLine("-- 单类继承 + 派生扩展 --");
        var d = new Dog("Rex");
        Debug.Assert(d.Name == "Rex");
        Debug.Assert(d.Eat() == "Rex eats");
        Debug.Assert(d.Bark() == "Rex barks");
        Console.WriteLine($"  {d.Eat()}; {d.Bark()}");
    }

    private static void DemoBaseCtorOrder()
    {
        Console.WriteLine("-- 构造顺序：基类先于派生 --");
        CtorTrace.Clear();
        _ = new OrderedChild("c");
        Debug.Assert(CtorTrace.Log is ["Base", "Child"]);
        Console.WriteLine($"  order=[{string.Join(" → ", CtorTrace.Log)}]");
    }

    private static void DemoBaseMemberAccess()
    {
        Console.WriteLine("-- base.成员 --");
        var c = new LabeledChild("kid");
        Debug.Assert(c.Describe() == "Child of Base:kid");
        Console.WriteLine($"  {c.Describe()}");
    }

    private static void DemoEverythingIsObject()
    {
        Console.WriteLine("-- 所有类最终继承 System.Object --");
        var d = new Dog("Spot");
        object o = d;
        Debug.Assert(o.GetType() == typeof(Dog));
        Debug.Assert(d is object);
        Console.WriteLine($"  runtime type={o.GetType().Name}");
    }

    private static class CtorTrace
    {
        public static List<string> Log { get; } = new();
        public static void Clear() => Log.Clear();
        public static void Add(string s) => Log.Add(s);
    }

    private class Animal
    {
        public string Name { get; }
        public Animal(string name) => Name = name;
        public string Eat() => $"{Name} eats";
    }

    private sealed class Dog : Animal
    {
        public Dog(string name) : base(name) { }
        public string Bark() => $"{Name} barks";
    }

    private class OrderedBase
    {
        public OrderedBase() => CtorTrace.Add("Base");
    }

    private sealed class OrderedChild : OrderedBase
    {
        public string Tag { get; }
        public OrderedChild(string tag) : base()
        {
            Tag = tag;
            CtorTrace.Add("Child");
        }
    }

    private class LabeledBase
    {
        public string Label { get; }
        public LabeledBase(string label) => Label = label;
        public virtual string Describe() => $"Base:{Label}";
    }

    private sealed class LabeledChild : LabeledBase
    {
        public LabeledChild(string label) : base(label) { }
        public override string Describe() => $"Child of {base.Describe()}";
    }
}
