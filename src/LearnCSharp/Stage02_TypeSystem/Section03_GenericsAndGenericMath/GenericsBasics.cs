// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第3部分-泛型与泛型数学.md
// Stage    : Stage02_TypeSystem
// Section  : Section03_GenericsAndGenericMath
// Item     : GenericsBasics
// Topic id : stage02/section03/generics_basics
//
// 步骤 1：泛型类型/方法、类型推断、类型安全、消除装箱。

using System.Collections;
using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section03;

internal static class GenericsBasics
{
    [LearnTopic("stage02/section03/generics_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GenericsBasics ===");
        DemoGenericType();
        DemoGenericMethodInference();
        DemoTypeSafety();
        DemoNoBoxing();
        DemoInvariantByDefault();
        return 0;
    }

    private static void DemoGenericType()
    {
        Console.WriteLine("-- 泛型类一份定义 --");
        Box<int> bi = new Box<int>(42);
        Box<string> bs = new Box<string>("hi");
        Debug.Assert(bi.Value == 42 && bs.Value == "hi");
        Console.WriteLine($"  Box<int>={bi.Value}, Box<string>={bs.Value}");
    }

    private static void DemoGenericMethodInference()
    {
        Console.WriteLine("-- 泛型方法 + 类型推断 --");
        int x = 1, y = 2;
        Swap(ref x, ref y);
        Debug.Assert(x == 2 && y == 1);
        Console.WriteLine($"  Swap → x={x}, y={y}");
    }

    private static void DemoTypeSafety()
    {
        Console.WriteLine("-- 编译期类型安全 --");
        List<int> list = [1, 2];
        // list.Add("no"); // 编译错误
        Debug.Assert(list[0] == 1);
        Console.WriteLine("  List<int> 编译期拦住 string");
    }

    private static void DemoNoBoxing()
    {
        Console.WriteLine("-- List<T> 值类型零装箱 vs ArrayList --");
        List<int> typed = [1, 2, 3];
        ArrayList legacy = [1, 2, 3]; // 装箱
        Debug.Assert(typed[0] == 1);
        Debug.Assert((int)legacy[0]! == 1);
        Console.WriteLine("  List<int>.Add 无 box；ArrayList.Add(int) 有 box");
    }

    private static void DemoInvariantByDefault()
    {
        Console.WriteLine("-- 默认不变：List<Derived> 不是 List<Base> --");
        List<string> strings = ["a"];
        // List<object> objs = strings; // 编译错误
        Debug.Assert(strings[0] == "a");
        Console.WriteLine("  泛型默认 invariant（协变/逆变见后续课）");
    }

    private static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);

    private sealed class Box<T>(T value)
    {
        public T Value { get; } = value;
    }
}
