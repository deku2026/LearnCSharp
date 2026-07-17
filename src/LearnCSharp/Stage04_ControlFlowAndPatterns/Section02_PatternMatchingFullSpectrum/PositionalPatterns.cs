// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第2部分-模式匹配全谱.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section02_PatternMatchingFullSpectrum
// Item     : PositionalPatterns
// Topic id : stage04/section02/positional_patterns
//
// 步骤 6：位置模式 — 解构 + 匹配（元组 / Deconstruct / record）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section02;

internal static class PositionalPatterns
{
    [LearnTopic("stage04/section02/positional_patterns")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== PositionalPatterns ===");
        DemoTupleQuadrant();
        DemoRecordPositional();
        DemoExtractVars();
        DemoDiagonal();
        DemoCustomDeconstruct();
        return 0;
    }

    private static void DemoTupleQuadrant()
    {
        Console.WriteLine("-- 元组位置匹配 --");
        static string Quadrant((int x, int y) p) => p switch
        {
            (0, 0) => "原点",
            ( > 0, > 0) => "第一象限",
            ( < 0, > 0) => "第二象限",
            ( < 0, < 0) => "第三象限",
            ( > 0, < 0) => "第四象限",
            (0, _) => "Y轴",
            (_, 0) => "X轴",
        };

        Debug.Assert(Quadrant((0, 0)) == "原点");
        Debug.Assert(Quadrant((3, 4)) == "第一象限");
        Debug.Assert(Quadrant((-1, 2)) == "第二象限");
        Debug.Assert(Quadrant((0, 5)) == "Y轴");
        Console.WriteLine($"  (3,4)→{Quadrant((3, 4))}, (0,0)→{Quadrant((0, 0))}");
    }

    private static void DemoRecordPositional()
    {
        Console.WriteLine("-- record 自动 Deconstruct --");
        static string Loc(Point p) => p switch
        {
            (0, 0) => "原点",
            ( > 0, > 0) => "右上",
            _ => "其他",
        };

        Debug.Assert(Loc(new Point(0, 0)) == "原点");
        Debug.Assert(Loc(new Point(1, 2)) == "右上");
        Debug.Assert(Loc(new Point(-1, 0)) == "其他");
        Console.WriteLine($"  Point(1,2)→{Loc(new Point(1, 2))}");
    }

    private static void DemoExtractVars()
    {
        Console.WriteLine("-- (var a, var b) 提取 --");
        (int x, int y) p = (3, 4);
        string s = p switch
        {
            (var a, var b) => $"({a},{b})",
        };
        Debug.Assert(s == "(3,4)");
        Console.WriteLine($"  提取 → {s}");
    }

    private static void DemoDiagonal()
    {
        Console.WriteLine("-- 位置 + 额外条件 --");
        static bool Diagonal(Point p) => p is (var x, var y) && x == y;
        Debug.Assert(Diagonal(new Point(3, 3)));
        Debug.Assert(!Diagonal(new Point(3, 4)));
        Console.WriteLine("  is (var x, var y) && x==y → 对角线");
    }

    private static void DemoCustomDeconstruct()
    {
        Console.WriteLine("-- 自定义类型 Deconstruct --");
        ColorRgb c = new ColorRgb(255, 0, 128);
        string kind = c switch
        {
            (255, 0, 0) => "纯红",
            (0, 255, 0) => "纯绿",
            (var r, 0, var b) when r > 0 && b > 0 => "红蓝混合",
            _ => "其他色",
        };
        Debug.Assert(kind == "红蓝混合");
        Console.WriteLine($"  ColorRgb(255,0,128)→{kind}");
    }

    private sealed record Point(int X, int Y);

    private sealed class ColorRgb(int r, int g, int b)
    {
        public int R { get; } = r;
        public int G { get; } = g;
        public int B { get; } = b;

        public void Deconstruct(out int r, out int g, out int b)
        {
            r = R;
            g = G;
            b = B;
        }
    }
}
