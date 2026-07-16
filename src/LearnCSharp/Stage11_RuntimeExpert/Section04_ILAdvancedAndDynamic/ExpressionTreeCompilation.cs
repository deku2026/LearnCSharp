// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第4部分-IL高级与动态生成.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section04_ILAdvancedAndDynamic
// Item     : ExpressionTreeCompilation
// Topic id : stage11/section04/expression_tree_compilation
//
// Lesson: Expression trees compile to delegates (IL under the hood) without hand-written Emit.

using System.Diagnostics;
using System.Linq.Expressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section04;

internal static class ExpressionTreeCompilation
{
    [LearnTopic("stage11/section04/expression_tree_compilation")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExpressionTreeCompilation ===");
        DemoSimpleCompile();
        DemoClosureAndCapture();
        DemoVsDynamicMethod();
        return 0;
    }

    private static void DemoSimpleCompile()
    {
        Console.WriteLine("-- Expression.Compile → delegate --");
        ParameterExpression a = Expression.Parameter(typeof(int), "a");
        ParameterExpression b = Expression.Parameter(typeof(int), "b");
        Expression body = Expression.Add(a, b);
        Expression<Func<int, int, int>> tree = Expression.Lambda<Func<int, int, int>>(body, a, b);
        Func<int, int, int> fn = tree.Compile();
        int r = fn(10, 32);
        Debug.Assert(r == 42);
        Console.WriteLine($"  tree: {tree}");
        Console.WriteLine($"  Compile()(10,32)={r}");
    }

    private static void DemoClosureAndCapture()
    {
        Console.WriteLine("-- capturing constants into tree --");
        ConstantExpression factor = Expression.Constant(3);
        ParameterExpression x = Expression.Parameter(typeof(int), "x");
        Expression mul = Expression.Multiply(x, factor);
        Func<int, int> times3 = Expression.Lambda<Func<int, int>>(mul, x).Compile();
        Debug.Assert(times3(5) == 15);
        Console.WriteLine($"  times3(5)={times3(5)}");
    }

    private static void DemoVsDynamicMethod()
    {
        Console.WriteLine("-- Expression trees vs Reflection.Emit --");
        Console.WriteLine("  Expression: safer, composable, used by LINQ providers.");
        Console.WriteLine("  DynamicMethod: full IL control, harder, more power.");
        Console.WriteLine("  Both produce callable managed methods JIT can optimize.");
        Expression<Func<int, bool>> pred = n => n % 2 == 0;
        Func<int, bool> isEven = pred.Compile();
        Debug.Assert(isEven(4) && !isEven(5));
        Console.WriteLine($"  lambda expression tree '{pred}' → isEven(4)={isEven(4)}");
    }
}
