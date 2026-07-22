// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : OtherModifiers (四、其他修饰符 — 13 个)
// Topic id : stage08/section01/other_modifiers
//
// static/const/readonly/abstract/sealed/virtual/override/new/event/extern/volatile/unsafe/fixed。

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class OtherModifiers
{
    [LearnTopic("stage08/section01/other_modifiers")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== OtherModifiers ===");
        DemoStaticConstReadonly();
        DemoPolymorphismModifiers();
        DemoEvent();
        DemoVolatileAndExtern();
        DemoUnsafeFixedNote();
        return 0;
    }

    private static void DemoStaticConstReadonly()
    {
        Console.WriteLine("-- static / const / readonly --");
        Debug.Assert(MathUtil.Double(3) == 6);
        Debug.Assert(MathUtil.PiApprox == 3.14);
        ReadonlyHolder r = new ReadonlyHolder(7);
        Debug.Assert(r.Value == 7);
        Debug.Assert(r.Id == 100);
        Console.WriteLine($"  Double(3)={MathUtil.Double(3)}, PiApprox={MathUtil.PiApprox}, Readonly={r.Value}");
    }

    private static void DemoPolymorphismModifiers()
    {
        Console.WriteLine("-- abstract / sealed / virtual / override / new --");
        Shape s = new Circle(2);
        Debug.Assert(Math.Abs(s.Area() - Math.PI * 4) < 1e-9);
        Debug.Assert(s.Tag() == "Circle");
        Base b = new Derived();
        Debug.Assert(b.VirtualName() == "Derived");
        Debug.Assert(b.HiddenName() == "Base"); // new 隐藏，按编译期类型
        Debug.Assert(((Derived)b).HiddenName() == "Derived");
        Debug.Assert(typeof(SealedLeaf).IsSealed);
        Console.WriteLine($"  Area≈{s.Area():F2}, Virtual={b.VirtualName()}, Hidden via Base={b.HiddenName()}");
    }

    private static void DemoEvent()
    {
        Console.WriteLine("-- event --");
        Publisher pub = new Publisher();
        int count = 0;
        pub.Changed += (_, e) => count += e;
        pub.Raise(3);
        pub.Raise(2);
        Debug.Assert(count == 5);
        Console.WriteLine($"  event sum={count}");
    }

    private static void DemoVolatileAndExtern()
    {
        Console.WriteLine("-- volatile / extern --");
        VolatileBox box = new VolatileBox();
        box.Flag = 1;
        Debug.Assert(box.Flag == 1);
        // extern + DllImport：声明外部实现（验证 MethodAttributes.PinvokeImpl 标志）
        MethodInfo? m = typeof(NativeProbe).GetMethod(nameof(NativeProbe.GetTickCount64));
        Debug.Assert(m is not null);
        Debug.Assert((m.Attributes & MethodAttributes.PinvokeImpl) != 0);
        Console.WriteLine($"  volatile Flag={box.Flag}, extern method={m.Name}");
    }

    private static void DemoUnsafeFixedNote()
    {
        Console.WriteLine("-- unsafe / fixed (概念；本项目未开 AllowUnsafeBlocks) --");
        // stackalloc 配 Span 可在安全上下文使用（无需 unsafe 指针语法）
        Span<int> stack = stackalloc int[3];
        stack[0] = 1; stack[1] = 2; stack[2] = 3;
        Debug.Assert(stack.Length == 3 && stack[2] == 3);
        Console.WriteLine("  stackalloc Span 演示栈分配；指针/fixed 需 unsafe 上下文");
    }

    private static class MathUtil
    {
        public const double PiApprox = 3.14;
        public static int Double(int x) => x * 2;
    }

    private sealed class ReadonlyHolder
    {
        public readonly int Id = 100;
        public int Value { get; }
        public ReadonlyHolder(int v) => Value = v;
    }

    private abstract class Shape
    {
        public abstract double Area();
        public virtual string Tag() => "Shape";
    }

    private sealed class Circle(double r) : Shape
    {
        public override double Area() => Math.PI * r * r;
        public override string Tag() => "Circle";
    }

    private class Base
    {
        public virtual string VirtualName() => "Base";
        public string HiddenName() => "Base";
    }

    private sealed class Derived : Base
    {
        public override string VirtualName() => "Derived";
        public new string HiddenName() => "Derived";
    }

    private sealed class SealedLeaf { }

    private sealed class Publisher
    {
        public event EventHandler<int>? Changed;
        public void Raise(int n) => Changed?.Invoke(this, n);
    }

    private sealed class VolatileBox
    {
        public volatile int Flag;
    }

    private static class NativeProbe
    {
        [DllImport("kernel32.dll")]
        public static extern ulong GetTickCount64();
    }
}
