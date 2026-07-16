// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第1部分-数据成员与封装.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section01_DataMembersAndEncapsulation
// Item     : StaticMembersAndTypes
// Topic id : stage03/section01/static_members_and_types
//
// 步骤 5：静态字段/方法/属性、静态构造(.cctor)、静态类、泛型静态独立。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section01;

internal static class StaticMembersAndTypes
{
    [LearnTopic("stage03/section01/static_members_and_types")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StaticMembersAndTypes ===");
        DemoSharedStaticField();
        DemoStaticConstructorOnce();
        DemoStaticClass();
        DemoGenericStaticPerClosedType();
        return 0;
    }

    private static void DemoSharedStaticField()
    {
        Console.WriteLine("-- 静态字段：所有实例共享 --");
        Counter.Reset();
        var a = new Counter();
        var b = new Counter();
        Debug.Assert(a.Id == 1 && b.Id == 2);
        Debug.Assert(Counter.Total == 2);
        Debug.Assert(Counter.Doubled == 4);
        Console.WriteLine($"  Total={Counter.Total}, Doubled={Counter.Doubled}");
    }

    private static void DemoStaticConstructorOnce()
    {
        Console.WriteLine("-- 静态构造：首次使用前跑一次 --");
        Debug.Assert(Settings.CctorRuns == 1); // 首次访问触发
        _ = Settings.Env;
        Debug.Assert(Settings.CctorRuns == 1); // 仍为 1
        Console.WriteLine($"  Env={Settings.Env}, CctorRuns={Settings.CctorRuns}");
    }

    private static void DemoStaticClass()
    {
        Console.WriteLine("-- 静态类：只能静态成员，不能 new --");
        double y = MathUtils.Square(3);
        Debug.Assert(Math.Abs(y - 9) < 1e-9);
        Console.WriteLine($"  MathUtils.Square(3)={y}");
        // var x = new MathUtils(); // ❌
    }

    private static void DemoGenericStaticPerClosedType()
    {
        Console.WriteLine("-- 泛型：每个封闭类型各有一份静态字段 --");
        Slot<int>.Value = 10;
        Slot<string>.Value = 20;
        Debug.Assert(Slot<int>.Value == 10);
        Debug.Assert(Slot<string>.Value == 20);
        Console.WriteLine($"  Slot<int>={Slot<int>.Value}, Slot<string>={Slot<string>.Value}");
    }

    private sealed class Counter
    {
        public static int Total;
        public int Id { get; }
        public Counter() => Id = ++Total;
        public static void Reset() => Total = 0;
        public static int Doubled => Total * 2;
    }

    private static class Settings
    {
        public static int CctorRuns;
        public static readonly string Env;
        static Settings()
        {
            CctorRuns++;
            Env = Environment.GetEnvironmentVariable("LEARN_CSHARP_ENV") ?? "dev";
        }
    }

    private static class MathUtils
    {
        public static double Square(double x) => x * x;
    }

    private static class Slot<T>
    {
        public static int Value;
    }
}
