// LearnCSharp example (filled)
// Doc      : CSharp-阶段1-语法基础与程序结构-详解.md
// Stage    : Stage01_SyntaxAndProgramStructure
// Section  : Section01_LanguageBasics
// Item     : Namespaces
// Topic id : stage01/section01/namespaces
//
// 步骤 4：块级 / 文件级 / 嵌套命名空间；全限定名消歧。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage01.Section01;

internal static class Namespaces
{
    [LearnTopic("stage01/section01/namespaces")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Namespaces ===");
        DemoFullyQualifiedNames();
        DemoSameNameDifferentNamespaces();
        DemoNestedNamespaceShape();
        DemoFileScopedRules();
        DemoCppContrast();
        return 0;
    }

    private static void DemoFullyQualifiedNames()
    {
        Console.WriteLine("-- 全限定名 --");
        // 不写 using 时用完整路径；分隔符是 . 不是 C++ 的 ::
        System.Console.WriteLine("  System.Console.WriteLine —— 不依赖 using");
        string full = typeof(System.Text.StringBuilder).FullName!;
        Console.WriteLine($"  typeof(StringBuilder).FullName = {full}");
        Debug.Assert(full == "System.Text.StringBuilder");
    }

    private static void DemoSameNameDifferentNamespaces()
    {
        Console.WriteLine("-- 同名类型 + 命名空间消歧 --");
        // 真实项目里是 App.Net.Logger vs App.Disk.Logger；此处用嵌套类型模拟，避免同文件混用命名空间写法
        AppNet.Logger net = new("net");
        AppDisk.Logger disk = new("disk");
        Console.WriteLine($"  {net.Describe()}");
        Console.WriteLine($"  {disk.Describe()}");

        Debug.Assert(net.Channel == "net");
        Debug.Assert(disk.Channel == "disk");
        Debug.Assert(net.GetType() != disk.GetType());
        Console.WriteLine($"  Type: {net.GetType().Name} vs {disk.GetType().Name}（容器不同 → 全名不同）");
        Console.WriteLine($"  FullName: {net.GetType().FullName}");
        Console.WriteLine($"  FullName: {disk.GetType().FullName}");
    }

    private static void DemoNestedNamespaceShape()
    {
        Console.WriteLine("-- 嵌套命名空间形态 --");
        // namespace Outer { namespace Inner { class C } } ≡ Outer.Inner.C
        Console.WriteLine("  namespace Outer { namespace Inner { class C } }");
        Console.WriteLine("  等价于 namespace Outer.Inner { class C }");
        Console.WriteLine("  完整名: Outer.Inner.C");

        OuterInner.C c = new();
        Console.WriteLine($"  模拟类型 Tag={c.Tag}, FullName={c.GetType().FullName}");
        Debug.Assert(c.Tag == "Outer.Inner.C");
    }

    private static void DemoFileScopedRules()
    {
        Console.WriteLine("-- 文件级 namespace 硬规则 --");
        string[] rules =
        [
            "CS8954: 一个文件最多一个文件级 namespace",
            "CS8955: 不能与块级 namespace 混用",
            "CS8956: 文件级 namespace 必须在所有成员之前",
            "本质: lower 成块级，花括号延伸到文件末尾",
            "现代默认: 文件级（IDE0161），少一层缩进",
        ];
        foreach (string r in rules)
            Console.WriteLine($"  · {r}");

        // 本文件本身使用文件级 namespace LearnCSharp.Stage01.Section01;
        Debug.Assert(typeof(Namespaces).Namespace == "LearnCSharp.Stage01.Section01");
        Console.WriteLine($"  本文件 Namespace = {typeof(Namespaces).Namespace}");
    }

    private static void DemoCppContrast()
    {
        Console.WriteLine("-- 🔶 C++ 对照 --");
        Console.WriteLine("  概念 ≈ namespace，但成员访问用 . 不是 ::");
        Console.WriteLine("  文件级 namespace Foo; 是 C# 特有简写");
        Console.WriteLine("  命名空间是逻辑分组；程序集是物理打包——两维度正交");
        Debug.Assert(typeof(Namespaces).Assembly is not null);
    }

    // 私有嵌套类型模拟不同命名空间下的同名 Logger（避免 CS8955）
    private static class AppNet
    {
        internal sealed class Logger
        {
            public Logger(string channel) => Channel = channel;
            public string Channel { get; }
            public string Describe() => $"[App.Net.Logger] channel={Channel}";
        }
    }

    private static class AppDisk
    {
        internal sealed class Logger
        {
            public Logger(string channel) => Channel = channel;
            public string Channel { get; }
            public string Describe() => $"[App.Disk.Logger] channel={Channel}";
        }
    }

    private static class OuterInner
    {
        internal sealed class C
        {
            public string Tag => "Outer.Inner.C";
        }
    }
}
