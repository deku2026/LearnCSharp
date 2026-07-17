// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第3部分-LINQ全谱.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section03_LinqFullSpectrum
// Item     : ExpressionTrees
// Topic id : stage05/section03/expression_trees
//
// 步骤 5：表达式树——代码即数据、Compile、手动构建。

using System.Diagnostics;
using System.Linq.Expressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section03;

internal static class ExpressionTrees
{
    [LearnTopic("stage05/section03/expression_trees")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExpressionTrees ===");
        DemoLambdaToTree();
        DemoInspectTree();
        DemoCompile();
        DemoManualBuild();
        DemoWhyEfNeedsTrees();
        return 0;
    }

    private static void DemoLambdaToTree()
    {
        Console.WriteLine("-- Func vs Expression&lt;Func&gt; --");
        Func<int, bool> compiled = x => x > 5;
        Expression<Func<int, bool>> tree = x => x > 5;
        Debug.Assert(compiled(6));
        Debug.Assert(tree.NodeType == ExpressionType.Lambda);
        Console.WriteLine($"  tree.Body = {tree.Body}");
    }

    private static void DemoInspectTree()
    {
        Console.WriteLine("-- 检视：Lambda → Binary GreaterThan → Parameter/Constant --");
        Expression<Func<int, bool>> tree = x => x > 5;
        Debug.Assert(tree.Body is BinaryExpression bin
            && bin.NodeType == ExpressionType.GreaterThan
            && bin.Left is ParameterExpression
            && bin.Right is ConstantExpression c
            && c.Value is 5);
        ParameterExpression p = tree.Parameters[0];
        Debug.Assert(p.Name == "x" && p.Type == typeof(int));
        Console.WriteLine($"  param={p.Name}, right={(tree.Body as BinaryExpression)!.Right}");
    }

    private static void DemoCompile()
    {
        Console.WriteLine("-- Expression.Compile → 可调用委托 --");
        Expression<Func<int, bool>> tree = x => x > 5;
        Func<int, bool> f = tree.Compile();
        Debug.Assert(f(6) && !f(3));
        Console.WriteLine($"  Compile()(6)={f(6)}, (3)={f(3)}");
    }

    private static void DemoManualBuild()
    {
        Console.WriteLine("-- 手动构建 Expression.GreaterThan --");
        ParameterExpression p = Expression.Parameter(typeof(int), "x");
        BinaryExpression body = Expression.GreaterThan(p, Expression.Constant(5));
        Expression<Func<int, bool>> tree = Expression.Lambda<Func<int, bool>>(body, p);
        Func<int, bool> f = tree.Compile();
        Debug.Assert(f(10) && !f(1));
        Console.WriteLine($"  manual tree {tree} → f(10)={f(10)}");
    }

    private static void DemoWhyEfNeedsTrees()
    {
        Console.WriteLine("-- 为何 EF 需要表达式树：把树译成 SQL，而非在内存调委托 --");
        Expression<Func<Person, bool>> pred = p => p.Age > 18;
        // 模拟 Provider：读树结构，不执行 C# 方法
        string fakeSql = TranslateAgeFilter(pred);
        Debug.Assert(fakeSql.Contains("Age") && fakeSql.Contains("18"));
        Console.WriteLine($"  toy translator → {fakeSql}");
        Console.WriteLine("  C++ 无运行时可检视 lambda 结构 → 无法做语言集成 SQL 翻译");
    }

    private static string TranslateAgeFilter(Expression<Func<Person, bool>> expr)
    {
        if (expr.Body is BinaryExpression { NodeType: ExpressionType.GreaterThan } bin
            && bin.Left is MemberExpression { Member.Name: "Age" }
            && bin.Right is ConstantExpression { Value: int age })
        {
            return $"SELECT * FROM People WHERE Age > {age}";
        }
        return "/* untranslatable */";
    }

    private sealed record Person(string Name, int Age);
}
