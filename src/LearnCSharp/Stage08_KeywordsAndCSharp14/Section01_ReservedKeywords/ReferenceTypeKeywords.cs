// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : ReferenceTypeKeywords (二、引用类型关键字 — 6 个)
// Topic id : stage08/section01/reference_type_keywords
//
// class / interface / delegate / object / string / void。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class ReferenceTypeKeywords
{
    [LearnTopic("stage08/section01/reference_type_keywords")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReferenceTypeKeywords ===");
        DemoClassAndInterface();
        DemoDelegate();
        DemoObjectAndString();
        DemoVoid();
        return 0;
    }

    private static void DemoClassAndInterface()
    {
        Console.WriteLine("-- class / interface --");
        IGreeter g = new HelloGreeter("Ada");
        Debug.Assert(g.Greet() == "Hello, Ada");
        Debug.Assert(typeof(HelloGreeter).IsClass);
        Debug.Assert(typeof(IGreeter).IsInterface);
        var a = new HelloGreeter("A");
        var b = a; // 引用拷贝
        Debug.Assert(ReferenceEquals(a, b));
        Console.WriteLine($"  Greet={g.Greet()}, IsClass={typeof(HelloGreeter).IsClass}");
    }

    private static void DemoDelegate()
    {
        Console.WriteLine("-- delegate --");
        IntTransform t = Square;
        t += Double;
        int r = t(3); // 多播：最后一个返回值
        Debug.Assert(r == 6);
        Debug.Assert(typeof(IntTransform).IsSubclassOf(typeof(Delegate))
                     || typeof(IntTransform).BaseType == typeof(MulticastDelegate));
        Console.WriteLine($"  multicast last return={r}");
    }

    private static void DemoObjectAndString()
    {
        Console.WriteLine("-- object / string --");
        object box = 42;
        string s = "hi";
        string s2 = "hi";
        Debug.Assert(box is int);
        Debug.Assert(typeof(object) == typeof(System.Object));
        Debug.Assert(typeof(string) == typeof(System.String));
        Debug.Assert(s == s2); // 值相等
        Debug.Assert(string.IsInterned(s) is not null || true);
        Console.WriteLine($"  object box={box}, string equal={s == s2}");
    }

    private static void DemoVoid()
    {
        Console.WriteLine("-- void --");
        SideEffect(7);
        MethodInfo? m = typeof(ReferenceTypeKeywords).GetMethod(
            nameof(SideEffect),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Debug.Assert(m is not null);
        Debug.Assert(m.ReturnType == typeof(void));
        Console.WriteLine($"  SideEffect return type={m.ReturnType.Name}");
    }

    private static int Square(int x) => x * x;
    private static int Double(int x) => x * 2;
    private static void SideEffect(int x) => Console.WriteLine($"  side-effect x={x}");

    private interface IGreeter
    {
        string Greet();
    }

    private sealed class HelloGreeter(string name) : IGreeter
    {
        public string Greet() => $"Hello, {name}";
    }

    private delegate int IntTransform(int x);
}
