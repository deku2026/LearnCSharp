// LearnCSharp example (filled)
// Doc      : CSharp-阶段4-控制流与模式匹配-第2部分-模式匹配全谱.md
// Stage    : Stage04_ControlFlowAndPatterns
// Section  : Section02_PatternMatchingFullSpectrum
// Item     : ListPatterns
// Topic id : stage04/section02/list_patterns
//
// 步骤 7：列表模式 + 切片 ..（C# 11）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage04.Section02;

internal static class ListPatterns
{
    [LearnTopic("stage04/section02/list_patterns")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ListPatterns ===");
        DemoExactAndLength();
        DemoSliceEnds();
        DemoExtractHeadRest();
        DemoSwitchDescribe();
        DemoCommandDispatch();
        DemoThreeDotsContexts();
        return 0;
    }

    private static void DemoExactAndLength()
    {
        Console.WriteLine("-- 精确匹配与长度 --");
        int[] a = [1, 2, 3];
        Debug.Assert(a is [1, 2, 3]);
        Debug.Assert(a is [_, _, _]);
        Debug.Assert(a is not [1, 2]);
        Debug.Assert(Array.Empty<int>() is []);
        Console.WriteLine("  [1,2,3] / [_,_,_] / []");
    }

    private static void DemoSliceEnds()
    {
        Console.WriteLine("-- 切片：首/尾/首尾 --");
        int[] a = [1, 2, 3, 9];
        Debug.Assert(a is [1, ..]);
        Debug.Assert(a is [.., 9]);
        Debug.Assert(a is [1, .., 9]);
        Debug.Assert(a is not [2, ..]);
        Console.WriteLine("  [1,..] / [..,9] / [1,..,9]");
    }

    private static void DemoExtractHeadRest()
    {
        Console.WriteLine("-- [var first, .. var rest] --");
        int[] a = [1, 2, 3];
        if (a is [var first, .. var rest])
        {
            Debug.Assert(first == 1);
            Debug.Assert(rest is [2, 3]);
            Console.WriteLine($"  first={first}, rest.Length={rest.Length}");
        }
        else
        {
            Debug.Assert(false);
        }
    }

    private static void DemoSwitchDescribe()
    {
        Console.WriteLine("-- switch 按序列形状 --");
        static string Describe(int[] arr) => arr switch
        {
            [] => "空",
            [var only] => $"单元素 {only}",
            [var f, var l] => $"两元素 {f},{l}",
            [var f, .., var l] => $"首 {f} 尾 {l}",
            _ => "其他",
        };

        Debug.Assert(Describe([]) == "空");
        Debug.Assert(Describe([7]) == "单元素 7");
        Debug.Assert(Describe([1, 2]) == "两元素 1,2");
        Debug.Assert(Describe([1, 2, 3, 4]) == "首 1 尾 4");
        Console.WriteLine($"  [1,2,3,4]→{Describe([1, 2, 3, 4])}");
    }

    private static void DemoCommandDispatch()
    {
        Console.WriteLine("-- 命令序列分发 --");
        static string Dispatch(string[] tokens) => tokens switch
        {
            [] => "empty",
            ["help"] => "show help",
            ["set", var key, var val] => $"set {key}={val}",
            ["get", var key] => $"get {key}",
            [var cmd, ..] => $"unknown:{cmd}",
        };

        Debug.Assert(Dispatch([]) == "empty");
        Debug.Assert(Dispatch(["help"]) == "show help");
        Debug.Assert(Dispatch(["set", "x", "1"]) == "set x=1");
        Debug.Assert(Dispatch(["get", "x"]) == "get x");
        Debug.Assert(Dispatch(["foo", "bar"]) == "unknown:foo");
        Console.WriteLine($"  set x 1 → {Dispatch(["set", "x", "1"])}");
    }

    private static void DemoThreeDotsContexts()
    {
        Console.WriteLine("-- 三种 .. 语境 --");
        // 1) 列表切片模式
        int[] a = [1, 2, 3];
        Debug.Assert(a is [1, ..]);

        // 2) 集合表达式 spread
        int[] b = [0, .. a, 4];
        Debug.Assert(b is [0, 1, 2, 3, 4]);

        // 3) 范围运算符（索引切片）
        int[] mid = a[1..];
        Debug.Assert(mid is [2, 3]);

        Console.WriteLine("  列表..切片 / 集合..spread / 范围 a[1..] 三种语境");
    }
}
