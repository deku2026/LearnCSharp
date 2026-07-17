// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第3部分-继承多态接口.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section03_InheritancePolymorphismInterfaces
// Item     : NewSealedAbstractAndVersioning
// Topic id : stage03/section03/new_sealed_abstract_and_versioning
//
// 步骤 3：new 隐藏、sealed、abstract 成员、协变返回、版本控制意图。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section03;

internal static class NewSealedAbstractAndVersioning
{
    [LearnTopic("stage03/section03/new_sealed_abstract_and_versioning")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== NewSealedAbstractAndVersioning ===");
        DemoNewHidingVsOverride();
        DemoSealedClassAndOverride();
        DemoAbstractMember();
        DemoCovariantReturn();
        return 0;
    }

    private static void DemoNewHidingVsOverride()
    {
        Console.WriteLine("-- new 隐藏 ≠ override 多态 --");
        DerivedHide d = new();
        Debug.Assert(d.Greet() == "Derived");
        Debug.Assert(((BaseHide)d).Greet() == "Base"); // 编译期类型

        BaseVirt bv = new DerivedVirt();
        Debug.Assert(bv.Greet() == "Derived"); // 运行时类型
        Console.WriteLine("  hide: Derived vs Base by compile-time type");
        Console.WriteLine("  override: always runtime type");
    }

    private static void DemoSealedClassAndOverride()
    {
        Console.WriteLine("-- sealed class / sealed override --");
        Mid m = new Leaf();
        Debug.Assert(m.M() == "Leaf");
        // class X : Final { } // ❌ sealed class
        // class Y : Leaf { public override string M() => ""; } // ❌ sealed override
        Console.WriteLine($"  Leaf.M via Mid={m.M()}");
        Console.WriteLine($"  Final.Tag={new Final().Tag}");
    }

    private static void DemoAbstractMember()
    {
        Console.WriteLine("-- abstract 成员强制 override --");
        AbstractShape s = new ConcreteCircle(2);
        Debug.Assert(Math.Abs(s.Area() - Math.PI * 4) < 1e-9);
        Debug.Assert(s.Print().Contains("Area="));
        Console.WriteLine($"  {s.Print()}");
    }

    private static void DemoCovariantReturn()
    {
        Console.WriteLine("-- 协变返回类型(C#9) --");
        Animal a = new Dog();
        Animal cloneA = a.Clone();
        Dog cloneD = ((Dog)a).Clone(); // override 返回 Dog
        Debug.Assert(cloneA is Dog);
        Debug.Assert(cloneD is Dog);
        Console.WriteLine($"  Dog.Clone() returns {cloneD.GetType().Name} without cast on Dog ref");
    }

    private class BaseHide
    {
        public string Greet() => "Base";
    }

    private sealed class DerivedHide : BaseHide
    {
        public new string Greet() => "Derived";
    }

    private class BaseVirt
    {
        public virtual string Greet() => "Base";
    }

    private sealed class DerivedVirt : BaseVirt
    {
        public override string Greet() => "Derived";
    }

    private sealed class Final
    {
        public string Tag => "final";
    }

    private class Root
    {
        public virtual string M() => "Root";
    }

    private class Mid : Root
    {
        public override string M() => "Mid";
    }

    private sealed class Leaf : Mid
    {
        public sealed override string M() => "Leaf";
    }

    private abstract class AbstractShape
    {
        public abstract double Area();
        public string Print() => $"Area={Area():F2}";
    }

    private sealed class ConcreteCircle : AbstractShape
    {
        public double R { get; }
        public ConcreteCircle(double r) => R = r;
        public override double Area() => Math.PI * R * R;
    }

    private class Animal
    {
        public virtual Animal Clone() => new Animal();
    }

    private sealed class Dog : Animal
    {
        public override Dog Clone() => new Dog();
    }
}
