// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第4部分-面向内存的类型.md
// Stage    : Stage02_TypeSystem
// Section  : Section04_MemoryOrientedTypes
// Item     : CollectionExpressions
// Topic id : stage02/section04/collection_expressions
//
// 步骤 3：集合表达式 [..]、spread .. 展开。

using System.Collections.Immutable;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section04;

internal static class CollectionExpressions
{
    [LearnTopic("stage02/section04/collection_expressions")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CollectionExpressions ===");
        DemoTargetTyping();
        DemoSpread();
        DemoEmptyAndNested();
        DemoSpanAndStackalloc();
        DemoImmutable();
        return 0;
    }

    private static void DemoTargetTyping()
    {
        Console.WriteLine("-- 目标类型决定具体集合 --");
        int[] arr = [1, 2, 3];
        List<int> list = [1, 2, 3];
        Span<int> span = [1, 2, 3];
        Debug.Assert(arr is [1, 2, 3]);
        Debug.Assert(list.Count == 3);
        Debug.Assert(span.Length == 3);
        Console.WriteLine($"  [] → array/List/Span 由左侧类型决定");
    }

    private static void DemoSpread()
    {
        Console.WriteLine("-- spread .. 展开 --");
        int[] a = [1, 2];
        int[] b = [3, 4];
        int[] merged = [.. a, 99, .. b];
        Debug.Assert(merged is [1, 2, 99, 3, 4]);
        List<string> names = ["Ada", .. new[] { "Grace", "Katherine" }];
        Debug.Assert(names.Count == 3);
        Console.WriteLine($"  merged=[{string.Join(',', merged)}]");
    }

    private static void DemoEmptyAndNested()
    {
        Console.WriteLine("-- 空集合与嵌套 --");
        int[] empty = [];
        Debug.Assert(empty.Length == 0);
        int[][] jagged = [[1, 2], [3]];
        Debug.Assert(jagged[0][1] == 2 && jagged[1][0] == 3);
        Console.WriteLine("  [] 空；[[1,2],[3]] 交错");
    }

    private static void DemoSpanAndStackalloc()
    {
        Console.WriteLine("-- 与 Span 配合 --");
        ReadOnlySpan<int> ros = [10, 20, 30];
        Debug.Assert(ros[1] == 20);
        // 编译器可为小集合优化分配策略
        Console.WriteLine($"  ReadOnlySpan from [] length={ros.Length}");
    }

    private static void DemoImmutable()
    {
        Console.WriteLine("-- ImmutableArray 等 --");
        ImmutableArray<int> ia = [1, 2, 3];
        Debug.Assert(ia.Length == 3 && ia[0] == 1);
        HashSet<int> set = [1, 2, 2, 3];
        Debug.Assert(set.Count == 3);
        Console.WriteLine($"  ImmutableArray + HashSet 均支持集合表达式");
    }
}
