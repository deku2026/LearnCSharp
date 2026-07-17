// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第3部分-LINQ全谱.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section03_LinqFullSpectrum
// Item     : QueryVsMethodSyntax
// Topic id : stage05/section03/query_vs_method_syntax
//
// 步骤 1：查询语法 vs 方法语法（编译器翻译为等价方法链）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section03;

internal static class QueryVsMethodSyntax
{
    [LearnTopic("stage05/section03/query_vs_method_syntax")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== QueryVsMethodSyntax ===");
        DemoEquivalent();
        DemoMethodOnlyOperators();
        DemoMixed();
        DemoJoinReadableAsQuery();
        return 0;
    }

    private static void DemoEquivalent()
    {
        Console.WriteLine("-- 查询语法 ≡ 方法语法 --");
        int[] nums = [5, 8, 1, 9, 3, 7];

        IEnumerable<int> q1 =
            from n in nums
            where n > 4
            orderby n
            select n * 10;

        IEnumerable<int> q2 = nums
            .Where(n => n > 4)
            .OrderBy(n => n)
            .Select(n => n * 10);

        List<int> a = q1.ToList();
        List<int> b = q2.ToList();
        Debug.Assert(a.SequenceEqual([50, 70, 80, 90]));
        Debug.Assert(a.SequenceEqual(b));
        Console.WriteLine($"  both → [{string.Join(", ", a)}]");
    }

    private static void DemoMethodOnlyOperators()
    {
        Console.WriteLine("-- 无查询关键字：Count / First / Take 只能方法语法 --");
        int[] nums = [5, 8, 1, 9, 3, 7];
        int count = nums.Count(n => n > 4);
        int first = nums.Where(n => n > 4).OrderBy(n => n).First();
        List<int> take2 = nums.OrderBy(n => n).Take(2).ToList();
        Debug.Assert(count == 4 && first == 5);
        Debug.Assert(take2.SequenceEqual([1, 3]));
        Console.WriteLine($"  Count(>4)={count}, First(sorted>4)={first}, Take2={string.Join(",", take2)}");
    }

    private static void DemoMixed()
    {
        Console.WriteLine("-- 混用：查询外包方法 --");
        int[] nums = [5, 8, 1, 9, 3, 7];
        bool any =
            (from n in nums
             where n > 8
             select n).Any();
        Debug.Assert(any);
        Console.WriteLine($"  (from…select…).Any() = {any}");
    }

    private static void DemoJoinReadableAsQuery()
    {
        Console.WriteLine("-- 复杂 join 时查询语法更易读 --");
        Order[] orders = [new(1, 100), new(2, 50), new(1, 25)];
        Customer[] customers = [new(1, "Ada"), new(2, "Bob")];

        var method = orders.Join(
            customers,
            o => o.CustomerId,
            c => c.Id,
            (o, c) => new { c.Name, o.Total });

        var query =
            from o in orders
            join c in customers on o.CustomerId equals c.Id
            select new { c.Name, o.Total };

        Debug.Assert(method.Count() == 3 && query.Count() == 3);
        Debug.Assert(query.Any(x => x.Name == "Ada" && x.Total == 100));
        Console.WriteLine($"  join rows={method.Count()}");
    }

    private sealed record Order(int CustomerId, int Total);
    private sealed record Customer(int Id, string Name);
}
