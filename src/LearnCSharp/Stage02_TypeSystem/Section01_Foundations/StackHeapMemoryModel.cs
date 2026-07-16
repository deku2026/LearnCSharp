// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第1部分-地基.md
// Stage    : Stage02_TypeSystem
// Section  : Section01_Foundations
// Item     : StackHeapMemoryModel
// Topic id : stage02/section01/stack_heap_memory_model
//
// 步骤 3：破除“值类型在栈、引用类型在堆”——存哪取决于上下文。

using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        // 纯局部值类型运算：不经 object 形参（否则会装箱）
        long pureValue = MeasureAlloc(static () =>
        {
            int acc = 0;
            for (int i = 0; i < 100; i++)
                acc += i;
            Blackhole(acc);
        });
        long withHeapObj = MeasureAlloc(static () =>
        {
            var h = new Holder { P = new PointV { X = 1, Y = 2 } };
            Consume(h);
        });
        Debug.Assert(withHeapObj > pureValue);
        Console.WriteLine($"  局部循环分配≈{pureValue}B；new Holder≈{withHeapObj}B");
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
        long arrAlloc = MeasureAlloc(static () =>
        {
            int[] a = new int[32];
            a[0] = 1;
            Consume(a);
        });
        Debug.Assert(arrAlloc > 0);
        Console.WriteLine($"  int[] Length={arr.Length}; new int[32] 分配≈{arrAlloc}B（堆数组）");
    }

    private static void DemoClosureCapturesToHeap()
    {
        Console.WriteLine("-- 闭包捕获局部：提升到堆上的闭包对象 --");
        long closureAlloc = MeasureAlloc(static () =>
        {
            int captured = 10;
            Func<int> f = () => captured + 1;
            captured = 20;
            Debug.Assert(f() == 21);
            Consume(f);
        });
        Debug.Assert(closureAlloc > 0);
        Console.WriteLine($"  lambda 捕获分配≈{closureAlloc}B —— 局部变成闭包字段（堆）");
    }

    private static void DemoBoxingToHeap()
    {
        Console.WriteLine("-- 装箱：值拷贝进堆上的盒子 --");
        int i = 5;
        long boxAlloc = MeasureAlloc(() =>
        {
            object o = i;
            Debug.Assert((int)o == 5);
            Consume(o);
        });
        i = 99;
        Debug.Assert(boxAlloc > 0);
        Console.WriteLine($"  装箱分配≈{boxAlloc}B；原值改后盒子仍是独立拷贝");
    }

    private static void DemoFiveScenariosSummary()
    {
        Console.WriteLine("-- 五场景速记 --");
        Console.WriteLine("  ① 方法局部 int → 通常栈");
        Console.WriteLine("  ② class 的 int 字段 → 堆内联");
        Console.WriteLine("  ③ int[] 元素 → 堆数组内联");
        Console.WriteLine("  ④ lambda 捕获局部 int → 堆闭包");
        Console.WriteLine("  ⑤ 装箱后的 int → 堆盒子");
        // 可测量：装箱 vs 不装箱
        long boxed = MeasureAlloc(static () =>
        {
            object[] boxes = new object[16];
            for (int i = 0; i < boxes.Length; i++)
                boxes[i] = i;
            Consume(boxes);
        });
        long unboxed = MeasureAlloc(static () =>
        {
            int[] vals = new int[16];
            for (int i = 0; i < vals.Length; i++)
                vals[i] = i;
            Consume(vals);
        });
        Debug.Assert(boxed > unboxed);
        Console.WriteLine($"  object[16] 装箱写入≈{boxed}B > int[16]≈{unboxed}B");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long MeasureAlloc(Action action)
    {
        action(); // warmup
        long before = GC.GetAllocatedBytesForCurrentThread();
        action();
        long after = GC.GetAllocatedBytesForCurrentThread();
        return after - before;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Consume(object o) => GC.KeepAlive(o);

    private static int s_blackhole;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Blackhole(int value) => s_blackhole = value;

    private struct PointV
    {
        public int X, Y;
    }

    private sealed class Holder
    {
        public PointV P;
    }
}
