// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第2部分-模式匹配全谱.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section02_PatternMatchingFullSpectrum
// Item     : ConstantAndRelationalPatterns
// Topic id : stage04/section02/constant_and_relational_patterns
//
// 步骤 3：常量模式与关系模式（C# 9）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section02;

internal static class ConstantAndRelationalPatterns
{
    [LearnTopic("stage04/section02/constant_and_relational_patterns")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ConstantAndRelationalPatterns ===");
        DemoConstantPatterns();
        DemoIsNull();
        DemoRelational();
        DemoScoreBands();
        DemoIsNullVsOverloadedEquals();
        return 0;
    }

    private static void DemoConstantPatterns()
    {
        Console.WriteLine("-- 常量模式 is 常量 --");
        int n = 0;
        Debug.Assert(n is 0);
        string s = "hi";
        Debug.Assert(s is "hi");
        DayOfWeek d = DayOfWeek.Saturday;
        string t = d is DayOfWeek.Saturday or DayOfWeek.Sunday ? "周末" : "工作日";
        Debug.Assert(t == "周末");
        Console.WriteLine($"  n is 0, s is \"hi\", Sat/Sun → {t}");
    }

    private static void DemoIsNull()
    {
        Console.WriteLine("-- is null / is not null --");
        object? obj = null;
        Debug.Assert(obj is null);
        obj = "x";
        Debug.Assert(obj is not null);
        Console.WriteLine("  is null 是判空推荐写法");
    }

    private static void DemoRelational()
    {
        Console.WriteLine("-- 关系模式 < > <= >= --");
        static string Sign(int n) => n switch
        {
            < 0 => "负数",
            0 => "零",
            > 0 => "正数",
        };

        Debug.Assert(Sign(-3) == "负数");
        Debug.Assert(Sign(0) == "零");
        Debug.Assert(Sign(5) == "正数");
        Console.WriteLine($"  -3→{Sign(-3)}, 0→{Sign(0)}, 5→{Sign(5)}");
    }

    private static void DemoScoreBands()
    {
        Console.WriteLine("-- 关系 + 区间分档 --");
        static string AgeGroup(int age) => age switch
        {
            < 13 => "儿童",
            >= 13 and < 18 => "青少年",
            >= 18 and < 65 => "成人",
            _ => "老年",
        };

        Debug.Assert(AgeGroup(10) == "儿童");
        Debug.Assert(AgeGroup(15) == "青少年");
        Debug.Assert(AgeGroup(30) == "成人");
        Debug.Assert(AgeGroup(70) == "老年");
        Console.WriteLine($"  15→{AgeGroup(15)}, 30→{AgeGroup(30)}");
    }

    private static void DemoIsNullVsOverloadedEquals()
    {
        Console.WriteLine("-- is null 不调用用户 == --");
        var a = new WeirdEq("a");
        WeirdEq? b = null;
        Debug.Assert(b is null);
        // a == null 会进重载；is null 走引用判空
        bool viaOp = a == null;
        Debug.Assert(!viaOp);
        Debug.Assert(a is not null);
        Console.WriteLine("  is null 绕过用户重载的 ==，更安全");
    }

    private sealed class WeirdEq(string name)
    {
        public string Name { get; } = name;

        public static bool operator ==(WeirdEq? left, WeirdEq? right)
        {
            // 故意“反常”：总认为不相等（演示 is null 不走这里）
            if (ReferenceEquals(left, right))
                return true;
            return false;
        }

        public static bool operator !=(WeirdEq? left, WeirdEq? right) => !(left == right);

        public override bool Equals(object? obj) => ReferenceEquals(this, obj);
        public override int GetHashCode() => Name.GetHashCode();
    }
}
