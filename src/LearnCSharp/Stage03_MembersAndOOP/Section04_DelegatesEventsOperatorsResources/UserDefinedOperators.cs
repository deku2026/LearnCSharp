// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第4部分-委托事件运算符资源管理.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section04_DelegatesEventsOperatorsResources
// Item     : UserDefinedOperators
// Topic id : stage03/section04/user_defined_operators
//
// 步骤 4：运算符重载、转换运算符、C#14 就地 += 与实例 ++。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section04;

internal static class UserDefinedOperators
{
    [LearnTopic("stage03/section04/user_defined_operators")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== UserDefinedOperators ===");
        DemoBinaryUnaryCompare();
        DemoConversionOperators();
        DemoCompoundAssignmentInPlace();
        DemoInstanceIncrement();
        return 0;
    }

    private static void DemoBinaryUnaryCompare()
    {
        Console.WriteLine("-- 二元/一元/比较运算符 --");
        var a = new Vector2(1, 2);
        var b = new Vector2(3, 4);
        var sum = a + b;
        var scaled = a * 2;
        var neg = -a;
        Debug.Assert(sum == new Vector2(4, 6));
        Debug.Assert(scaled == new Vector2(2, 4));
        Debug.Assert(neg == new Vector2(-1, -2));
        Debug.Assert(a != b);
        Console.WriteLine($"  (1,2)+(3,4)={sum}, *2={scaled}, -a={neg}");
    }

    private static void DemoConversionOperators()
    {
        Console.WriteLine("-- 用户定义转换 --");
        var v = new Vector2(3, 4);
        double len = v; // implicit
        Debug.Assert(Math.Abs(len - 5) < 1e-9);
        Vector2 fromInt = (Vector2)5; // explicit
        Debug.Assert(fromInt == new Vector2(5, 0));
        Console.WriteLine($"  implicit length={len}, explicit (Vector2)5={fromInt}");
    }

    private static void DemoCompoundAssignmentInPlace()
    {
        Console.WriteLine("-- C#14 实例 += 就地 --");
        var v = new Vector3(1, 2, 3);
        v += new Vector3(4, 5, 6);
        Debug.Assert(v == new Vector3(5, 7, 9));
        var classic = new Vector3(1, 0, 0) + new Vector3(0, 1, 0);
        Debug.Assert(classic == new Vector3(1, 1, 0));
        Console.WriteLine($"  in-place += => {v}; static + => {classic}");
    }

    private static void DemoInstanceIncrement()
    {
        Console.WriteLine("-- C#14 实例 ++ 就地 --");
        var c = new Counter(10);
        c++;
        Debug.Assert(c.Value == 11);
        Console.WriteLine($"  Counter after ++ => {c.Value}");
    }

    private readonly struct Vector2(double x, double y)
    {
        public double X { get; } = x;
        public double Y { get; } = y;

        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator *(Vector2 v, double k) => new(v.X * k, v.Y * k);
        public static Vector2 operator -(Vector2 v) => new(-v.X, -v.Y);
        public static bool operator ==(Vector2 a, Vector2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);
        public override bool Equals(object? o) => o is Vector2 v && this == v;
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X},{Y})";

        public static implicit operator double(Vector2 v) => Math.Sqrt(v.X * v.X + v.Y * v.Y);
        public static explicit operator Vector2(int x) => new(x, 0);
    }

    private record struct Vector3(double X, double Y, double Z)
    {
        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        // C#14：实例复合赋值，就地修改
        public void operator +=(Vector3 r)
        {
            X += r.X;
            Y += r.Y;
            Z += r.Z;
        }
    }

    private record struct Counter(int Value)
    {
        public void operator ++() => Value++;
    }
}
