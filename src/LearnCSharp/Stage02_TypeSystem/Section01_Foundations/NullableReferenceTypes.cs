// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : NullableReferenceTypes
// Topic id : stage02/section01/nullable_reference_types
//
// 步骤 8：NRT 纯编译期、!、可空特性 [NotNullWhen] 等。

#nullable enable

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section01;

internal static class NullableReferenceTypes
{
    [LearnTopic("stage02/section01/nullable_reference_types")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== NullableReferenceTypes ===");
        DemoNullStateFlow();
        DemoCompileTimeOnly();
        DemoNullForgiving();
        DemoNullableAttributes();
        DemoTwoQuestionMarks();
        return 0;
    }

    private static void DemoNullStateFlow()
    {
        Console.WriteLine("-- null-state 流分析 --");
        string notNull = "hi";
        string? canBeNull = null;
        Debug.Assert(notNull.Length == 2);

        if (canBeNull is not null)
            Debug.Assert(false); // 本 demo 中为 null
        else
            Console.WriteLine("  canBeNull is null —— 解引用前需 is not null 收窄");

        canBeNull = "ok";
        if (canBeNull is not null)
            Debug.Assert(canBeNull.Length == 2);
    }

    private static void DemoCompileTimeOnly()
    {
        Console.WriteLine("-- ⭐ NRT 纯编译期：运行时 string? == string --");
        string a = "x";
        string? b = "x";
        Debug.Assert(a.GetType() == b.GetType());
        Debug.Assert(typeof(string) == typeof(string)); // 无独立 Nullable 类型
        Console.WriteLine("  运行时同一类型；? 只是元数据标注 + 编译器警告");
    }

    private static void DemoNullForgiving()
    {
        Console.WriteLine("-- null-forgiving !（让编译器闭嘴，无运行时效果） --");
        string? maybe = GetMaybeName(true);
        string sure = maybe!; // 我担保非空
        Debug.Assert(sure.Length > 0);
        Console.WriteLine($"  maybe! = {sure} —— 滥用 ! = 自废 NRT");
    }

    private static void DemoNullableAttributes()
    {
        Console.WriteLine("-- [NotNullWhen] Try 风格 --");
        if (TryGetName(out var n))
        {
            Debug.Assert(n.Length == 3); // 特性：true 时 n 非空
            Console.WriteLine($"  TryGetName → {n}");
        }

        if (!TryGetNameFail(out var miss))
        {
            Debug.Assert(miss is null);
            Console.WriteLine("  Try 失败时 out 可为 null");
        }
    }

    private static void DemoTwoQuestionMarks()
    {
        Console.WriteLine("-- 两个 ? 机制天差地别 --");
        int? value = 1;       // 运行时 Nullable<int>
        string? text = "a";   // 运行时就是 string
        Debug.Assert(Nullable.GetUnderlyingType(typeof(int?)) == typeof(int));
        Debug.Assert(Nullable.GetUnderlyingType(typeof(string)) is null);
        Debug.Assert(text is not null);
        Console.WriteLine($"  Nullable.GetUnderlyingType(int?)={Nullable.GetUnderlyingType(typeof(int?))}");
        Console.WriteLine($"  Nullable.GetUnderlyingType(string)={Nullable.GetUnderlyingType(typeof(string))?.Name ?? "null"}");
        _ = value;
    }

    private static string? GetMaybeName(bool present) => present ? "Ada" : null;

    private static bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = "Ada";
        return true;
    }

    private static bool TryGetNameFail([NotNullWhen(true)] out string? name)
    {
        name = null;
        return false;
    }
}
