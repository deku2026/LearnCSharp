// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第2部分-模式匹配全谱.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section02_PatternMatchingFullSpectrum
// Item     : WhenClauseAndExhaustiveness
// Topic id : stage04/section02/when_clause_and_exhaustiveness
//
// 步骤 8：when 守卫、穷尽性 CS8509、综合应用。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section02;

internal static class WhenClauseAndExhaustiveness
{
    [LearnTopic("stage04/section02/when_clause_and_exhaustiveness")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WhenClauseAndExhaustiveness ===");
        DemoWhenOnSwitchExpression();
        DemoWhenOnSwitchStatement();
        DemoWhenRuntimeCompare();
        DemoEnumExhaustiveness();
        DemoCombinedPatterns();
        return 0;
    }

    private static void DemoWhenOnSwitchExpression()
    {
        Console.WriteLine("-- when：模式后任意运行时条件 --");
        var now = new DateTime(2026, 7, 16);
        static string Check(Order o, DateTime now) => o switch
        {
            { Total: > 1000 } when o.Date > now.AddDays(-30) => "近期大额",
            { Total: > 1000 } => "大额",
            _ => "普通",
        };

        var recentBig = new Order(1500m, now.AddDays(-5));
        var oldBig = new Order(1500m, now.AddDays(-60));
        var small = new Order(50m, now);
        Debug.Assert(Check(recentBig, now) == "近期大额");
        Debug.Assert(Check(oldBig, now) == "大额");
        Debug.Assert(Check(small, now) == "普通");
        Console.WriteLine($"  近期大额 / 大额 / 普通 分流 OK");
    }

    private static void DemoWhenOnSwitchStatement()
    {
        Console.WriteLine("-- switch 语句 case + when --");
        static string Parity(int n)
        {
            switch (n)
            {
                case int x when x % 2 == 0:
                    return "偶数";
                case int:
                    return "奇数";
            }
        }

        Debug.Assert(Parity(4) == "偶数");
        Debug.Assert(Parity(7) == "奇数");
        // if (n is int x when ...) 不合法 — when 仅 switch
        Console.WriteLine("  when 仅用于 switch，if 用 && 接条件");
    }

    private static void DemoWhenRuntimeCompare()
    {
        Console.WriteLine("-- 关系模式须编译期常量；运行时值用 when --");
        int threshold = 10; // 运行时
        static string Above(int n, int threshold) => n switch
        {
            // case > threshold: 不合法
            var x when x > threshold => "above",
            _ => "at-or-below",
        };

        Debug.Assert(Above(15, threshold) == "above");
        Debug.Assert(Above(5, threshold) == "at-or-below");
        Console.WriteLine($"  n when n > threshold(={threshold})");
    }

    private static void DemoEnumExhaustiveness()
    {
        Console.WriteLine("-- 穷尽性：枚举列全仍加 _ --");
        static string Name(Direction d) => d switch
        {
            Direction.North => "北",
            Direction.South => "南",
            Direction.East => "东",
            Direction.West => "西",
            _ => throw new ArgumentOutOfRangeException(nameof(d)),
        };

        Debug.Assert(Name(Direction.North) == "北");
        Debug.Assert(Name(Direction.West) == "西");

        bool threw = false;
        try
        {
            _ = Name((Direction)99);
        }
        catch (ArgumentOutOfRangeException)
        {
            threw = true;
        }
        Debug.Assert(threw);
        Console.WriteLine("  枚举不封闭：未定义整数走 _ 兜底");
    }

    private static void DemoCombinedPatterns()
    {
        Console.WriteLine("-- 综合：类型/属性/位置/列表/常量 + when + _ --");
        static decimal Price(object item) => item switch
        {
            Book { Pages: > 500 } => 49.9m,
            Book => 29.9m,
            Movie { Rating: >= 8.0 } => 19.9m,
            Movie => 9.9m,
            (string _, int qty) when qty > 0 => qty * 5m,
            int[] { Length: > 0 } => 9.9m,
            null => throw new ArgumentNullException(nameof(item)),
            _ => 0m,
        };

        Debug.Assert(Price(new Book("Long", 600)) == 49.9m);
        Debug.Assert(Price(new Book("Short", 100)) == 29.9m);
        Debug.Assert(Price(new Movie("Hit", 8.5)) == 19.9m);
        Debug.Assert(Price(("widget", 3)) == 15m);
        Debug.Assert(Price(new[] { 1, 2 }) == 9.9m);
        Debug.Assert(Price("other") == 0m);
        Console.WriteLine("  多模式综合定价 OK");
    }

    private sealed record Order(decimal Total, DateTime Date);
    private sealed record Book(string Title, int Pages);
    private sealed record Movie(string Title, double Rating);

    private enum Direction
    {
        North,
        South,
        East,
        West,
    }
}
