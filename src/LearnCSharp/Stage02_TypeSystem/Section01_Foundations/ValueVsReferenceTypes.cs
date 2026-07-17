// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : ValueVsReferenceTypes
// Topic id : stage02/section01/value_vs_reference_types
//
// 步骤 2：值类型 vs 引用类型——赋值/传参语义、struct vs class。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section01;

internal static class ValueVsReferenceTypes
{
    [LearnTopic("stage02/section01/value_vs_reference_types")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ValueVsReferenceTypes ===");
        DemoAssignmentCopy();
        DemoPassByValue();
        DemoDefaultEquality();
        DemoDefaultsAndNull();
        DemoPitfallCppIntuition();
        return 0;
    }

    private static void DemoAssignmentCopy()
    {
        Console.WriteLine("-- 赋值：复制整份 vs 复制引用 --");
        PointV a = new() { X = 1, Y = 2 };
        PointV b = a;
        b.X = 99;
        Debug.Assert(a.X == 1);
        Console.WriteLine($"  struct: a.X={a.X} (b 改了不影响 a)");

        PointR c = new() { X = 1, Y = 2 };
        PointR d = c;
        d.X = 99;
        Debug.Assert(c.X == 99);
        Console.WriteLine($"  class:  c.X={c.X} (d 与 c 同一对象)");
    }

    private static void DemoPassByValue()
    {
        Console.WriteLine("-- 传参默认按值：struct 传副本，class 传引用副本 --");
        PointV v = new() { X = 1, Y = 2 };
        MutateStruct(v);
        Debug.Assert(v.X == 1);

        PointR r = new() { X = 1, Y = 2 };
        MutateClass(r);
        Debug.Assert(r.X == 99);
        Console.WriteLine($"  after MutateStruct: v.X={v.X}; after MutateClass: r.X={r.X}");
    }

    private static void DemoDefaultEquality()
    {
        Console.WriteLine("-- 默认相等：值相等 vs 引用相等 --");
        PointV a1 = new() { X = 1, Y = 2 }, a2 = new() { X = 1, Y = 2 };
        Debug.Assert(a1.Equals(a2));

        PointR c1 = new() { X = 1, Y = 2 }, c2 = new() { X = 1, Y = 2 };
        // class 默认 Equals == 引用相等（未重写）；用 ReferenceEquals 表达同一概念，
        // 避免在未重写 Equals 的类上调用 .Equals() 触发质量门禁告警。
        Debug.Assert(!ReferenceEquals(c1, c2));
        bool classSameRef = ReferenceEquals(c1, c2);
        bool classEqualByDefault = c1 == c2; // 未重载 == → 走 object 引用比较
        Console.WriteLine($"  struct.Equals(值相等)={a1.Equals(a2)}; class ReferenceEquals={classSameRef}, class == {classEqualByDefault} (均为引用比较)");
    }

    private static void DemoDefaultsAndNull()
    {
        Console.WriteLine("-- 默认值与 null --");
        PointV dv = default;
        Debug.Assert(dv.X == 0 && dv.Y == 0);
        // PointV? 才能为 null；裸 struct 不能
        PointR? nr = null;
        Debug.Assert(nr is null);
        Console.WriteLine($"  default(PointV)=({dv.X},{dv.Y}); PointR 可为 null");
    }

    private static void DemoPitfallCppIntuition()
    {
        Console.WriteLine("-- ⚠ C++ 直觉坑：C# class 默认共享 --");
        // C++: class/struct 默认都是值拷贝；C# class = 引用语义
        PointR a = new() { X = 1 };
        PointR b = a;
        b.X = 9;
        Debug.Assert(a.X == 9);
        Console.WriteLine("  SomeClass b = a; b.X=9 → a 也被改（引用共享）");
    }

    private static void MutateStruct(PointV p) => p.X = 99;
    private static void MutateClass(PointR p) => p.X = 99;

    private struct PointV
    {
        public int X, Y;
    }

    private sealed class PointR
    {
        public int X, Y;
    }
}
