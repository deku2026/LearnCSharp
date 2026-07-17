// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第3部分-泛型与泛型数学.md
// Stage    : Stage02_TypeSystem
// Section  : Section03_GenericsAndGenericMath
// Item     : ReifiedVsTemplateMonomorphization
// Topic id : stage02/section03/reified_vs_template_monomorphization
//
// 步骤 3：reified 泛型 vs C++ 模板单态化——运行时类型信息与 JIT 共享/特化。

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section03;

internal static class ReifiedVsTemplateMonomorphization
{
    [LearnTopic("stage02/section03/reified_vs_template_monomorphization")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReifiedVsTemplateMonomorphization ===");
        DemoRuntimeTypeInfo();
        DemoValueTypeNoBoxing();
        DemoReferenceCodeSharingConcept();
        DemoConstraintCheckedAtDefinition();
        DemoComparisonTable();
        return 0;
    }

    private static void DemoRuntimeTypeInfo()
    {
        Console.WriteLine("-- reified：运行时保留类型参数 --");
        List<int> listInt = new List<int>();
        List<string> listStr = new List<string>();
        Type tInt = listInt.GetType();
        Type tStr = listStr.GetType();
        Debug.Assert(tInt.IsGenericType);
        Debug.Assert(tInt.GetGenericArguments()[0] == typeof(int));
        Debug.Assert(tStr.GetGenericArguments()[0] == typeof(string));
        Debug.Assert(tInt != tStr);
        Console.WriteLine($"  List<int> arg={tInt.GetGenericArguments()[0].Name}");
        Console.WriteLine($"  List<string> arg={tStr.GetGenericArguments()[0].Name}");
        Console.WriteLine("  C++ 模板实例化后类型参数在运行时消失；C# 反射仍可见");
    }

    private static void DemoValueTypeNoBoxing()
    {
        Console.WriteLine("-- 值类型 JIT 特化 → 零装箱（可测分配） --");
        Holder<int> holder = new Holder<int>(42);
        Debug.Assert(holder.Value == 42);
        Debug.Assert(holder.GetType().GetGenericArguments()[0] == typeof(int));

        // object[] 写入 int → 每次装箱；List<int>/int[] 写入 → 不装箱
        long boxBytes = MeasureAlloc(static () =>
        {
            object[] objs = new object[64];
            for (int i = 0; i < objs.Length; i++)
                objs[i] = i; // box
            Consume(objs);
        });
        long genericBytes = MeasureAlloc(static () =>
        {
            List<int> list = new List<int>(64);
            for (int i = 0; i < 64; i++)
                list.Add(i); // 无装箱
            Consume(list);
        });
        Debug.Assert(boxBytes > genericBytes);
        Console.WriteLine($"  object[] 装箱分配≈{boxBytes}B；List<int> 添加≈{genericBytes}B（含列表自身）");
        Console.WriteLine("  每个不同值类型一份特化机器码（JIT monomorphization）");
    }

    private static void DemoReferenceCodeSharingConcept()
    {
        Console.WriteLine("-- 引用类型共享一份本地代码（概念） --");
        Holder<string> a = new Holder<string>("x");
        Holder<object> b = new Holder<object>("y");
        Debug.Assert(a.Value == "x");
        Debug.Assert(b.Value is string);
        // 引用都是指针大小 → 机器码可共享；类型元数据仍各自不同
        Debug.Assert(a.GetType() != b.GetType());
        Console.WriteLine("  List<string>/List<object> 共用处理引用的机器码，但 Type 不同");
    }

    private static void DemoConstraintCheckedAtDefinition()
    {
        Console.WriteLine("-- C#：定义时检查；C++：实例化时鸭子检查 --");
        // 无约束时只能当 object 用；有约束才能 CompareTo
        Debug.Assert(Max(1, 2) == 2);
        // 不能在无约束 T 上写 a + b（需泛型数学 static abstract）
        Console.WriteLine("  where 在泛型定义处验证；模板错误常在实例化点爆炸");
    }

    private static void DemoComparisonTable()
    {
        Console.WriteLine("-- 对照速记 --");
        Console.WriteLine("  | 维度 | C# 泛型 | C++ 模板 |");
        Console.WriteLine("  | 何时特化 | JIT 运行时 | 编译期 |");
        Console.WriteLine("  | 值类型 | 每类型特化 | 每类型单态化 |");
        Console.WriteLine("  | 引用类型 | 代码共享 | 也单态化 |");
        Console.WriteLine("  | 运行时类型 | reified 可反射 | 擦掉 |");
        Console.WriteLine("  | 约束 | 显式 where | 鸭子/concepts |");
        // 可观察差异：非泛型 ArrayList.Add(int) 装箱；List<int> 不装箱
        long arrayListBox = MeasureAlloc(static () =>
        {
            ArrayList al = new ArrayList(32);
            for (int i = 0; i < 32; i++)
                al.Add(i);
            Consume(al);
        });
        long listNoBox = MeasureAlloc(static () =>
        {
            List<int> list = new List<int>(32);
            for (int i = 0; i < 32; i++)
                list.Add(i);
            Consume(list);
        });
        Debug.Assert(arrayListBox > listNoBox);
        Console.WriteLine($"  ArrayList 装箱≈{arrayListBox}B vs List<int>≈{listNoBox}B");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long MeasureAlloc(Action action)
    {
        // 预热一次，降低 JIT 干扰
        action();
        long before = GC.GetAllocatedBytesForCurrentThread();
        action();
        long after = GC.GetAllocatedBytesForCurrentThread();
        return after - before;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Consume(object o) => GC.KeepAlive(o);

    private static T Max<T>(T a, T b) where T : IComparable<T>
        => a.CompareTo(b) >= 0 ? a : b;

    private sealed class Holder<T>(T value)
    {
        public T Value { get; } = value;
    }
}
