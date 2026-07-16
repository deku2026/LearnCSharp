// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : NullableValueTypes
// Topic id : stage02/section01/nullable_value_types
//
// 步骤 7：T? = Nullable<T>，HasValue/Value、提升运算符、null 传播。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section01;

internal static class NullableValueTypes
{
    [LearnTopic("stage02/section01/nullable_value_types")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== NullableValueTypes ===");
        DemoBasics();
        DemoHasValueAndDefault();
        DemoNullCoalescing();
        DemoLiftedOperators();
        DemoIsNullableT();
        return 0;
    }

    private static void DemoBasics()
    {
        Console.WriteLine("-- T? 即 Nullable<T> --");
        int? age = null;
        Debug.Assert(typeof(int?) == typeof(Nullable<int>));
        Debug.Assert(!age.HasValue);
        age = 30;
        Debug.Assert(age.HasValue && age.Value == 30);
        Console.WriteLine($"  int? == Nullable<int>: {typeof(int?) == typeof(Nullable<int>)}");
    }

    private static void DemoHasValueAndDefault()
    {
        Console.WriteLine("-- HasValue / Value / GetValueOrDefault --");
        int? empty = null;
        Debug.Assert(empty.GetValueOrDefault() == 0);
        Debug.Assert(empty.GetValueOrDefault(-1) == -1);

        try
        {
            _ = empty.Value;
            Debug.Assert(false);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("  无值取 .Value → InvalidOperationException ✓");
        }

        int? filled = 7;
        Debug.Assert(filled.Value == 7);
    }

    private static void DemoNullCoalescing()
    {
        Console.WriteLine("-- ?? 空合并 --");
        int? age = null;
        int x = age ?? -1;
        Debug.Assert(x == -1);
        age = 18;
        Debug.Assert((age ?? -1) == 18);
        Console.WriteLine($"  null ?? -1 = {x}");
    }

    private static void DemoLiftedOperators()
    {
        Console.WriteLine("-- 提升运算符：null 传播 --");
        int? a = 10, b = null;
        int? sum = a + b;
        Debug.Assert(sum is null);
        int? sum2 = a + 5;
        Debug.Assert(sum2 == 15);
        Console.WriteLine($"  10 + null = {(sum.HasValue ? sum.Value.ToString() : "null")}; 10+5={sum2}");
    }

    private static void DemoIsNullableT()
    {
        Console.WriteLine("-- 与 string? 对照（机制不同） --");
        int? n = 1;
        string? s = "x";
        Debug.Assert(n.GetType() == typeof(int)); // 有值时 GetType 返回底层 T
        Debug.Assert(s!.GetType() == typeof(string));
        Console.WriteLine("  int? 运行时是结构体 Nullable<T>；string? 运行时就是 string（见下一课）");
    }
}
