// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第3部分-继承多态接口.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section03_InheritancePolymorphismInterfaces
// Item     : PolymorphismVirtualOverride
// Topic id : stage03/section03/polymorphism_virtual_override
//
// 步骤 2：virtual/override、运行时分派、无切片、override 强制。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section03;

internal static class PolymorphismVirtualOverride
{
    [LearnTopic("stage03/section03/polymorphism_virtual_override")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== PolymorphismVirtualOverride ===");
        DemoVirtualDispatch();
        DemoListOfBase();
        DemoNoSlicing();
        DemoNonVirtualIsStaticBind();
        return 0;
    }

    private static void DemoVirtualDispatch()
    {
        Console.WriteLine("-- 基类引用 → 运行时类型方法 --");
        Shape s = new Circle(2);
        double area = s.Area();
        Debug.Assert(Math.Abs(area - Math.PI * 4) < 1e-9);
        Console.WriteLine($"  Shape s = Circle(2); s.Area()={area:F4}");
    }

    private static void DemoListOfBase()
    {
        Console.WriteLine("-- List<Shape> 多态遍历 --");
        List<Shape> shapes = [new Circle(1), new Rect(3, 4), new Shape()];
        double[] areas = shapes.Select(x => x.Area()).ToArray();
        Debug.Assert(Math.Abs(areas[0] - Math.PI) < 1e-9);
        Debug.Assert(Math.Abs(areas[1] - 12) < 1e-9);
        Debug.Assert(areas[2] == 0);
        Console.WriteLine($"  areas=[{string.Join(", ", areas.Select(a => a.ToString("F2")))}]");
    }

    private static void DemoNoSlicing()
    {
        Console.WriteLine("-- 引用赋值不切片(对比 C++ 按值) --");
        Circle c = new(3);
        Shape s = c; // 引用，仍指向完整 Circle
        Debug.Assert(s is Circle);
        Debug.Assert(Math.Abs(s.Area() - Math.PI * 9) < 1e-9);
        Console.WriteLine($"  s is Circle={s is Circle}, Area={s.Area():F4}");
    }

    private static void DemoNonVirtualIsStaticBind()
    {
        Console.WriteLine("-- 非虚方法：编译期类型绑定 --");
        Shape s = new Circle(1);
        Debug.Assert(s.Kind() == "Shape"); // 非虚
        Debug.Assert(((Circle)s).Kind() == "Circle");
        Console.WriteLine($"  s.Kind()={s.Kind()} (non-virtual), ((Circle)s).Kind()=Circle");
    }

    private class Shape
    {
        public virtual double Area() => 0;
        public virtual string Describe() => $"Shape, area={Area()}";
        public string Kind() => "Shape"; // 非虚
    }

    private sealed class Circle : Shape
    {
        public double Radius { get; }
        public Circle(double r) => Radius = r;
        public override double Area() => Math.PI * Radius * Radius;
        public new string Kind() => "Circle";
    }

    private sealed class Rect : Shape
    {
        public double W { get; }
        public double H { get; }
        public Rect(double w, double h) => (W, H) = (w, h);
        public override double Area() => W * H;
    }
}
