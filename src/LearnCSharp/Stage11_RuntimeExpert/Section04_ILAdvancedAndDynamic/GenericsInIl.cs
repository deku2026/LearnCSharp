// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第4部分-IL高级与动态生成.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section04_ILAdvancedAndDynamic
// Item     : GenericsInIl
// Topic id : stage11/section04/generics_in_il
//
// Lesson: generic IL uses !T / !!T; shared code for ref T; reified for value T.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section04;

internal static class GenericsInIl
{
    [LearnTopic("stage11/section04/generics_in_il")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GenericsInIl ===");
        DemoGenericMethod();
        DemoConstrainedCallvirt();
        DemoSharedVsReified();
        return 0;
    }

    private static void DemoGenericMethod()
    {
        Console.WriteLine("-- generic method IL uses type parameters --");
        int a = Identity(42);
        string b = Identity("x");
        Debug.Assert(a == 42 && b == "x");
        MethodInfo open = typeof(GenericsInIl).GetMethod(nameof(Identity), BindingFlags.NonPublic | BindingFlags.Static)!;
        Console.WriteLine($"  Identity IsGenericMethod={open.IsGenericMethod}, def={open.GetGenericMethodDefinition().Name}");
        MethodInfo closed = open.MakeGenericMethod(typeof(int));
        Console.WriteLine($"  closed Identity<int> → {closed}");
        Debug.Assert((int)closed.Invoke(null, [99])! == 99);
    }

    private static void DemoConstrainedCallvirt()
    {
        Console.WriteLine("-- constrained.callvirt avoids boxing for struct virtuals --");
        // C# calls ToString/Equals/GetHashCode on T with constrained prefix when possible
        int h = HashOf(42);
        int h2 = HashOf("hi");
        Console.WriteLine($"  HashOf(42)={h}, HashOf(\"hi\")={h2}");
        Debug.Assert(h == 42.GetHashCode());
        Debug.Assert(h2 == "hi".GetHashCode());
        Console.WriteLine("  IL: constrained. !!T / callvirt instance int32 Object::GetHashCode()");
    }

    private static void DemoSharedVsReified()
    {
        Console.WriteLine("-- code sharing --");
        Box<string> listStr = new Box<string>("a");
        Box<object> listObj = new Box<object>("b");
        Box<int> listInt = new Box<int>(3);
        Console.WriteLine($"  Box<string>={listStr.Value}, Box<object>={listObj.Value}, Box<int>={listInt.Value}");
        Debug.Assert(listInt.Value == 3);
        Console.WriteLine("  Reference-type instantiations often share native code;");
        Console.WriteLine("  each value-type instantiation gets specialized layout/code.");
        Type tRef = typeof(Box<string>);
        Type tVal = typeof(Box<int>);
        Debug.Assert(tRef.GetGenericTypeDefinition() == tVal.GetGenericTypeDefinition());
        Debug.Assert(tRef != tVal);
    }

    private static T Identity<T>(T value) => value;

    private static int HashOf<T>(T value) where T : notnull => value.GetHashCode();

    private sealed class Box<T>(T value)
    {
        public T Value { get; } = value;
    }
}
