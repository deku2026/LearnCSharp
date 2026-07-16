// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第3部分-泛型与泛型数学.md
// Stage    : Stage02_TypeSystem
// Section  : Section03_GenericsAndGenericMath
// Item     : GenericConstraints
// Topic id : stage02/section03/generic_constraints
//
// 步骤 2：where 约束——解锁对 T 的操作。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section03;

internal static class GenericConstraints
{
    [LearnTopic("stage02/section03/generic_constraints")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GenericConstraints ===");
        DemoComparableConstraint();
        DemoNewConstraint();
        DemoClassStructNotnull();
        DemoInterfaceAndBase();
        DemoEnumConstraint();
        return 0;
    }

    private static void DemoComparableConstraint()
    {
        Console.WriteLine("-- where T : IComparable<T> --");
        Debug.Assert(Max(3, 9) == 9);
        Debug.Assert(Max("a", "z") == "z");
        Console.WriteLine($"  Max(3,9)={Max(3, 9)}");
    }

    private static void DemoNewConstraint()
    {
        Console.WriteLine("-- where T : new() --");
        var p = Create<Plain>();
        Debug.Assert(p is not null);
        Console.WriteLine($"  Create<Plain>() type={p.GetType().Name}");
    }

    private static void DemoClassStructNotnull()
    {
        Console.WriteLine("-- class / struct / notnull --");
        Debug.Assert(IsReference("hi"));
        Debug.Assert(IsValue(42));
        Debug.Assert(NotNullLen("ab") == 2);
        Console.WriteLine("  class 约束可 as/判 null；struct 约束保证值类型");
    }

    private static void DemoInterfaceAndBase()
    {
        Console.WriteLine("-- 基类/接口约束 --");
        var d = new Dog("Rex");
        Debug.Assert(Describe(d) == "Dog:Rex");
        Console.WriteLine($"  Describe(Dog)={Describe(d)}");
    }

    private static void DemoEnumConstraint()
    {
        Console.WriteLine("-- where T : Enum --");
        Debug.Assert(EnumName(DayOfWeek.Monday) == "Monday");
        Console.WriteLine($"  EnumName={EnumName(DayOfWeek.Monday)}");
    }

    private static T Max<T>(T a, T b) where T : IComparable<T>
        => a.CompareTo(b) >= 0 ? a : b;

    private static T Create<T>() where T : new() => new();

    private static bool IsReference<T>(T _) where T : class => true;

    private static bool IsValue<T>(T _) where T : struct => true;

    private static int NotNullLen<T>(T value) where T : notnull
        => value.ToString()!.Length;

    private static string Describe<T>(T animal) where T : Animal
        => $"{animal.GetType().Name}:{animal.Name}";

    private static string EnumName<T>(T value) where T : struct, Enum
        => value.ToString();

    private sealed class Plain;

    private abstract class Animal(string name)
    {
        public string Name { get; } = name;
    }

    private sealed class Dog(string name) : Animal(name);
}
