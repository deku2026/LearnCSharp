// LearnCSharp example (filled)
// Doc      : CSharp-阶段7-异步编程基础-第2部分-异步进阶与陷阱.md
// Stage    : Stage07_AsyncBasics
// Section  : Section02_AsyncAdvancedAndPitfalls
// Item     : ValueTask
// Topic id : stage07/section02/value_task
//
// 步骤 4：ValueTask 常同步完成减分配；限制：只 await 一次

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage07.Section02;

internal static class ValueTaskTopic
{
    private static readonly Dictionary<int, string> Cache = new()
    {
        [1] = "cached-one",
        [2] = "cached-two",
    };

    [LearnTopic("stage07/section02/value_task")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ValueTask ===");
        DemoCacheHitSyncPath().GetAwaiter().GetResult();
        DemoCacheMissAsyncPath().GetAwaiter().GetResult();
        DemoValueTaskRestrictions().GetAwaiter().GetResult();
        DemoPreferTaskByDefault();
        return 0;
    }

    private static async Task DemoCacheHitSyncPath()
    {
        Console.WriteLine("-- cache hit: ValueTask from result (sync complete) --");
        string a = await GetAsync(1);
        string b = await GetAsync(2);
        Debug.Assert(a == "cached-one" && b == "cached-two");
        Console.WriteLine($"  hits: {a}, {b}");
        Console.WriteLine("  ValueTask is a struct; sync path avoids allocating a Task object");
    }

    private static async Task DemoCacheMissAsyncPath()
    {
        Console.WriteLine("-- cache miss: wrap real Task inside ValueTask --");
        string loaded = await GetAsync(99);
        Debug.Assert(loaded == "loaded-99");
        // Second call should hit cache after LoadAsync stored it.
        string again = await GetAsync(99);
        Debug.Assert(again == "loaded-99");
        Console.WriteLine($"  miss then hit: {loaded}");
    }

    private static async Task DemoValueTaskRestrictions()
    {
        Console.WriteLine("-- restrictions: await once; AsTask for WhenAll --");
        ValueTask<string> vt = GetAsync(1);
        string once = await vt;
        Debug.Assert(once == "cached-one");

        // Cannot safely await the same ValueTask instance twice.
        // For composition APIs, convert:
        Task<string> t1 = GetAsync(1).AsTask();
        Task<string> t2 = GetAsync(2).AsTask();
        string[] all = await Task.WhenAll(t1, t2);
        Debug.Assert(all is ["cached-one", "cached-two"]);
        Console.WriteLine($"  AsTask + WhenAll: [{string.Join(", ", all)}]");
    }

    private static void DemoPreferTaskByDefault()
    {
        Console.WriteLine("-- default to Task; ValueTask only after profiling --");
        Console.WriteLine("  use when: often sync-complete + hot path allocation pressure");
        Console.WriteLine("  avoid when: multiple await, WhenAll without AsTask, concurrent consumers");
        Debug.Assert(typeof(ValueTask).IsValueType);
    }

    private static ValueTask<string> GetAsync(int id)
    {
        if (Cache.TryGetValue(id, out string? cached))
            return new ValueTask<string>(cached);

        return new ValueTask<string>(LoadAsync(id));
    }

    private static async Task<string> LoadAsync(int id)
    {
        await Task.Delay(5);
        string value = $"loaded-{id}";
        Cache[id] = value;
        return value;
    }
}
