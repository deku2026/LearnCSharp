// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第5部分-发布裁剪与AOT.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section05_PublishTrimmingAndAOT
// Item     : Trimming
// Topic id : stage10/section05/trimming
//
// IL 裁剪：静态可达性分析删除未用代码；反射是主要风险。

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section05;

internal static class Trimming
{
    [LearnTopic("stage10/section05/trimming")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Trimming ===");
        DemoWhatIsTrim();
        DemoEnable();
        DemoWhyReflectionBreaks();
        DemoRootingIdea();
        DemoWarnings();
        return 0;
    }

    private static void DemoWhatIsTrim()
    {
        Console.WriteLine("-- what trimming does --");
        Console.WriteLine("  从应用 + 框架程序集中删除静态分析认为不可达的 IL");
        Console.WriteLine("  目标: 更小 self-contained / 更快加载");
        Console.WriteLine("  分析是静态的：看不到字符串反射、插件扫描等动态根");
        Debug.Assert(true);
    }

    private static void DemoEnable()
    {
        Console.WriteLine("-- enable --");
        Console.WriteLine("  <PublishTrimmed>true</PublishTrimmed>");
        Console.WriteLine("  <TrimMode>link</TrimMode>  // 或 partial 等策略（随 SDK 演进）");
        Console.WriteLine("  $ dotnet publish -c Release -r linux-x64 -p:PublishTrimmed=true");
        string[] props = ["PublishTrimmed", "TrimMode", "TrimmerDefaultAction"];
        Debug.Assert(props[0] == "PublishTrimmed");
    }

    private static void DemoWhyReflectionBreaks()
    {
        Console.WriteLine("-- why reflection is dangerous under trim --");
        // 静态可见调用：裁剪器能看到
        string direct = new UsedService().Name;
        Debug.Assert(direct == "used");

        // 仅通过字符串拿到类型：静态图可能认为 UnusedService 不可达 → 被裁掉
        string typeName = typeof(UsedService).FullName!.Replace("UsedService", "MaybeMissingService", StringComparison.Ordinal);
        Type? dynamicType = Type.GetType(typeName);
        Console.WriteLine($"  Type.GetType(\"{typeName}\") => {dynamicType?.Name ?? "null"}");
        Console.WriteLine("  若类型只被字符串引用，publish trim 后运行时可能 TypeLoad/Null");
        Debug.Assert(direct.Length > 0);
    }

    private static void DemoRootingIdea()
    {
        Console.WriteLine("-- rooting (keep) idea --");
        Console.WriteLine("  DynamicDependency / DynamicDependencyAttribute");
        Console.WriteLine("  DynamicallyAccessedMembers on Type/string parameters");
        Console.WriteLine("  RD.xml / ILLink descriptors（遗留/高级）");
        Console.WriteLine("  源生成把“动态”变成编译期静态引用 → 对裁剪友好");
        string[] tools =
        [
            "DynamicallyAccessedMembers",
            "DynamicDependency",
            "RequiresUnreferencedCode",
            "source generators",
        ];
        foreach (string t in tools)
            Console.WriteLine($"  • {t}");
        Debug.Assert(tools.Length == 4);
    }

    private static void DemoWarnings()
    {
        Console.WriteLine("-- trim warnings --");
        Console.WriteLine("  构建会报告 ILLink/IL2xxx 类警告：潜在被裁成员");
        Console.WriteLine("  把警告当错误: <SuppressTrimAnalysisWarnings>false</…> + warnaserror 策略");
        Console.WriteLine("  先修警告再发布；生产 trim 应用必须测 publish 产物");
        Assembly asm = typeof(Trimming).Assembly;
        Console.WriteLine($"  当前程序集(未裁剪开发构建): {asm.GetName().Name}");
        Debug.Assert(asm.GetTypes().Any(t => t.Name.Contains("Trimming", StringComparison.Ordinal)));
    }

    private sealed class UsedService
    {
        public string Name => "used";
    }

    // 可能仅被动态字符串引用的类型（示意）
    private sealed class MaybeMissingService
    {
        public string Name => "maybe";
    }
}
