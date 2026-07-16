// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第2部分-上下文关键字与C#14专题.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section02_ContextualKeywordsAndCSharp14
// Item     : CSharpVersionHistory (C# 1.0 → 14.0 演进全表)
// Topic id : stage08/section02/csharp_version_history
//
// 用可运行微示例锚定关键版本特性（非穷举实现，而是版本地图 + 抽样验证）。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section02;

internal static class CSharpVersionHistory
{
    [LearnTopic("stage08/section02/csharp_version_history")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CSharpVersionHistory ===");
        DemoEarlyFoundations();
        DemoLinqAndAsyncEra();
        DemoModernPatternRecord();
        DemoRecentPerformanceExpressiveness();
        PrintVersionMap();
        return 0;
    }

    private static void DemoEarlyFoundations()
    {
        Console.WriteLine("-- C# 1–2 地基：class/delegate/event/generic/yield --");
        Func<int, int> square = static x => x * x; // 现代写法；委托思想自 C#1
        Debug.Assert(square(3) == 9);
        int? maybe = null;
        Debug.Assert(maybe is null); // 可空值类型 C#2
        var gen = new Box<int>(5);
        Debug.Assert(gen.Value == 5);
        Debug.Assert(YieldTwo().SequenceEqual([1, 2]));
        Console.WriteLine("  generics + nullable + yield ok");
    }

    private static void DemoLinqAndAsyncEra()
    {
        Console.WriteLine("-- C# 3 LINQ/var/lambda；C# 5 async/await --");
        var data = new[] { 1, 2, 3, 4 };
        var q = data.Where(x => x % 2 == 0).Select(x => x * 10).ToArray();
        Debug.Assert(q is [20, 40]);
        int v = GetAsync().GetAwaiter().GetResult();
        Debug.Assert(v == 1);
        Console.WriteLine($"  LINQ={string.Join(',', q)}, await={v}");
    }

    private static void DemoModernPatternRecord()
    {
        Console.WriteLine("-- C# 7–9：元组/模式/record/init/switch 表达式 --");
        (int x, int y) t = (1, 2);
        Debug.Assert(t is (1, 2));
        string kind = 5 switch
        {
            < 0 => "neg",
            0 => "zero",
            > 0 and < 10 => "small",
            _ => "big",
        };
        Debug.Assert(kind == "small");
        var r = new PersonRec("Ada") { Age = 36 };
        var r2 = r with { Age = 37 };
        Debug.Assert(r2 is { Name: "Ada", Age: 37 });
        Console.WriteLine($"  pattern={kind}, record with Age={r2.Age}");
    }

    private static void DemoRecentPerformanceExpressiveness()
    {
        Console.WriteLine("-- C# 11–14：列表模式/集合表达式/field/extension 抽样 --");
        int[] arr = [1, 2, 3];
        Debug.Assert(arr is [1, 2, 3]);
        Debug.Assert(arr is [1, .., 3]);
        var p = new FieldSample { Name = "x" };
        Debug.Assert(p.Name == "x");
        Debug.Assert("a b".HistoryWordCount == 2);
        string unbound = nameof(List<>);
        Debug.Assert(unbound == "List");
        Console.WriteLine($"  list pattern ok, field Name={p.Name}, nameof(List<>)={unbound}");
    }

    private static void PrintVersionMap()
    {
        Console.WriteLine("-- 版本关键特性地图（摘要） --");
        (string Ver, string Highlights)[] map =
        [
            ("1.0", "class/struct/interface/delegate/event/property"),
            ("2.0", "generics, nullable value types, yield, partial"),
            ("3.0", "LINQ, lambda, var, extension methods, auto-props"),
            ("4.0", "dynamic, named/optional args, variance"),
            ("5.0", "async/await"),
            ("6.0", "nameof, ?., string interpolation, exception filters"),
            ("7.x", "tuples, pattern matching, local functions, ref returns"),
            ("8.0", "nullable refs, switch expr, default interface members, ranges"),
            ("9.0", "record, init, top-level, and/or/not, nint"),
            ("10.0", "global using, file-scoped namespace, record struct"),
            ("11.0", "raw strings, generic math, list patterns, required, file types"),
            ("12.0", "primary constructors, collection expressions"),
            ("13.0", "params collections, Lock, allows ref struct, partial props"),
            ("14.0", "field, extension members, compound assignment, null-cond assign"),
        ];
        foreach (var (ver, h) in map)
            Console.WriteLine($"  C# {ver}: {h}");
        Debug.Assert(map.Length >= 14);
    }

    private static IEnumerable<int> YieldTwo()
    {
        yield return 1;
        yield return 2;
    }

    private static async Task<int> GetAsync()
    {
        await Task.CompletedTask;
        return 1;
    }

    private sealed class Box<T>(T value)
    {
        public T Value { get; } = value;
    }

    private record PersonRec(string Name)
    {
        public int Age { get; init; }
    }

    private sealed class FieldSample
    {
        public string Name
        {
            get;
            set => field = value ?? "";
        } = "";
    }
}

file static class Stage08VersionHistoryExtensions
{
    extension(string s)
    {
        public int HistoryWordCount =>
            s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
