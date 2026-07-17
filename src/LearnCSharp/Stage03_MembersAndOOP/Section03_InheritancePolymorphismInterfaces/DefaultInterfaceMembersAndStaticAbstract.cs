// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第3部分-继承多态接口.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section03_InheritancePolymorphismInterfaces
// Item     : DefaultInterfaceMembersAndStaticAbstract
// Topic id : stage03/section03/default_interface_members_and_static_abstract
//
// 步骤 7：默认接口成员(C#8)、静态抽象(C#11)、钻石消歧、接口 vs 抽象类。

using System.Diagnostics;
using System.Numerics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section03;

internal static class DefaultInterfaceMembersAndStaticAbstract
{
    [LearnTopic("stage03/section03/default_interface_members_and_static_abstract")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DefaultInterfaceMembersAndStaticAbstract ===");
        DemoDefaultInterfaceMembers();
        DemoDiamondResolution();
        DemoStaticAbstractGenericMath();
        DemoInterfaceVsAbstractClass();
        return 0;
    }

    private static void DemoDefaultInterfaceMembers()
    {
        Console.WriteLine("-- 默认接口成员(C#8) --");
        IGreeter g = new SimpleGreeter();
        Debug.Assert(g.Greet("Ada") == "Hello, Ada");
        Debug.Assert(g.GreetLoud("Ada") == "HELLO, ADA"); // 默认实现
        Console.WriteLine($"  Greet={g.Greet("Ada")}, GreetLoud={g.GreetLoud("Ada")}");
    }

    private static void DemoDiamondResolution()
    {
        Console.WriteLine("-- 钻石：组合接口提供最具体默认 --");
        ILeftRight x = new DiamondImpl();
        // 通过 ILeftRight 调用走组合接口上的最具体默认
        Debug.Assert(((ILeftRight)x).Name() == "Merged");
        Debug.Assert(((ILeft)x).Name() == "Left");
        Debug.Assert(((IRight)x).Name() == "Right");
        Console.WriteLine($"  ILeftRight.Name={((ILeftRight)x).Name()}, ILeft={((ILeft)x).Name()}, IRight={((IRight)x).Name()}");
    }

    private static void DemoStaticAbstractGenericMath()
    {
        Console.WriteLine("-- static abstract + 泛型数学(C#11) --");
        Debug.Assert(AddAll(1, 2, 3) == 6);
        Debug.Assert(Math.Abs(AddAll(1.5, 2.5) - 4.0) < 1e-9);
        Scale v = new Scale(2);
        Debug.Assert(DoubleMe(v).Value == 4);
        Console.WriteLine($"  AddAll(1,2,3)={AddAll(1, 2, 3)}, DoubleMe(Scale(2))={DoubleMe(v).Value}");
    }

    private static void DemoInterfaceVsAbstractClass()
    {
        Console.WriteLine("-- 接口 vs 抽象类：能力 vs 模板+状态 --");
        ICapability cap = new Tool();
        AbstractTemplate t = new ConcreteTemplate(10);
        Debug.Assert(cap.Run() == "tool");
        Debug.Assert(t.Execute() == "base:10");
        Console.WriteLine($"  interface capability={cap.Run()}, abstract template={t.Execute()}");
    }

    private static T AddAll<T>(params T[] values) where T : INumber<T>
    {
        T sum = T.Zero;
        foreach (T v in values) sum += v;
        return sum;
    }

    private static T DoubleMe<T>(T value) where T : IDoublable<T> => T.Double(value);

    private interface IGreeter
    {
        string Greet(string name);
        string GreetLoud(string name) => Greet(name).ToUpperInvariant();
    }

    private sealed class SimpleGreeter : IGreeter
    {
        public string Greet(string name) => $"Hello, {name}";
    }

    private interface ILeft
    {
        string Name() => "Left";
    }

    private interface IRight
    {
        string Name() => "Right";
    }

    private interface ILeftRight : ILeft, IRight
    {
        // 最具体覆盖：消除钻石二义性
        new string Name() => "Merged";
    }

    private sealed class DiamondImpl : ILeftRight { }

    private interface IDoublable<T> where T : IDoublable<T>
    {
        static abstract T Double(T value);
    }

    private readonly struct Scale(int value) : IDoublable<Scale>
    {
        public int Value { get; } = value;
        public static Scale Double(Scale value) => new(value.Value * 2);
    }

    private interface ICapability
    {
        string Run();
    }

    private sealed class Tool : ICapability
    {
        public string Run() => "tool";
    }

    private abstract class AbstractTemplate
    {
        protected int State { get; }
        protected AbstractTemplate(int state) => State = state;
        public string Execute() => $"base:{State}";
    }

    private sealed class ConcreteTemplate : AbstractTemplate
    {
        public ConcreteTemplate(int state) : base(state) { }
    }
}
