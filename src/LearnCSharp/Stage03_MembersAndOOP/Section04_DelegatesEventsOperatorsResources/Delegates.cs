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
        DemoMulticastThrowStopsRest();
        DemoSafeInvokeLoop();
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
        StringBuilder sb = new StringBuilder();
        Action pipeline = () => sb.Append('A');
        pipeline += () => sb.Append('B');
        pipeline += () => sb.Append('C');
        pipeline();
        Debug.Assert(sb.ToString() == "ABC");
        Console.WriteLine($"  pipeline() => {sb}");
    }

    private static void DemoMulticastThrowStopsRest()
    {
        Console.WriteLine("-- 多播陷阱：一个 handler 抛异常 → 后续不再执行 --");
        StringBuilder log = new StringBuilder();
        Action chain = () => log.Append('A');
        chain += () =>
        {
            log.Append('B');
            throw new InvalidOperationException("boom-in-handler");
        };
        chain += () => log.Append('C'); // 不会跑到

        try
        {
            chain(); // 直接 Invoke：异常在调用点冒出，C 被跳过
            Debug.Assert(false, "expected throw");
        }
        catch (InvalidOperationException ex)
        {
            Debug.Assert(ex.Message == "boom-in-handler");
        }

        Debug.Assert(log.ToString() == "AB");
        Console.WriteLine($"  log after throw-at-invoke-site='{log}'（C 未执行）");
    }

    private static void DemoSafeInvokeLoop()
    {
        Console.WriteLine("-- 对照：GetInvocationList 逐个 try/catch → 其余 handler 仍跑 --");
        StringBuilder log = new StringBuilder();
        Action chain = () => log.Append('A');
        chain += () =>
        {
            log.Append('B');
            throw new InvalidOperationException("boom-in-handler");
        };
        chain += () => log.Append('C');

        int failures = 0;
        foreach (Delegate d in chain.GetInvocationList())
        {
            try
            {
                ((Action)d)();
            }
            catch (InvalidOperationException)
            {
                failures++;
            }
        }

        Debug.Assert(failures == 1);
        Debug.Assert(log.ToString() == "ABC");
        Console.WriteLine($"  safe loop log='{log}', failures={failures}");
    }

    private static void DemoInvocationList()
    {
        Console.WriteLine("-- GetInvocationList 逐个调用 --");
        Func<int> f = () => 1;
        f += () => 2;
        f += () => 3;
        Delegate[] list = f.GetInvocationList();
        Debug.Assert(list.Length == 3);
        Debug.Assert(f() == 3); // 多播有返回值时只保留最后一个
        int sum = 0;
        foreach (Delegate d in list)
            sum += ((Func<int>)d)();
        Debug.Assert(sum == 6);
        Console.WriteLine($"  last return={f()}, sum via list={sum}");
    }

    private static int Add(int a, int b) => a + b;
    private static int Mul(int a, int b) => a * b;
}
