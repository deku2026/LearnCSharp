// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第4部分-委托事件运算符资源管理.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section04_DelegatesEventsOperatorsResources
// Item     : Delegates
// Topic id : stage03/section04/delegates
//
// 步骤 1：自定义 delegate、Action/Func/Predicate、多播 Combine/Remove。

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section04;

internal static class Delegates
{
    [LearnTopic("stage03/section04/delegates")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Delegates ===");
        DemoCustomDelegate();
        DemoFuncActionPredicate();
        DemoMulticast();
        DemoInvocationList();
        return 0;
    }

    private delegate int BinaryOp(int a, int b);

    private static void DemoCustomDelegate()
    {
        Console.WriteLine("-- 自定义 delegate 类型 --");
        BinaryOp op = Add;
        Debug.Assert(op(3, 4) == 7);
        op = Mul;
        Debug.Assert(op(3, 4) == 12);
        Console.WriteLine($"  Add(3,4) then Mul(3,4) via BinaryOp");
    }

    private static void DemoFuncActionPredicate()
    {
        Console.WriteLine("-- Func / Action / Predicate --");
        Func<int, int, int> add = (a, b) => a + b;
        Func<string, int> len = s => s.Length;
        Action<string> print = s => { /* side-effect free for assert */ _ = s; };
        Action greet = () => { };
        Predicate<int> isEven = n => n % 2 == 0;
        Debug.Assert(add(2, 3) == 5);
        Debug.Assert(len("hi") == 2);
        Debug.Assert(isEven(4) && !isEven(5));
        print("x");
        greet();
        Console.WriteLine($"  Func add=5, len=2, Predicate isEven(4)={isEven(4)}");
    }

    private static void DemoMulticast()
    {
        Console.WriteLine("-- 多播：+= / -= --");
        var sb = new StringBuilder();
        Action pipeline = () => sb.Append('A');
        pipeline += () => sb.Append('B');
        pipeline += () => sb.Append('C');
        pipeline();
        Debug.Assert(sb.ToString() == "ABC");
        Console.WriteLine($"  pipeline() => {sb}");
    }

    private static void DemoInvocationList()
    {
        Console.WriteLine("-- GetInvocationList 逐个调用 --");
        Func<int> f = () => 1;
        f += () => 2;
        f += () => 3;
        var list = f.GetInvocationList();
        Debug.Assert(list.Length == 3);
        Debug.Assert(f() == 3); // 多播有返回值时只保留最后一个
        int sum = 0;
        foreach (var d in list)
            sum += ((Func<int>)d)();
        Debug.Assert(sum == 6);
        Console.WriteLine($"  last return={f()}, sum via list={sum}");
    }

    private static int Add(int a, int b) => a + b;
    private static int Mul(int a, int b) => a * b;
}
