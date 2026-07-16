// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第2部分-模式匹配全谱.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section02_PatternMatchingFullSpectrum
// Item     : IsAndSwitchHosts
// Topic id : stage04/section02/is_and_switch_hosts
//
// 步骤 1：三个宿主 — is 表达式 / switch 语句 / switch 表达式。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section02;

internal static class IsAndSwitchHosts
{
    [LearnTopic("stage04/section02/is_and_switch_hosts")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IsAndSwitchHosts ===");
        DemoIsExpression();
        DemoSwitchStatementHost();
        DemoSwitchExpression();
        DemoFirstMatchWins();
        DemoShapeArea();
        return 0;
    }

    private static void DemoIsExpression()
    {
        Console.WriteLine("-- is 表达式：返回 bool + 提取 --");
        object o = "hello";
        if (o is string s)
            Debug.Assert(s.Length == 5);

        int age = 30;
        Debug.Assert(age is >= 18 and < 65);
        Debug.Assert(o is not null);
        Console.WriteLine($"  o is string s → Length={((string)o).Length}; age is [18,65)");
    }

    private static void DemoSwitchStatementHost()
    {
        Console.WriteLine("-- switch 语句：case 用模式 --");
        static string Describe(Shape shape)
        {
            switch (shape)
            {
                case Circle { Radius: > 0 } c:
                    return $"圆 r={c.Radius}";
                case Rectangle r:
                    return $"矩形 {r.Width}x{r.Height}";
                default:
                    return "未知";
            }
        }

        Debug.Assert(Describe(new Circle(2)) == "圆 r=2");
        Debug.Assert(Describe(new Rectangle(3, 4)) == "矩形 3x4");
        Debug.Assert(Describe(new Circle(0)).StartsWith("未知") || Describe(new Circle(0)) == "未知");
        // Radius: > 0 不匹配 0
        Debug.Assert(Describe(new Circle(0)) == "未知");
        Console.WriteLine($"  Circle(2)→{Describe(new Circle(2))}");
    }

    private static void DemoSwitchExpression()
    {
        Console.WriteLine("-- switch 表达式：模式 => 结果 --");
        static string Grade(int score) => score switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 60 => "Pass",
            _ => "Fail",
        };

        Debug.Assert(Grade(95) == "A");
        Debug.Assert(Grade(85) == "B");
        Debug.Assert(Grade(70) == "Pass");
        Debug.Assert(Grade(50) == "Fail");
        Console.WriteLine($"  95→{Grade(95)}, 50→{Grade(50)}");
    }

    private static void DemoFirstMatchWins()
    {
        Console.WriteLine("-- 从上到下取第一个匹配 --");
        static string Band(int n) => n switch
        {
            >= 90 => "high",
            >= 60 => "mid", // 若把 >=60 放前面会吞掉 90+
            _ => "low",
        };

        Debug.Assert(Band(95) == "high");
        Debug.Assert(Band(70) == "mid");
        Console.WriteLine("  顺序敏感：更具体的模式应在前");
    }

    private static void DemoShapeArea()
    {
        Console.WriteLine("-- 按形状求面积 --");
        static double Area(Shape s) => s switch
        {
            Circle c => Math.PI * c.Radius * c.Radius,
            Rectangle r => r.Width * r.Height,
            _ => throw new ArgumentException("未知形状"),
        };

        Debug.Assert(Math.Abs(Area(new Circle(1)) - Math.PI) < 1e-9);
        Debug.Assert(Area(new Rectangle(3, 4)) == 12);
        Console.WriteLine($"  Circle(1)≈{Area(new Circle(1)):F2}, Rect 3x4={Area(new Rectangle(3, 4))}");
    }

    private abstract record Shape;
    private sealed record Circle(double Radius) : Shape;
    private sealed record Rectangle(double Width, double Height) : Shape;
}
