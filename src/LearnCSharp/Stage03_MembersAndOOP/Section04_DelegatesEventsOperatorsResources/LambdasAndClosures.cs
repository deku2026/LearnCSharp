// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第4部分-委托事件运算符资源管理.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section04_DelegatesEventsOperatorsResources
// Item     : LambdasAndClosures
// Topic id : stage03/section04/lambdas_and_closures
//
// 步骤 2：表达式/语句 lambda、闭包按变量捕获、static lambda。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section04;

internal static class LambdasAndClosures
{
    [LearnTopic("stage03/section04/lambdas_and_closures")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== LambdasAndClosures ===");
        DemoLambdaShapes();
        DemoClosureKeepsState();
        DemoCaptureByVariableNotValue();
        DemoStaticLambda();
        return 0;
    }

    private static void DemoLambdaShapes()
    {
        Console.WriteLine("-- 表达式 / 语句 / 多参数 lambda --");
        Func<int, int> square = x => x * x;
        Func<int, int, int> add = (a, b) => a + b;
        Action<string> log = s => { _ = $"[LOG] {s}"; };
        Func<int, int> withType = (int x) => x + 1;
        Debug.Assert(square(5) == 25);
        Debug.Assert(add(2, 3) == 5);
        Debug.Assert(withType(9) == 10);
        log("ok");
        Console.WriteLine($"  square(5)={square(5)}, add(2,3)={add(2, 3)}");
    }

    private static void DemoClosureKeepsState()
    {
        Console.WriteLine("-- 闭包保活外层变量 --");
        Func<int> counter = MakeCounter();
        Debug.Assert(counter() == 1);
        Debug.Assert(counter() == 2);
        Debug.Assert(counter() == 3);
        Console.WriteLine("  MakeCounter: 1,2,3");
    }

    private static void DemoCaptureByVariableNotValue()
    {
        Console.WriteLine("-- 按变量捕获(非快照值) --");
        int n = 100;
        Func<int, int> f = x => x + n;
        n = 200;
        Debug.Assert(f(1) == 201);
        // 模拟 C++ [=] 快照：引入临时
        int snapshot = n;
        Func<int, int> g = x => x + snapshot;
        n = 999;
        Debug.Assert(g(1) == 201);
        Console.WriteLine($"  f(1) after n=200 => 201; snapshot g(1)=201 after n=999");
    }

    private static void DemoStaticLambda()
    {
        Console.WriteLine("-- static lambda：禁止捕获 --");
        Func<int, bool> isEven = static x => x % 2 == 0;
        Debug.Assert(isEven(4) && !isEven(5));
        // static x => x + outer; // ❌ 不能捕获
        Console.WriteLine($"  static isEven(4)={isEven(4)}");
    }

    private static Func<int> MakeCounter()
    {
        int count = 0;
        return () => ++count;
    }
}
