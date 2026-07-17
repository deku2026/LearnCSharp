// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第1部分-数据成员与封装.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section01_DataMembersAndEncapsulation
// Item     : ConstVsReadonly
// Topic id : stage03/section01/const_vs_readonly
//
// 步骤 4：const 编译期内联 vs readonly 运行期；引用类型浅 const。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section01;

internal static class ConstVsReadonly
{
    [LearnTopic("stage03/section01/const_vs_readonly")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ConstVsReadonly ===");
        DemoConstCompileTime();
        DemoStaticReadonlyRuntime();
        DemoInstanceReadonly();
        DemoReadonlyReferenceShallow();
        DemoVersioningHint();
        return 0;
    }

    private static void DemoConstCompileTime()
    {
        Console.WriteLine("-- const：编译期常量、隐式 static --");
        int max = Config.MaxItems; // 使用处会被内联为字面量
        Debug.Assert(max == 100);
        Debug.Assert(Config.AppName == "LearnCSharp");
        Console.WriteLine($"  MaxItems={max}, AppName={Config.AppName}");
    }

    private static void DemoStaticReadonlyRuntime()
    {
        Console.WriteLine("-- static readonly：运行期初始化、任意类型 --");
        Debug.Assert(Config.CacheSize == 256);
        Debug.Assert(Config.DefaultEndpoint is not null);
        Console.WriteLine($"  CacheSize={Config.CacheSize}, Endpoint={Config.DefaultEndpoint}");
    }

    private static void DemoInstanceReadonly()
    {
        Console.WriteLine("-- 实例 readonly：每对象构造时定值 --");
        Stamp a = new Stamp();
        Stamp b = new Stamp();
        Debug.Assert(a.Created <= b.Created);
        Console.WriteLine($"  a.Created={a.Created:O}");
        Console.WriteLine($"  b.Created={b.Created:O}");
    }

    private static void DemoReadonlyReferenceShallow()
    {
        Console.WriteLine("-- readonly 引用类型：引用不可改，对象可变(非深 const) --");
        Bag bag = new Bag();
        bag.Items.Add(1);
        bag.Items.Add(2);
        Debug.Assert(bag.Items.Count == 2);
        // bag.Items = new List<int>(); // ❌ 不能换引用
        Console.WriteLine($"  Items.Count={bag.Items.Count} (Add OK, reassign NO)");
    }

    private static void DemoVersioningHint()
    {
        Console.WriteLine("-- 版本化：跨程序集可变常量优先 static readonly --");
        // const 会内联到消费方；改库后必须重编消费方
        // static readonly 运行期读字段，只重编库即可
        int inlined = Config.MaxItems;
        int runtime = Config.CacheSize;
        Debug.Assert(inlined == 100 && runtime == 256);
        Console.WriteLine("  const=inline; static readonly=field load");
    }

    private static class Config
    {
        public const int MaxItems = 100;
        public const string AppName = "LearnCSharp";
        public static readonly int CacheSize = ComputeSize();
        public static readonly Uri DefaultEndpoint = new("https://example.local/");
        private static int ComputeSize() => 256;
    }

    private sealed class Stamp
    {
        public readonly DateTime Created;
        public Stamp() => Created = DateTime.UtcNow;
    }

    private sealed class Bag
    {
        public readonly List<int> Items = new();
    }
}
