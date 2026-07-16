// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : BoxingAndUnboxing
// Topic id : stage02/section01/boxing_and_unboxing
//
// 步骤 4：装箱/拆箱——隐式堆盒子、精确类型拆箱、隐藏装箱点。

using System.Collections;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section01;

internal static class BoxingAndUnboxing
{
    [LearnTopic("stage02/section01/boxing_and_unboxing")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== BoxingAndUnboxing ===");
        DemoBasicBoxUnbox();
        DemoIndependentCopy();
        DemoExactTypeRequired();
        DemoHiddenBoxing();
        DemoGenericAvoidsBoxing();
        return 0;
    }

    private static void DemoBasicBoxUnbox()
    {
        Console.WriteLine("-- 基本装箱/拆箱 --");
        int i = 123;
        object o = i;           // 隐式装箱
        int j = (int)o;         // 显式拆箱
        Debug.Assert(j == 123);
        Console.WriteLine($"  i={i}, boxed→unboxed j={j}");
    }

    private static void DemoIndependentCopy()
    {
        Console.WriteLine("-- 盒子是独立拷贝 --");
        int i = 123;
        object o = i;
        i = 456;
        Debug.Assert((int)o == 123);
        Console.WriteLine($"  改 i 后 (int)o 仍是 {(int)o}");
    }

    private static void DemoExactTypeRequired()
    {
        Console.WriteLine("-- 拆箱类型必须精确匹配 --");
        object o = 42; // 装的是 int
        int ok = (int)o;
        Debug.Assert(ok == 42);

        try
        {
            long bad = (long)o; // 不能顺便数值转换
            Debug.Assert(false, "should throw");
            _ = bad;
        }
        catch (InvalidCastException)
        {
            Console.WriteLine("  (long)boxed-int → InvalidCastException ✓");
        }

        try
        {
            object? n = null;
            _ = (int)n!;
            Debug.Assert(false);
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("  对 null 拆箱 → NullReferenceException ✓");
        }
    }

    private static void DemoHiddenBoxing()
    {
        Console.WriteLine("-- 隐藏装箱：接口赋值、非泛型集合 --");
        IComparable c = 5; // 值类型 → 接口 = 装箱
        Debug.Assert(c.CompareTo(3) > 0);

        ArrayList list = new();
        list.Add(1); // 装箱
        list.Add(2);
        Debug.Assert((int)list[0]! == 1);
        Console.WriteLine($"  IComparable c=5; ArrayList.Add(1) 都会 box");
    }

    private static void DemoGenericAvoidsBoxing()
    {
        Console.WriteLine("-- 泛型容器消除装箱 --");
        List<int> typed = [1, 2, 3]; // 直接存 int，零装箱
        Debug.Assert(typed[0] == 1);
        Console.WriteLine("  List<int> 不装箱；ArrayList 存 int 会装箱（泛型动机之一）");
    }
}
