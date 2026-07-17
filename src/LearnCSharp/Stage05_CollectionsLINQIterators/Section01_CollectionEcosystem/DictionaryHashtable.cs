// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第1部分-集合体系.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section01_CollectionEcosystem
// Item     : DictionaryHashtable
// Topic id : stage05/section01/dictionary_hashtable
//
// 步骤 3：Dictionary + ⭐TryGetValue；自定义键 GetHashCode/Equals。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section01;

internal static class DictionaryHashtable
{
    [LearnTopic("stage05/section01/dictionary_hashtable")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DictionaryHashtable ===");
        DemoBasicAndTryGetValue();
        DemoWordCount();
        DemoIndexerThrows();
        DemoCustomKey();
        DemoTryAdd();
        return 0;
    }

    private static void DemoBasicAndTryGetValue()
    {
        Console.WriteLine("-- 索引器 vs TryGetValue（一次哈希） --");
        Dictionary<string, int> ages = new() { ["Ada"] = 36, ["Bob"] = 40 };
        ages["Cara"] = 25;
        Debug.Assert(ages["Ada"] == 36);
        Debug.Assert(!ages.TryGetValue("Dan", out _));
        Debug.Assert(ages.TryGetValue("Bob", out int bob) && bob == 40);
        Debug.Assert(ages.ContainsKey("Cara"));
        ages.Remove("Bob");
        Debug.Assert(!ages.ContainsKey("Bob"));
        Console.WriteLine($"  keys=[{string.Join(", ", ages.Keys)}]");
    }

    private static void DemoWordCount()
    {
        Console.WriteLine("-- 单词计数：TryGetValue 热路径 --");
        string[] words = ["a", "b", "a", "c", "b", "a"];
        Dictionary<string, int> counts = new();
        foreach (string w in words)
        {
            if (counts.TryGetValue(w, out int n))
                counts[w] = n + 1;
            else
                counts[w] = 1;
        }
        Debug.Assert(counts["a"] == 3 && counts["b"] == 2 && counts["c"] == 1);
        Console.WriteLine($"  a={counts["a"]}, b={counts["b"]}, c={counts["c"]}");
    }

    private static void DemoIndexerThrows()
    {
        Console.WriteLine("-- ⚠ dict[missing] → KeyNotFoundException --");
        Dictionary<string, int> d = new() { ["x"] = 1 };
        bool threw = false;
        try
        {
            _ = d["missing"];
        }
        catch (KeyNotFoundException)
        {
            threw = true;
        }
        Debug.Assert(threw);
        Console.WriteLine("  C# 抛异常；C++ map[missing] 会插默认值");
    }

    private static void DemoCustomKey()
    {
        Console.WriteLine("-- 自定义键须重写 GetHashCode + Equals --");
        Dictionary<PointKey, string> map = new()
        {
            [new PointKey(1, 2)] = "origin-ish",
        };
        bool found = map.TryGetValue(new PointKey(1, 2), out string? v);
        Debug.Assert(found && v == "origin-ish");
        Console.WriteLine($"  PointKey(1,2) → {v}");
    }

    private static void DemoTryAdd()
    {
        Console.WriteLine("-- TryAdd：不存在才加 --");
        Dictionary<string, int> d = new();
        Debug.Assert(d.TryAdd("Eve", 30));
        Debug.Assert(!d.TryAdd("Eve", 99));
        Debug.Assert(d["Eve"] == 30);
        Console.WriteLine($"  Eve stays {d["Eve"]}");
    }

    private readonly struct PointKey(int x, int y) : IEquatable<PointKey>
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public bool Equals(PointKey other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is PointKey p && Equals(p);
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
