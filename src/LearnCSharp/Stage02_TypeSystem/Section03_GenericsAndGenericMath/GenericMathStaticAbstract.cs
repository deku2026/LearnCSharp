// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第3部分-泛型与泛型数学.md
// Stage    : Stage02_TypeSystem
// Section  : Section03_GenericsAndGenericMath
// Item     : GenericMathStaticAbstract
// Topic id : stage02/section03/generic_math_static_abstract
//
// 步骤 5：static abstract 接口成员 + INumber<T> 泛型数学。

using System.Diagnostics;
using System.Numerics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section03;

internal static class GenericMathStaticAbstract
{
    [LearnTopic("stage02/section03/generic_math_static_abstract")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GenericMathStaticAbstract ===");
        DemoINumberSum();
        DemoStaticAbstractInterface();
        DemoWhyItExists();
        DemoCustomNumericLike();
        return 0;
    }

    private static void DemoINumberSum()
    {
        Console.WriteLine("-- INumber<T>：对 T 做算术 --");
        Debug.Assert(Sum(1, 2, 3) == 6);
        Debug.Assert(Sum(1.5, 2.5) == 4.0);
        Debug.Assert(Sum(1.1m, 2.2m) == 3.3m);
        Console.WriteLine($"  Sum(int)={Sum(1, 2, 3)}; Sum(double)={Sum(1.5, 2.5)}");
    }

    private static void DemoStaticAbstractInterface()
    {
        Console.WriteLine("-- static abstract 接口成员 --");
        Debug.Assert(Combine<AddOps, int>(3, 4) == 7);
        Debug.Assert(Combine<MulOps, int>(3, 4) == 12);
        Console.WriteLine($"  Combine Add={Combine<AddOps, int>(3, 4)}; Mul={Combine<MulOps, int>(3, 4)}");
    }

    private static void DemoWhyItExists()
    {
        Console.WriteLine("-- 为什么：以前不能对无约束 T 写 a+b --");
        // static T Bad<T>(T a, T b) => a + b; // 编译错误：T 无 +
        // 泛型数学用 static abstract operator 把运算符变成接口契约
        Debug.Assert(Average(2, 4, 6) == 4);
        Console.WriteLine("  INumber/IAdditionOperators 解锁 + - * / 等，且零装箱（值类型特化）");
    }

    private static void DemoCustomNumericLike()
    {
        Console.WriteLine("-- 自定义实现 IAdditionOperators --");
        var a = new Score(10);
        var b = new Score(5);
        var c = AddScores(a, b);
        Debug.Assert(c.Value == 15);
        Console.WriteLine($"  Score 10+5={c.Value}");
    }

    private static T Sum<T>(params T[] values) where T : INumber<T>
    {
        T acc = T.Zero;
        foreach (var v in values)
            acc += v;
        return acc;
    }

    private static T Average<T>(params T[] values) where T : INumber<T>
    {
        T sum = Sum(values);
        return sum / T.CreateChecked(values.Length);
    }

    private static T Combine<TOps, T>(T a, T b)
        where TOps : IBinaryOps<T>
        where T : INumber<T>
        => TOps.Apply(a, b);

    private static TAdd AddScores<TAdd>(TAdd a, TAdd b)
        where TAdd : IAdditionOperators<TAdd, TAdd, TAdd>
        => a + b;

    private interface IBinaryOps<T> where T : INumber<T>
    {
        static abstract T Apply(T a, T b);
    }

    private readonly struct AddOps : IBinaryOps<int>
    {
        public static int Apply(int a, int b) => a + b;
    }

    private readonly struct MulOps : IBinaryOps<int>
    {
        public static int Apply(int a, int b) => a * b;
    }

    private readonly struct Score : IAdditionOperators<Score, Score, Score>
    {
        public int Value { get; }
        public Score(int value) => Value = value;
        public static Score operator +(Score left, Score right) => new(left.Value + right.Value);
    }
}
