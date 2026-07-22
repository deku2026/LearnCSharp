// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第2部分-函数成员与构造.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section02_FunctionMembersAndConstruction
// Item     : NestedAndPartial
// Topic id : stage03/section02/nested_and_partial
//
// 步骤 6：嵌套类型访问外层私有；partial class/method/property。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section02;

internal static class NestedAndPartial
{
    [LearnTopic("stage03/section02/nested_and_partial")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== NestedAndPartial ===");
        DemoNestedAccessPrivate();
        DemoPartialClassMerge();
        DemoPartialMethodOptional();
        DemoPartialProperty();
        return 0;
    }

    private static void DemoNestedAccessPrivate()
    {
        Console.WriteLine("-- 嵌套类型可访问外层私有成员 --");
        Outer outer = new Outer(42);
        Outer.Nested nested = new Outer.Nested();
        Debug.Assert(nested.Read(outer) == 42);
        Console.WriteLine($"  Nested.Read(Outer)={nested.Read(outer)}");
    }

    private static void DemoPartialClassMerge()
    {
        Console.WriteLine("-- partial class 多部分合并 --");
        PartialPerson p = new PartialPerson { First = "Ada", Last = "Lovelace" };
        Debug.Assert(p.FullName() == "Ada Lovelace");
        Console.WriteLine($"  FullName={p.FullName()}");
    }

    private static void DemoPartialMethodOptional()
    {
        Console.WriteLine("-- 经典 partial void：无实现则调用被移除 --");
        HookHost h = new HookHost();
        h.Run(); // OnBefore 无实现 → 编译期移除调用
        Debug.Assert(h.Ran);
        Console.WriteLine("  HookHost.Run completed (optional partial void)");
    }

    private static void DemoPartialProperty()
    {
        Console.WriteLine("-- partial 属性(C#13) --");
        PartialConfig c = new PartialConfig { Title = "demo" };
        Debug.Assert(c.Title == "demo");
        Console.WriteLine($"  Title={c.Title}");
    }

    private sealed class Outer
    {
        private readonly int _secret;
        public Outer(int secret) => _secret = secret;

        public sealed class Nested
        {
            public int Read(Outer o) => o._secret;
        }
    }

    private sealed partial class PartialPerson
    {
        public string First { get; set; } = "";
        public string Last { get; set; } = "";
    }

    private sealed partial class PartialPerson
    {
        public string FullName() => $"{First} {Last}";
    }

    private sealed partial class HookHost
    {
        public bool Ran { get; private set; }
        partial void OnBefore();

        public void Run()
        {
            OnBefore();
            Ran = true;
        }
    }

    // 无 OnBefore 实现 → 调用被移除

    private sealed partial class PartialConfig
    {
        public partial string Title { get; set; }
    }

    private sealed partial class PartialConfig
    {
        private string _title = "";
        public partial string Title
        {
            get => _title;
            set => _title = value;
        }
    }
}
