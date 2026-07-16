// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第1部分-字符串与格式化.md
// Stage    : Stage09_BCL
// Section  : Section01_StringsAndFormatting
// Item     : StringType
// Topic id : stage09/section01/string_type
//
// 步骤 1：String 不可变、常用操作、UTF-16、驻留、AsSpan 零分配切片

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section01;

internal static class StringType
{
    [LearnTopic("stage09/section01/string_type")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StringType ===");
        DemoImmutability();
        DemoCommonOps();
        DemoUtf16AndIntern();
        DemoAsSpanSlice();
        return 0;
    }

    private static void DemoImmutability()
    {
        Console.WriteLine("-- immutability: ops return new string --");
        string s = "hello";
        string upper = s.ToUpperInvariant();
        Debug.Assert(s == "hello" && upper == "HELLO");
        s.Replace("l", "L"); // discarded → no effect
        Debug.Assert(s == "hello");
        s = s.Replace("l", "L");
        Debug.Assert(s == "heLLo");
        Console.WriteLine($"  after reassignment: {s}");
    }

    private static void DemoCommonOps()
    {
        Console.WriteLine("-- Length / IndexOf / Split / Join / Trim --");
        string s = "Hello, World";
        Debug.Assert(s.Length == 12);
        Debug.Assert(s.Substring(7) == "World");
        Debug.Assert(s.IndexOf("World", StringComparison.Ordinal) == 7);
        Debug.Assert(s.Contains("World", StringComparison.Ordinal));
        string[] parts = s.Split(',');
        Debug.Assert(parts.Length == 2);
        string joined = string.Join("-", "a", "b", "c");
        Debug.Assert(joined == "a-b-c");
        Debug.Assert(string.IsNullOrWhiteSpace("  "));
        Console.WriteLine($"  Join → {joined}; Trim → '{s.Trim()}'");
    }

    private static void DemoUtf16AndIntern()
    {
        Console.WriteLine("-- UTF-16 code units; interning of literals --");
        string emoji = "😀";
        Debug.Assert(emoji.Length == 2); // surrogate pair = 2 chars
        string a = "shared-literal";
        string b = "shared-literal";
        Debug.Assert(ReferenceEquals(a, b)); // interned
        string built = string.Concat("shared-", "literal");
        Debug.Assert(!ReferenceEquals(a, built) || string.IsInterned(built) is not null);
        Console.WriteLine($"  emoji Length={emoji.Length} (surrogate pair); literals share instance");
    }

    private static void DemoAsSpanSlice()
    {
        Console.WriteLine("-- AsSpan: zero-alloc slice vs Substring --");
        string s = "Hello, World";
        ReadOnlySpan<char> span = s.AsSpan(0, 5);
        Debug.Assert(span.SequenceEqual("Hello"));
        string sub = s.Substring(0, 5); // allocates
        Debug.Assert(sub == "Hello");
        Console.WriteLine($"  AsSpan(0,5)='{span.ToString()}' (no new string until materialize)");
    }
}
