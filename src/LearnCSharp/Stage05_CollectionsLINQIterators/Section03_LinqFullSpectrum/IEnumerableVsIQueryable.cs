// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第3部分-LINQ全谱.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section03_LinqFullSpectrum
// Item     : IEnumerableVsIQueryable
// Topic id : stage05/section03/ienumerable_vs_iqueryable
//
// 步骤 4：IEnumerable（内存委托）vs IQueryable（表达式树翻译）。

using System.Diagnostics;
using System.Linq.Expressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section03;

internal static class IEnumerableVsIQueryable
{
    [LearnTopic("stage05/section03/ienumerable_vs_iqueryable")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IEnumerableVsIQueryable ===");
        DemoEnumerableInMemory();
        DemoQueryableExpressionShape();
        DemoAsEnumerableSwitchesToMemory();
        DemoFuncVsExpression();
        return 0;
    }

    private static void DemoEnumerableInMemory()
    {
        Console.WriteLine("-- IEnumerable.Where：Func 委托，内存过滤 --");
        List<Person> people =
        [
            new(1, "Ada", 36),
            new(2, "Bob", 17),
            new(3, "Cara", 40),
        ];
        IEnumerable<Person> memory = people;
        List<Person> adults = memory.Where(p => p.Age > 18).ToList();
        Debug.Assert(adults.Count == 2 && adults.All(p => p.Age > 18));
        Console.WriteLine($"  memory adults={adults.Count}");
    }

    private static void DemoQueryableExpressionShape()
    {
        Console.WriteLine("-- IQueryable.Where：Expression，Provider 可翻译 --");
        List<Person> people =
        [
            new(1, "Ada", 36),
            new(2, "Bob", 17),
            new(3, "Cara", 40),
        ];
        // AsQueryable 仍在内存用 EnumerableQuery，但运算符签名走 Expression
        IQueryable<Person> q = people.AsQueryable();
        IQueryable<Person> filtered = q.Where(p => p.Age > 18);
        Debug.Assert(filtered.Provider is not null);
        Debug.Assert(filtered.Expression is not null);
        List<Person> got = filtered.ToList();
        Debug.Assert(got.Count == 2);
        Console.WriteLine($"  IQueryable expression node={filtered.Expression.NodeType}, count={got.Count}");
        Console.WriteLine("  EF Core 会把同类 Expression 译成 SQL WHERE（此处为内存 Provider）");
    }

    private static void DemoAsEnumerableSwitchesToMemory()
    {
        Console.WriteLine("-- ⚠ AsEnumerable 后后续在内存执行 --");
        List<Person> people =
        [
            new(1, "Ada", 36),
            new(2, "Bob", 17),
        ];
        IQueryable<Person> q = people.AsQueryable();
        // 模拟：过早 AsEnumerable 后，Where 用 Enumerable（Func），不再可翻译
        IEnumerable<Person> inMem = q.AsEnumerable().Where(p => p.Age > 18);
        Debug.Assert(inMem.Count() == 1);
        Console.WriteLine("  AsEnumerable().Where 已离开 IQueryable 管道（EF 场景可能拉全表）");
    }

    private static void DemoFuncVsExpression()
    {
        Console.WriteLine("-- 同一 lambda：Func 可调用；Expression 可检视 --");
        Func<int, bool> fn = x => x > 5;
        Expression<Func<int, bool>> tree = x => x > 5;
        Debug.Assert(fn(6));
        Debug.Assert(tree.Compile()(6));
        Debug.Assert(tree.Body.NodeType == ExpressionType.GreaterThan);
        Console.WriteLine($"  Func(6)={fn(6)}; Expression.Body={tree.Body}");
    }

    private sealed record Person(int Id, string Name, int Age);
}
