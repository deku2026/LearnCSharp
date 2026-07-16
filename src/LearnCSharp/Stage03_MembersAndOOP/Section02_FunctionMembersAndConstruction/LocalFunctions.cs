// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第2部分-函数成员与构造.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section02_FunctionMembersAndConstruction
// Item     : LocalFunctions
// Topic id : stage03/section02/local_functions
//
// 步骤 2：本地函数、static 本地、递归、校验+迭代器拆段、vs lambda。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section02;

internal static class LocalFunctions
{
    [LearnTopic("stage03/section02/local_functions")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== LocalFunctions ===");
        DemoRecursiveStaticLocal();
        DemoValidateThenIterator();
        DemoCaptureVsStatic();
        DemoLocalVsLambdaIntent();
        return 0;
    }

    private static void DemoRecursiveStaticLocal()
    {
        Console.WriteLine("-- static 本地函数递归 --");
        Debug.Assert(Factorial(5) == 120);
        Debug.Assert(Factorial(0) == 1);
        try
        {
            _ = Factorial(-1);
            Debug.Assert(false);
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("  Factorial(-1) throws");
        }
        Console.WriteLine($"  Factorial(5)={Factorial(5)}");
    }

    private static void DemoValidateThenIterator()
    {
        Console.WriteLine("-- 立即校验 + 惰性迭代器(本地函数拆段) --");
        try
        {
            _ = EvensUpTo(-1);
            Debug.Assert(false);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("  EvensUpTo(-1) throws at call site (not on enumerate)");
        }

        var list = EvensUpTo(6).ToList();
        Debug.Assert(list is [0, 2, 4, 6]);
        Console.WriteLine($"  EvensUpTo(6)=[{string.Join(',', list)}]");
    }

    private static void DemoCaptureVsStatic()
    {
        Console.WriteLine("-- 捕获外层 vs static 禁止捕获 --");
        int factor = 3;
        int WithCapture(int x) => x * factor; // 捕获 factor
        static int NoCapture(int x, int f) => x * f;
        Debug.Assert(WithCapture(4) == 12);
        Debug.Assert(NoCapture(4, factor) == 12);
        Console.WriteLine($"  capture={WithCapture(4)}, static={NoCapture(4, factor)}");
    }

    private static void DemoLocalVsLambdaIntent()
    {
        Console.WriteLine("-- 本地函数不分配委托(除非转委托) --");
        int LocalAdd(int a, int b) => a + b;
        Func<int, int, int> asDelegate = LocalAdd;
        Debug.Assert(LocalAdd(2, 3) == 5);
        Debug.Assert(asDelegate(2, 3) == 5);
        Console.WriteLine("  直接调本地函数无委托；赋给 Func 才分配委托");
    }

    private static int Factorial(int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
        return Compute(n);

        static int Compute(int k) => k <= 1 ? 1 : k * Compute(k - 1);
    }

    private static IEnumerable<int> EvensUpTo(int max)
    {
        Validate();
        return Iterator();

        void Validate()
        {
            if (max < 0) throw new ArgumentException("max < 0", nameof(max));
        }

        IEnumerable<int> Iterator()
        {
            for (int i = 0; i <= max; i += 2)
                yield return i;
        }
    }
}
