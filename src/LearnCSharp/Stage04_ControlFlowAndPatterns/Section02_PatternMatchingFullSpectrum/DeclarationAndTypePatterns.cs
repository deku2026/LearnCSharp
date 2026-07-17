// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第2部分-模式匹配全谱.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section02_PatternMatchingFullSpectrum
// Item     : DeclarationAndTypePatterns
// Topic id : stage04/section02/declaration_and_type_patterns
//
// 步骤 2：声明/类型模式、var 模式、弃元 _。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section02;

internal static class DeclarationAndTypePatterns
{
    [LearnTopic("stage04/section02/declaration_and_type_patterns")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DeclarationAndTypePatterns ===");
        DemoDeclarationPattern();
        DemoTypePattern();
        DemoVarPattern();
        DemoDiscardPattern();
        DemoNullableUnpack();
        return 0;
    }

    private static void DemoDeclarationPattern()
    {
        Console.WriteLine("-- 声明模式 Type v：检查 + 提取，拒 null --");
        object o = "hi";
        if (o is string s)
            Debug.Assert(s.Length == 2);

        object? n = null;
        Debug.Assert(n is not string);
        // null is string s → false（用 object? 变量承载，避免常量折叠警告）
        object? nullBox = null;
        Debug.Assert(!(nullBox is string));
        Console.WriteLine("  null is string → false（声明模式拒 null）");
    }

    private static void DemoTypePattern()
    {
        Console.WriteLine("-- 类型模式 Type：只检查不提取 --");
        object o = 42;
        Debug.Assert(o is int);
        Debug.Assert(o is not string);

        string kind = o switch
        {
            int => "整数",
            string => "字符串",
            _ => "其他",
        };
        Debug.Assert(kind == "整数");
        Console.WriteLine($"  42 is int → {kind}");
    }

    private static void DemoVarPattern()
    {
        Console.WriteLine("-- var 模式：永远成功（含 null） --");
        object? o = null;
        Debug.Assert(o is var x);
        Debug.Assert(x is null);

        o = "x";
        if (o is var y)
            Debug.Assert(y is string);

        // var 不做类型检查，别当 is string 用
        Console.WriteLine("  null is var x → true；var 只绑名不检查类型");
    }

    private static void DemoDiscardPattern()
    {
        Console.WriteLine("-- 弃元 _：匹配一切不绑定 --");
        object o = 3.14;
        string kind = o switch
        {
            int => "整数",
            string => "字符串",
            _ => "其他",
        };
        Debug.Assert(kind == "其他");

        // switch 语句里裸 _ 会被当标识符，常用 var _
        switch (o)
        {
            case int:
                Debug.Assert(false);
                break;
            case var _:
                Console.WriteLine("  switch 语句兜底用 var _");
                break;
        }
    }

    private static void DemoNullableUnpack()
    {
        Console.WriteLine("-- 可空值类型：is int value 解包 --");
        int? maybe = null;
        Debug.Assert(!(maybe is int));

        maybe = 7;
        if (maybe is int value)
            Debug.Assert(value == 7);
        else
            Debug.Assert(false);

        Console.WriteLine("  nullableInt is int value → 非 null 且解包一步完成");
    }
}
