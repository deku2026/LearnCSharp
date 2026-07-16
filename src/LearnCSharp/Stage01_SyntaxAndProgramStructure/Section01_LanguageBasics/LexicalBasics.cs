// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : LexicalBasics
// Topic id : stage01/section01/lexical_basics
//
// 步骤 6：语句/表达式、标识符、@ 转义关键字、保留 vs 上下文关键字、空白。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class LexicalBasics
{
    [LearnTopic("stage01/section01/lexical_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== LexicalBasics ===");
        DemoStatementsAndExpressions();
        DemoIdentifiersAndAtEscape();
        DemoKeywordTables();
        DemoWhitespaceAndNaming();
        return 0;
    }

    private static void DemoStatementsAndExpressions()
    {
        Console.WriteLine("-- 语句 vs 表达式 --");
        // 表达式产生值；语句执行动作，通常以 ; 结尾
        int maxResult = Add(1, 2) + Add(3, 4);
        Console.WriteLine($"  int maxResult = Add(1,2)+Add(3,4) → {maxResult}");
        Debug.Assert(maxResult == 10);

        // 块用 { }；与 C++ 一致
        {
            int nested = 1;
            nested += 2;
            Debug.Assert(nested == 3);
            Console.WriteLine($"  块内 nested={nested}");
        }
    }

    private static void DemoIdentifiersAndAtEscape()
    {
        Console.WriteLine("-- 标识符与 @ 转义 --");
        // 字母/下划线开头；大小写敏感；@ 可把关键字当标识符
        int @class = 5;
        int @if = 1;
        string @namespace = "demo";
        Console.WriteLine($"  @class={@class}, @if={@if}, @namespace={@namespace}");
        Debug.Assert(@class == 5);
        Debug.Assert(@if == 1);

        // Unicode 字母可用（教学演示，生产代码慎用）
        int 数量 = 3;
        Debug.Assert(数量 == 3);
        Console.WriteLine($"  Unicode 标识符 数量={数量}");

        // 大小写敏感
        int value = 1;
        int Value = 2;
        Debug.Assert(value != Value);
        Console.WriteLine($"  value={value}, Value={Value}（大小写敏感）");
    }

    private static void DemoKeywordTables()
    {
        Console.WriteLine("-- 关键字两张表 --");
        string[] reserved =
        [
            "class", "int", "if", "return", "static", "namespace", "using", "void", "true", "null",
        ];
        string[] contextual =
        [
            "var", "record", "with", "yield", "async", "await", "required", "scoped", "allows",
            "extension", "field", // C# 14
        ];

        Console.WriteLine("  保留关键字: 任何地方不能直接当标识符 → " + string.Join(", ", reserved));
        Console.WriteLine("  上下文关键字: 仅特定语境是关键字 → " + string.Join(", ", contextual));
        Console.WriteLine("  🔶 上下文关键字便于向后兼容（老代码变量名 record/field 仍可编译）");

        // var 作为上下文关键字：此处是合法变量声明（本主题只演示合法性）
#pragma warning disable IDE0008
        var sample = 42;
#pragma warning restore IDE0008
        Debug.Assert(sample == 42);
        Debug.Assert(reserved.Contains("class"));
        Debug.Assert(contextual.Contains("var"));
        Debug.Assert(contextual.Contains("field"));
    }

    private static void DemoWhitespaceAndNaming()
    {
        Console.WriteLine("-- 空白与命名约定 --");
        int a = 1 + 2;
        int b = 1 + 2;
        Debug.Assert(a == b);
        Console.WriteLine("  空白不影响语义（字符串内除外）；风格靠 .editorconfig");

        // .NET 约定
        Console.WriteLine("  类型/方法/属性: PascalCase");
        Console.WriteLine("  局部变量/参数: camelCase");
        Console.WriteLine("  私有字段: _camelCase");

        int localCount = CountItems(["a", "b"]);
        Debug.Assert(localCount == 2);
        Console.WriteLine($"  CountItems → {localCount}");
    }

    private static int Add(int left, int right) => left + right;

    private static int CountItems(string[] items) => items.Length;
}
