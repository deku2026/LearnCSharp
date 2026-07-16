// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第4部分-面向内存的类型.md
// Stage    : Stage02_TypeSystem
// Section  : Section04_MemoryOrientedTypes
// Item     : ArraysIndexAndRange
// Topic id : stage02/section04/arrays_index_and_range
//
// 步骤 1：数组、^ 索引、.. 范围；数组切片会拷贝。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section04;

internal static class ArraysIndexAndRange
{
    [LearnTopic("stage02/section04/arrays_index_and_range")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ArraysIndexAndRange ===");
        DemoArrayBasics();
        DemoFromEndIndex();
        DemoRanges();
        DemoIndexRangeTypes();
        DemoArraySliceCopies();
        return 0;
    }

    private static void DemoArrayBasics()
    {
        Console.WriteLine("-- 数组基础 --");
        int[] a = new int[3];
        int[] b = { 1, 2, 3 };
        int[,] grid = new int[2, 3];
        int[][] jagged = [new int[2], new int[3]];
        Debug.Assert(a.Length == 3 && b[2] == 3);
        Debug.Assert(grid.GetLength(0) == 2);
        jagged[0][0] = 9;
        Debug.Assert(jagged[0][0] == 9);
        Console.WriteLine($"  b.Length={b.Length}; 数组是引用类型，元素内联在堆数组中");
    }

    private static void DemoFromEndIndex()
    {
        Console.WriteLine("-- ^ 从末尾索引 --");
        int[] arr = [10, 20, 30, 40, 50];
        Debug.Assert(arr[^1] == 50);
        Debug.Assert(arr[^2] == 40);
        // arr[^0] 越界：^0 == Length
        Console.WriteLine($"  arr[^1]={arr[^1]}, arr[^2]={arr[^2]}");
    }

    private static void DemoRanges()
    {
        Console.WriteLine("-- .. 范围：含起始不含结束 --");
        int[] arr = [10, 20, 30, 40, 50];
        int[] mid = arr[1..3];
        int[] head = arr[..2];
        int[] tail = arr[2..];
        int[] last2 = arr[^2..];
        Debug.Assert(mid is [20, 30]);
        Debug.Assert(head is [10, 20]);
        Debug.Assert(tail is [30, 40, 50]);
        Debug.Assert(last2 is [40, 50]);
        Console.WriteLine($"  mid=[{string.Join(',', mid)}], last2=[{string.Join(',', last2)}]");
    }

    private static void DemoIndexRangeTypes()
    {
        Console.WriteLine("-- Index / Range 变量 --");
        int[] arr = [10, 20, 30, 40, 50];
        Index last = ^1;
        Range middle = 1..3;
        Debug.Assert(arr[last] == 50);
        int[] m = arr[middle];
        Debug.Assert(m is [20, 30]);
        Console.WriteLine($"  Index/Range 是结构体，可独立存储");
    }

    private static void DemoArraySliceCopies()
    {
        Console.WriteLine("-- ⚠ 数组 [..] 切片分配新数组并拷贝 --");
        int[] arr = [1, 2, 3, 4, 5];
        int[] slice = arr[1..3];
        slice[0] = 99;
        Debug.Assert(arr[1] == 2); // 原数组未改 → 证明是拷贝
        Span<int> spanSlice = arr.AsSpan()[1..3];
        spanSlice[0] = 77;
        Debug.Assert(arr[1] == 77); // Span 切片是视图
        Console.WriteLine("  array[1..3] 拷贝；span[1..3] 零拷贝视图");
    }
}
