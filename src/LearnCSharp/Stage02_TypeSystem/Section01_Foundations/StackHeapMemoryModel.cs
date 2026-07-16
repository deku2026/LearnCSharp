// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : StackHeapMemoryModel
// Topic id : stage02/section01/stack_heap_memory_model
//
// 步骤 3：破除“值类型在栈、引用类型在堆”——存哪取决于上下文。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section01;

internal static class StackHeapMemoryModel
{
    [LearnTopic("stage02/section01/stack_heap_memory_model")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StackHeapMemoryModel ===");
        DemoLocalValueUsuallyStack();
        DemoStructFieldOnHeap();
        DemoArrayElementsOnHeap();
        DemoClosureCapturesToHeap();
        DemoBoxingToHeap();
        DemoFiveScenariosSummary();
        return 0;
    }

    private static void DemoLocalValueUsuallyStack()
    {
        Console.WriteLine("-- 局部值类型：通常在栈帧/寄存器 --");
        int local = 42;
        PointV p = new() { X = 1, Y = 2 };
        Debug.Assert(local == 42 && p.X == 1);
        Console.WriteLine("  语言规范只规定复制语义，不规定必须在栈（Eric Lippert: 栈是实现细节）");
    }

    private static void DemoStructFieldOnHeap()
    {
        Console.WriteLine("-- class 字段中的 struct：内联在堆对象里 --");
        var holder = new Holder { P = new PointV { X = 7, Y = 8 } };
        Debug.Assert(holder.P.X == 7);
        // holder 在 GC 堆；P 作为字段内联在 holder 对象布局中，不在独立栈槽
        Console.WriteLine($"  Holder.P=({holder.P.X},{holder.P.Y}) —— 值类型字段跟着堆对象走");
    }

    private static void DemoArrayElementsOnHeap()
    {
        Console.WriteLine("-- 数组元素：紧凑排在堆上的数组对象内 --");
        int[] arr = { 1, 2, 3 };
        PointV[] pts = { new() { X = 1 }, new() { X = 2 } };
        Debug.Assert(arr[0] == 1 && pts[1].X == 2);
        Console.WriteLine($"  int[] Length={arr.Length}; PointV[] 元素也在堆数组内联");
    }

    private static void DemoClosureCapturesToHeap()
    {
        Console.WriteLine("-- 闭包捕获局部：提升到堆上的闭包对象 --");
        int captured = 10;
        Func<int> f = () => captured + 1;
        captured = 20;
        Debug.Assert(f() == 21);
        Console.WriteLine($"  lambda 读到 captured={f() - 1} —— 局部变成闭包字段（堆）");
    }

    private static void DemoBoxingToHeap()
    {
        Console.WriteLine("-- 装箱：值拷贝进堆上的盒子 --");
        int i = 5;
        object o = i;
        i = 99;
        Debug.Assert((int)o == 5);
        Console.WriteLine($"  原值改后盒子仍是 {(int)o}");
    }

    private static void DemoFiveScenariosSummary()
    {
        Console.WriteLine("-- 五场景速记 --");
        Console.WriteLine("  ① 方法局部 int → 通常栈");
        Console.WriteLine("  ② class 的 int 字段 → 堆内联");
        Console.WriteLine("  ③ int[] 元素 → 堆数组内联");
        Console.WriteLine("  ④ lambda 捕获局部 int → 堆闭包");
        Console.WriteLine("  ⑤ 装箱后的 int → 堆盒子");
        Debug.Assert(true);
    }

    private struct PointV
    {
        public int X, Y;
    }

    private sealed class Holder
    {
        public PointV P;
    }
}
