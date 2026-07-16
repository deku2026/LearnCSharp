// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第3部分-泛型与泛型数学.md
// Stage    : Stage02_TypeSystem
// Section  : Section03_GenericsAndGenericMath
// Item     : GenericConstraints
// Topic id : stage02/section03/generic_constraints
//
// 步骤 2：where 约束——解锁对 T 的操作。

using System.Diagnostics;
using System.Runtime.InteropServices;
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
        DemoDefaultOfT();
        DemoUnmanagedConstraint();
        DemoAllowsRefStruct();
        DemoMultiWhereClauses();
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

    private static void DemoDefaultOfT()
    {
        Console.WriteLine("-- default(T)：class → null，struct → 零初始化 --");
        Debug.Assert(DefaultOfClass<string>() is null);
        Debug.Assert(DefaultOfClass<object>() is null);
        int zero = DefaultOfStruct<int>();
        Point zeroPt = DefaultOfStruct<Point>();
        Debug.Assert(zero == 0);
        Debug.Assert(zeroPt.X == 0 && zeroPt.Y == 0);
        // 无 class/struct 约束时 default(T) 对引用是 null、对值是 zeroed
        Debug.Assert(DefaultAny<string>() is null);
        Debug.Assert(DefaultAny<int>() == 0);
        Console.WriteLine($"  default(string)=null; default(int)={zero}; default(Point)=({zeroPt.X},{zeroPt.Y})");
    }

    private static void DemoUnmanagedConstraint()
    {
        Console.WriteLine("-- where T : unmanaged（可 blittable / 指针友好） --");
        Debug.Assert(SizeOfUnmanaged<int>() == sizeof(int));
        Debug.Assert(SizeOfUnmanaged<Point>() == 8);
        Debug.Assert(SizeOfUnmanaged<byte>() == 1);
        // string / 含引用字段的类型不能作 unmanaged
        Console.WriteLine($"  sizeof unmanaged Point={SizeOfUnmanaged<Point>()}");
    }

    private static void DemoAllowsRefStruct()
    {
        Console.WriteLine("-- where T : allows ref struct（C# 13+，可接受 Span 等） --");
        Span<int> span = stackalloc int[3] { 1, 2, 3 };
        // Span is a ref struct — cannot satisfy ordinary where T : struct + interface boxing constraints.
        Debug.Assert(AcceptsRefStruct(span));
        int sum = SumSpan(span);
        Debug.Assert(sum == 6);
        Debug.Assert(SumSpan(new int[] { 4, 5 }.AsSpan()) == 9);
        Console.WriteLine($"  allows ref struct accepts Span; SumSpan(stackalloc)={sum}");
    }

    private static void DemoMultiWhereClauses()
    {
        Console.WriteLine("-- 多 where：多个类型参数各自约束 --");
        var factory = CreatePair<Plain, int>();
        Debug.Assert(factory.Item1 is not null);
        Debug.Assert(factory.Item2 == 0);
        string named = NameAndSize<Dog, Point>(new Dog("Rex"));
        Debug.Assert(named.StartsWith("Dog:", StringComparison.Ordinal));
        Debug.Assert(named.Contains("8", StringComparison.Ordinal));
        Console.WriteLine($"  CreatePair / NameAndSize => {named}");
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

    private static T? DefaultOfClass<T>() where T : class => default;

    private static T DefaultOfStruct<T>() where T : struct => default;

    private static T? DefaultAny<T>() => default;

    private static unsafe int SizeOfUnmanaged<T>() where T : unmanaged
        => sizeof(T);

    private static bool AcceptsRefStruct<T>(T _) where T : allows ref struct => true;

    private static int SumSpan(ReadOnlySpan<int> values)
    {
        int acc = 0;
        foreach (int n in values)
            acc += n;
        return acc;
    }

    private static (T Left, U Right) CreatePair<T, U>()
        where T : class, new()
        where U : struct
        => (new T(), default);

    private static string NameAndSize<TAnimal, TUnmanaged>(TAnimal animal)
        where TAnimal : Animal
        where TUnmanaged : unmanaged
        => $"{animal.GetType().Name}:{animal.Name}/unmanagedSize={Marshal.SizeOf<TUnmanaged>()}";

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

    private struct Point
    {
        public int X, Y;
    }
}
