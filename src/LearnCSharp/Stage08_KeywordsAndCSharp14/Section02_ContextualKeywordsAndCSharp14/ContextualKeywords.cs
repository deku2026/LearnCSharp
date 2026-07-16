// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第2部分-上下文关键字与C#14专题.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section02_ContextualKeywordsAndCSharp14
// Item     : ContextualKeywords (上下文关键字全表 ~50 个)
// Topic id : stage08/section02/contextual_keywords
//
// 分族演示：var/dynamic、LINQ、访问器、模式逻辑、泛型约束、async/yield、record/required 等。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section02;

internal static class ContextualKeywords
{
    [LearnTopic("stage08/section02/contextual_keywords")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ContextualKeywords ===");
        DemoVarAndDynamic();
        DemoLinqQueryKeywords();
        DemoAccessorsAndRecord();
        DemoPatternLogical();
        DemoGenericConstraints();
        DemoAsyncAwaitYield();
        DemoNintGlobalNameofWith();
        return 0;
    }

    private static void DemoVarAndDynamic()
    {
        Console.WriteLine("-- var / dynamic --");
        var n = 42; // 编译期推断 int
        Debug.Assert(n.GetType() == typeof(int));
        dynamic d = "hello";
        Debug.Assert((int)d.Length == 5);
        d = 10;
        Debug.Assert((int)(d + 1) == 11);
        Console.WriteLine($"  var type=int, dynamic Length then +1 ok");
    }

    private static void DemoLinqQueryKeywords()
    {
        Console.WriteLine("-- LINQ: from where select group by into orderby join let --");
        int[] nums = [1, 2, 3, 4, 5, 6];
        var q =
            from x in nums
            where x % 2 == 0
            let doubled = x * 2
            orderby doubled descending
            select doubled;
        Debug.Assert(q.SequenceEqual([12, 8, 4]));

        var people = new[] { new { Id = 1, Name = "A" }, new { Id = 2, Name = "B" } };
        var orders = new[] { new { PersonId = 1, Total = 10 }, new { PersonId = 1, Total = 5 } };
        var joined =
            from p in people
            join o in orders on p.Id equals o.PersonId
            select p.Name + ":" + o.Total;
        Debug.Assert(joined.Count() == 2);

        var groups =
            from x in nums
            group x by x % 2 into g
            select (g.Key, Count: g.Count());
        Debug.Assert(groups.Any(g => g.Key == 0 && g.Count == 3));
        Console.WriteLine($"  even*2 desc=[{string.Join(',', q)}], join count={joined.Count()}");
    }

    private static void DemoAccessorsAndRecord()
    {
        Console.WriteLine("-- get/set/init/value/required/record/partial/file --");
        var p = new Person { Name = "Ada", Age = 36 };
        Debug.Assert(p.Name == "Ada" && p.Age == 36);
        // p.Name = "x"; // init-only after construction
        var r = new PointRec(1, 2);
        var r2 = r with { Y = 9 };
        Debug.Assert(r2 is { X: 1, Y: 9 });
        Debug.Assert(r == new PointRec(1, 2));
        var cfg = new Config { Title = "t" };
        Debug.Assert(cfg.Title == "t");
        Console.WriteLine($"  Person={p.Name}/{p.Age}, with Y={r2.Y}");
    }

    private static void DemoPatternLogical()
    {
        Console.WriteLine("-- and / or / not / when --");
        int n = 50;
        bool inRange = n is >= 0 and <= 100;
        bool smallOrBig = n is 1 or 2 or >= 40;
        bool notNull = "x" is not null;
        string grade = n switch
        {
            >= 90 => "A",
            >= 0 and <= 100 when n != 75 => "mid",
            _ => "other",
        };
        Debug.Assert(inRange && smallOrBig && notNull);
        Debug.Assert(grade == "mid");
        Console.WriteLine($"  patterns grade={grade}");
    }

    private static void DemoGenericConstraints()
    {
        Console.WriteLine("-- where 约束 / notnull / unmanaged --");
        Debug.Assert(CreateDefault<int>() == 0);
        Debug.Assert(IsNotNull("a"));
        Debug.Assert(SizeOfUnmanaged<int>() == sizeof(int));
        Console.WriteLine($"  CreateDefault<int>={CreateDefault<int>()}, unmanaged sizeof={SizeOfUnmanaged<int>()}");
    }

    private static void DemoAsyncAwaitYield()
    {
        Console.WriteLine("-- async / await / yield --");
        int v = GetValueAsync().GetAwaiter().GetResult();
        Debug.Assert(v == 7);
        var seq = TakeThree().ToArray();
        Debug.Assert(seq is [0, 1, 2]);
        Console.WriteLine($"  await result={v}, yield=[{string.Join(',', seq)}]");
    }

    private static void DemoNintGlobalNameofWith()
    {
        Console.WriteLine("-- nint / nuint / global / nameof / with / args --");
        nint ni = 8;
        nuint nu = 8;
        Debug.Assert(ni == 8 && nu == 8);
        string name = nameof(Person.Name);
        Debug.Assert(name == "Name");
        Type t = typeof(global::System.String);
        Debug.Assert(t == typeof(string));
        // args 是顶级语句上下文；此处用方法参数示意
        string[] sampleArgs = ["a", "b"];
        Debug.Assert(sampleArgs.Length == 2);
        Console.WriteLine($"  nint={ni}, nameof={name}, args.Length={sampleArgs.Length}");
    }

    private static T CreateDefault<T>() where T : new() => new();
    private static bool IsNotNull<T>(T value) where T : notnull => value is not null;
    private static int SizeOfUnmanaged<T>() where T : unmanaged
        => System.Runtime.CompilerServices.Unsafe.SizeOf<T>();

    private static async Task<int> GetValueAsync()
    {
        await Task.Yield();
        return 7;
    }

    private static IEnumerable<int> TakeThree()
    {
        yield return 0;
        yield return 1;
        yield return 2;
        yield break;
    }

    private sealed class Person
    {
        public required string Name { get; init; }
        public int Age
        {
            get;
            set => field = value < 0 ? throw new ArgumentOutOfRangeException(nameof(value)) : value;
        }
    }

    private record PointRec(int X, int Y);

    private sealed partial class Config
    {
        public partial string Title { get; set; }
    }

    private sealed partial class Config
    {
        private string _title = "";
        public partial string Title
        {
            get => _title;
            set => _title = value;
        }
    }
}
