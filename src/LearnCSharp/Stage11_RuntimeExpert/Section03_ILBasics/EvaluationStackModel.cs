// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第3部分-IL中间语言基础.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section03_ILBasics
// Item     : EvaluationStackModel
// Topic id : stage11/section03/evaluation_stack_model
//
// Lesson: IL ops push/pop evaluation stack; maxstack verified; locals separate.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section03;

internal static class EvaluationStackModel
{
    [LearnTopic("stage11/section03/evaluation_stack_model")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== EvaluationStackModel ===");
        DemoStackDepth();
        DemoLocalsVsStack();
        DemoDupAndPopPatterns();
        return 0;
    }

    private static void DemoStackDepth()
    {
        Console.WriteLine("-- evaluation stack depth --");
        // Expression ((1+2)*(3+4)) needs temporary stack depth
        int v = (1 + 2) * (3 + 4);
        Debug.Assert(v == 21);
        Console.WriteLine($"  (1+2)*(3+4)={v}");
        Console.WriteLine("  Typical IL: ldc.i4.1 ldc.i4.2 add  ldc.i4.3 ldc.i4.4 add  mul");
        Console.WriteLine("  Verifier ensures stack depth never exceeds .maxstack and types match.");
    }

    private static void DemoLocalsVsStack()
    {
        Console.WriteLine("-- locals (stloc/ldloc) vs pure stack --");
        int local = 0;
        local = 5;
        local = local + 7;
        Debug.Assert(local == 12);
        Console.WriteLine($"  local ends at {local}");
        Console.WriteLine("  Locals survive across statements; evaluation stack is transient per sequence.");
        Console.WriteLine("  C# temps for complex expressions often become compiler-generated locals.");
    }

    private static void DemoDupAndPopPatterns()
    {
        Console.WriteLine("-- dup / pop style patterns in C# --");
        // field++ conceptually: load addr, dup, ldind, ldc 1, add, stind
        var box = new Counter();
        box.Value++;
        Debug.Assert(box.Value == 1);
        Console.WriteLine($"  Counter++ → {box.Value}");
        // discard: _ = Foo() may pop
        _ = Identity(99);
        Console.WriteLine("  Discarded call result ≈ pop; assignment keeps value via stloc/stfld.");
    }

    private static int Identity(int x) => x;

    private sealed class Counter
    {
        public int Value;
    }
}
