// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第4部分-面向内存的类型.md
// Stage    : Stage02_TypeSystem
// Section  : Section04_MemoryOrientedTypes
// Item     : RefFamilyAndDefensiveCopy
// Topic id : stage02/section04/ref_family_and_defensive_copy
//
// 步骤 4：ref/in/ref readonly、ref struct、防御性拷贝、可变 struct 陷阱。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section04;

internal static class RefFamilyAndDefensiveCopy
{
    [LearnTopic("stage02/section04/ref_family_and_defensive_copy")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== RefFamilyAndDefensiveCopy ===");
        DemoRefParameter();
        DemoOutParameter();
        DemoInAndRefReadonly();
        DemoRefReturn();
        DemoDefensiveCopy();
        DemoMutableStructTrap();
        DemoRefStructBasics();
        return 0;
    }

    private static void DemoRefParameter()
    {
        Console.WriteLine("-- ref：按引用传，可改调用方变量 --");
        int x = 1;
        Increment(ref x);
        Debug.Assert(x == 2);
        Point p = new() { X = 1, Y = 2 };
        Move(ref p, 10, 0);
        Debug.Assert(p.X == 11);
        Console.WriteLine($"  ref int → {x}; ref Point → ({p.X},{p.Y})");
    }

    private static void DemoOutParameter()
    {
        Console.WriteLine("-- out：调用前可不赋值；方法内必须 definite assignment --");
        // out 参数在调用处视为“未初始化输出槽”，方法返回前必须全部赋值路径写满
        TryParsePositive("42", out int n);
        Debug.Assert(n == 42);
        bool fail = TryParsePositive("x", out int bad);
        Debug.Assert(!fail);
        Debug.Assert(bad == 0); // 失败路径也写了 out（约定清零）

        SplitName("Ada Lovelace", out string first, out string last);
        Debug.Assert(first == "Ada" && last == "Lovelace");

        // int unassigned; Use(ref unassigned); // ref 要求调用前已赋值
        // out 允许：
        DiscardOut(out _);
        Console.WriteLine($"  TryParsePositive(\"42\")={n}; SplitName → {first} {last}");
    }

    private static void DemoInAndRefReadonly()
    {
        Console.WriteLine("-- in / ref readonly：只读引用，避免大 struct 拷贝 --");
        BigPayload big = new(1, 2, 3, 4);
        int sum = SumIn(in big);
        Debug.Assert(sum == 10);
        ref readonly BigPayload ro = ref big;
        // ro.A = 9; // 编译错误
        Debug.Assert(ro.A == 1);
        Console.WriteLine($"  SumIn={sum}（不拷贝大结构）");
    }

    private static void DemoRefReturn()
    {
        Console.WriteLine("-- ref 返回：返回存储位置而非拷贝 --");
        int[] arr = [10, 20, 30];
        ref int slot = ref GetRef(arr, 1);
        slot = 99;
        Debug.Assert(arr[1] == 99);
        Console.WriteLine($"  ref return 改 arr[1]={arr[1]}");
    }

    private static void DemoDefensiveCopy()
    {
        Console.WriteLine("-- ⚠ 防御性拷贝：readonly 上调可变 struct 方法 --");
        var holder = new ReadonlyHolder(new Counter { Value = 0 });
        holder.Bump(); // 在只读字段上调可变方法 → 编译器先拷贝再调，原字段不变
        Debug.Assert(holder.C.Value == 0);
        Console.WriteLine($"  readonly Counter 上 Bump 后仍是 {holder.C.Value}（拷贝被改）");

        // 对比：非 readonly 字段会被真正修改
        var mut = new MutableHolder(new Counter { Value = 0 });
        mut.Bump();
        Debug.Assert(mut.C.Value == 1);
        Console.WriteLine($"  非 readonly 字段 Bump 后={mut.C.Value}");
    }

    private static void DemoMutableStructTrap()
    {
        Console.WriteLine("-- 可变 struct 陷阱：属性返回拷贝 --");
        var bag = new CounterBag();
        bag.Current.Bump(); // Current 属性返回拷贝，Bump 改的是临时副本
        Debug.Assert(bag.Current.Value == 0);
        bag.BumpInPlace();
        Debug.Assert(bag.Current.Value == 1);
        Console.WriteLine("  属性 get 返回 struct 是拷贝；应用 ref 返回或改为 class/不可变");
    }

    private static void DemoRefStructBasics()
    {
        Console.WriteLine("-- ref struct 只能在栈上 --");
        StackOnly s = new(42);
        Debug.Assert(s.Value == 42);
        // object o = s; // 不能装箱
        // StackOnly[] arr = new StackOnly[1]; // 不能作数组元素
        Console.WriteLine("  Span 即 ref struct；限制换取零堆保证");
    }

    private static void Increment(ref int n) => n++;

    private static void Move(ref Point p, int dx, int dy)
    {
        p.X += dx;
        p.Y += dy;
    }

    private static bool TryParsePositive(string text, out int value)
    {
        // 编译器强制：所有返回路径必须给 value 赋值
        if (int.TryParse(text, out int parsed) && parsed > 0)
        {
            value = parsed;
            return true;
        }
        value = 0;
        return false;
    }

    private static void SplitName(string full, out string first, out string last)
    {
        int sp = full.IndexOf(' ');
        if (sp < 0)
        {
            first = full;
            last = "";
            return;
        }
        first = full[..sp];
        last = full[(sp + 1)..];
    }

    private static void DiscardOut(out int sink) => sink = 0;

    private static int SumIn(in BigPayload p) => p.A + p.B + p.C + p.D;

    private static ref int GetRef(int[] arr, int i) => ref arr[i];

    private struct Point
    {
        public int X, Y;
    }

    private readonly struct BigPayload(int a, int b, int c, int d)
    {
        public int A { get; } = a;
        public int B { get; } = b;
        public int C { get; } = c;
        public int D { get; } = d;
    }

    private struct Counter
    {
        public int Value;
        public void Bump() => Value++;
    }

    private readonly struct ReadonlyHolder(Counter c)
    {
        public readonly Counter C = c;
        public void Bump() => C.Bump(); // 防御性拷贝
    }

    private struct MutableHolder(Counter c)
    {
        public Counter C = c;
        public void Bump() => C.Bump();
    }

    private sealed class CounterBag
    {
        private Counter _c;
        public Counter Current => _c; // 返回拷贝
        public void BumpInPlace() => _c.Bump();
    }

    private ref struct StackOnly(int value)
    {
        public int Value { get; } = value;
    }
}
