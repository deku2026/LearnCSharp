// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : CommonTypeSystem
// Topic id : stage02/section01/common_type_system
//
// 步骤 1：统一类型系统(CTS)——万物皆 object，值/引用二分。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section01;

internal static class CommonTypeSystem
{
    [LearnTopic("stage02/section01/common_type_system")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CommonTypeSystem ===");
        DemoEverythingIsObject();
        DemoValueTypeHierarchy();
        DemoReferenceTypeHierarchy();
        DemoGetTypeChains();
        DemoSealedValueTypes();
        return 0;
    }

    private static void DemoEverythingIsObject()
    {
        Console.WriteLine("-- 万物皆 object --");
        object box = 42;
        Debug.Assert(box is int);
        Debug.Assert(42.ToString() == "42");
        Debug.Assert(42.GetType() == typeof(int));
        Debug.Assert(typeof(int) == typeof(System.Int32));
        Console.WriteLine($"  42.GetType()={42.GetType()}, box type={box.GetType()}");
    }

    private static void DemoValueTypeHierarchy()
    {
        Console.WriteLine("-- 值类型继承链 --");
        // int → ValueType → Object
        Type t = typeof(int);
        Debug.Assert(t.IsValueType);
        Debug.Assert(t.BaseType == typeof(ValueType));
        Debug.Assert(typeof(ValueType).BaseType == typeof(object));
        Debug.Assert(typeof(ValueType).IsClass); // ValueType 本身是 class(引用类型)
        Console.WriteLine($"  int.BaseType={t.BaseType!.Name}, ValueType.IsClass={typeof(ValueType).IsClass}");

        Debug.Assert(typeof(double).IsValueType);
        Debug.Assert(typeof(bool).IsValueType);
        Debug.Assert(typeof(char).IsValueType);
        Debug.Assert(typeof(DayOfWeek).IsEnum && typeof(DayOfWeek).IsValueType);
        Debug.Assert(typeof(PointV).IsValueType);
    }

    private static void DemoReferenceTypeHierarchy()
    {
        Console.WriteLine("-- 引用类型挂在 object 下 --");
        Debug.Assert(typeof(string).IsClass);
        Debug.Assert(typeof(string).BaseType == typeof(object));
        Debug.Assert(typeof(PointR).IsClass);
        Debug.Assert(typeof(PointR).BaseType == typeof(object));
        Debug.Assert(typeof(int[]).IsArray);
        Debug.Assert(typeof(Action).IsSubclassOf(typeof(Delegate)) || typeof(Action).BaseType == typeof(MulticastDelegate));
        Console.WriteLine($"  string.BaseType={typeof(string).BaseType!.Name}");
    }

    private static void DemoGetTypeChains()
    {
        Console.WriteLine("-- GetType 练习链 --");
        int n = 1;
        string s = "x";
        PointR p = new PointR { X = 1 };

        PrintChain(n);
        PrintChain(s);
        PrintChain(p);

        Debug.Assert(n.GetType() == typeof(int));
        Debug.Assert(s.GetType() == typeof(string));
        Debug.Assert(p.GetType() == typeof(PointR));
    }

    private static void DemoSealedValueTypes()
    {
        Console.WriteLine("-- 值类型隐式 sealed --");
        Debug.Assert(typeof(int).IsSealed);
        Debug.Assert(typeof(PointV).IsSealed);
        // class Bad : PointV { } // 编译错误：不能从值类型派生
        Console.WriteLine("  值类型不可被继承；ValueType 自身却是 class");
    }

    private static void PrintChain(object o)
    {
        Type t = o.GetType();
        Console.WriteLine($"  {t.Name} → {t.BaseType?.Name ?? "(root)"}");
    }

#pragma warning disable CS0649 // 演示默认值初始化：字段故意不赋值
    private struct PointV
    {
        public int X, Y;
    }

    private sealed class PointR
    {
        public int X, Y;
    }
#pragma warning restore CS0649
}
