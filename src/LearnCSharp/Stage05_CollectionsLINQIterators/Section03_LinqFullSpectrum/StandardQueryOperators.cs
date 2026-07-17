// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第3部分-LINQ全谱.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section03_LinqFullSpectrum
// Item     : StandardQueryOperators
// Topic id : stage05/section03/standard_query_operators
//
// 步骤 3：标准查询运算符全谱（过滤/投影/排序/分组/连接/聚合…）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section03;

internal static class StandardQueryOperators
{
    [LearnTopic("stage05/section03/standard_query_operators")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StandardQueryOperators ===");
        DemoFilterProjectSort();
        DemoGroupJoin();
        DemoAggregateQuantifiersElements();
        DemoPartitionSetConvertGenerate();
        return 0;
    }

    private static void DemoFilterProjectSort()
    {
        Console.WriteLine("-- Where / Select / SelectMany / OrderBy / ThenBy --");
        Person[] people =
        [
            new("Ada", 36, "Eng", ["cat"]),
            new("Bob", 17, "Sales", ["dog", "fish"]),
            new("Cara", 40, "Eng", []),
        ];

        List<string> adults = people.Where(p => p.Age >= 18).Select(p => p.Name).ToList();
        Debug.Assert(adults.SequenceEqual(["Ada", "Cara"]));

        List<string> pets = people.SelectMany(p => p.Pets).ToList();
        Debug.Assert(pets.SequenceEqual(["cat", "dog", "fish"]));

        List<string> sorted = people
            .OrderBy(p => p.Age)
            .ThenByDescending(p => p.Name)
            .Select(p => p.Name)
            .ToList();
        Debug.Assert(sorted[0] == "Bob" && sorted[^1] == "Cara");

        object[] mixed = [1, "hi", 2.5, "yo"];
        List<string> strings = mixed.OfType<string>().ToList();
        Debug.Assert(strings.SequenceEqual(["hi", "yo"]));
        Console.WriteLine($"  adults={string.Join(",", adults)}; pets={string.Join(",", pets)}");
    }

    private static void DemoGroupJoin()
    {
        Console.WriteLine("-- GroupBy / Join / GroupJoin --");
        Person[] people =
        [
            new("Ada", 36, "Eng", []),
            new("Bob", 17, "Sales", []),
            new("Cara", 40, "Eng", []),
        ];
        List<IGrouping<string, Person>> byDept = people.GroupBy(p => p.Department).ToList();
        Debug.Assert(byDept.Count == 2);
        Debug.Assert(byDept.Single(g => g.Key == "Eng").Count() == 2);

        Order[] orders = [new(1, 100), new(2, 50), new(1, 25)];
        Customer[] customers = [new(1, "Ada"), new(2, "Bob")];
        var joined = orders.Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new { c.Name, o.Total }).ToList();
        Debug.Assert(joined.Count == 3);

        var grouped = customers.GroupJoin(
            orders,
            c => c.Id,
            o => o.CustomerId,
            (c, os) => new { c.Name, Count = os.Count() }).ToList();
        Debug.Assert(grouped.Single(x => x.Name == "Ada").Count == 2);
        Console.WriteLine($"  groups={byDept.Count}, join={joined.Count}, Ada orders={grouped.Single(x => x.Name == "Ada").Count}");
    }

    private static void DemoAggregateQuantifiersElements()
    {
        Console.WriteLine("-- Sum/Max/Aggregate/Any/First/Single --");
        int[] nums = [1, 2, 3, 4, 5];
        Debug.Assert(nums.Sum() == 15 && nums.Average() == 3 && nums.Max() == 5 && nums.Min() == 1);
        Debug.Assert(nums.Count(n => n > 3) == 2);
        int product = nums.Aggregate(1, (acc, x) => acc * x);
        Debug.Assert(product == 120);
        Debug.Assert(nums.Any(n => n == 3) && nums.All(n => n > 0) && nums.Contains(4));
        Debug.Assert(nums.First(n => n > 3) == 4);
        Debug.Assert(nums.FirstOrDefault(n => n > 99) == 0);
        Debug.Assert(nums.Single(n => n == 3) == 3);
        Debug.Assert(nums.ElementAt(2) == 3);

        Person[] people = [new("Ada", 36, "Eng", []), new("Bob", 17, "Sales", [])];
        Person oldest = people.MaxBy(p => p.Age)!;
        Debug.Assert(oldest.Name == "Ada");
        Console.WriteLine($"  product={product}, oldest={oldest.Name}");
    }

    private static void DemoPartitionSetConvertGenerate()
    {
        Console.WriteLine("-- Skip/Take/Chunk/Distinct/Union/ToDictionary/Range --");
        int[] nums = [1, 2, 2, 3, 4, 5, 6];
        List<int> page = nums.Skip(2).Take(3).ToList();
        Debug.Assert(page.SequenceEqual([2, 3, 4]));
        int[][] chunks = nums.Chunk(3).ToArray();
        Debug.Assert(chunks.Length == 3 && chunks[0].SequenceEqual([1, 2, 2]));

        Debug.Assert(nums.Distinct().SequenceEqual([1, 2, 3, 4, 5, 6]));
        int[] a = [1, 2, 3], b = [3, 4];
        Debug.Assert(a.Union(b).SequenceEqual([1, 2, 3, 4]));
        Debug.Assert(a.Intersect(b).SequenceEqual([3]));
        Debug.Assert(a.Except(b).SequenceEqual([1, 2]));

        Person[] people = [new("Ada", 36, "Eng", []), new("Bob", 17, "Sales", [])];
        Dictionary<string, int> ages = people.ToDictionary(p => p.Name, p => p.Age);
        Debug.Assert(ages["Ada"] == 36);
        ILookup<string, Person> lookup = people.ToLookup(p => p.Department);
        Debug.Assert(lookup["Eng"].Count() == 1);

        List<int> range = Enumerable.Range(1, 5).ToList();
        Debug.Assert(range.SequenceEqual([1, 2, 3, 4, 5]));
        Debug.Assert(Enumerable.Repeat("x", 3).SequenceEqual(["x", "x", "x"]));
        Debug.Assert(!Enumerable.Empty<int>().Any());
        Console.WriteLine($"  page=[{string.Join(",", page)}]; Range1..5 ok");
    }

    private sealed record Person(string Name, int Age, string Department, string[] Pets);
    private sealed record Order(int CustomerId, int Total);
    private sealed record Customer(int Id, string Name);
}
