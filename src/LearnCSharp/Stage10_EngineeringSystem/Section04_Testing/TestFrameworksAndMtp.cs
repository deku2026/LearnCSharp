// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第4部分-测试.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section04_Testing
// Item     : TestFrameworksAndMtp
// Topic id : stage10/section04/test_frameworks_and_mtp
//
// xUnit/NUnit/MSTest + VSTest vs Microsoft.Testing.Platform；.NET 10 默认 MTP。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section04;

internal static class TestFrameworksAndMtp
{
    [LearnTopic("stage10/section04/test_frameworks_and_mtp")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TestFrameworksAndMtp ===");
        DemoFrameworkApis();
        DemoVsTestVsMtp();
        DemoNet10Default();
        DemoChoosingStack();
        DemoDiscoveryMentalModel();
        return 0;
    }

    private static void DemoFrameworkApis()
    {
        Console.WriteLine("-- test framework APIs (how you write tests) --");
        (string Fw, string Attr, string Assert)[] fws =
        [
            ("xUnit", "[Fact]/[Theory]", "Assert.Equal"),
            ("NUnit", "[Test]/[TestCase]", "Assert.That"),
            ("MSTest", "[TestMethod]", "Assert.AreEqual"),
        ];
        foreach (var (fw, attr, assert) in fws)
            Console.WriteLine($"  {fw,-8} {attr,-24} {assert}");
        Debug.Assert(fws.Length == 3);
        Console.WriteLine("  框架 ≠ 运行平台：框架提供属性/断言；平台负责发现与执行");
    }

    private static void DemoVsTestVsMtp()
    {
        Console.WriteLine("-- VSTest vs Microsoft.Testing.Platform (MTP) --");
        Console.WriteLine("  VSTest: 经典适配器模型（.NET 多年默认）");
        Console.WriteLine("  MTP: 更现代的测试平台，扩展点清晰、可独立 exe 宿主");
        Console.WriteLine("  迁移: 测试代码属性大多不变；改的是 runner/包/过滤语法细节");
        (string Aspect, string VSTest, string Mtp)[] cmp =
        [
            ("宿主", "vstest.console", "Microsoft.Testing.Platform"),
            ("扩展", "Test Adapter", "MTP extensions"),
            ("过滤", "--filter 表达式", "框架相关；xUnit v3 语法有演进"),
        ];
        foreach (var (aspect, vst, mtp) in cmp)
            Console.WriteLine($"  {aspect,-6} | VSTest: {vst,-18} | MTP: {mtp}");
        Debug.Assert(cmp.Length == 3);
    }

    private static void DemoNet10Default()
    {
        Console.WriteLine("-- .NET 10: dotnet test leans MTP --");
        Console.WriteLine("  新模板/默认路径走向 MTP（以当前 SDK 文档为准）");
        Console.WriteLine("  旧项目可继续 VSTest；混合仓库注意 runner 配置");
        Console.WriteLine("  命令仍常是: dotnet test（底层平台可切换）");
        string cmd = "dotnet test";
        Debug.Assert(cmd.StartsWith("dotnet ", StringComparison.Ordinal));
    }

    private static void DemoChoosingStack()
    {
        Console.WriteLine("-- choosing a stack --");
        string[] tips =
        [
            "新项目: xUnit 或 MSTest + 官方模板即可",
            "团队已有 NUnit → 保持，不必为换而换",
            "关注: 并行默认、Theory 数据、断言可读性",
            "平台: 跟 SDK 默认，除非有适配器依赖锁死 VSTest",
        ];
        foreach (string t in tips)
            Console.WriteLine($"  • {t}");
        Debug.Assert(tips.Length == 4);
    }

    private static void DemoDiscoveryMentalModel()
    {
        Console.WriteLine("-- discovery mental model --");
        // 不用 xunit 包，用自研“发现”演示
        var catalog = new List<string>();
        foreach (var m in typeof(SampleTests).GetMethods())
        {
            if (m.GetCustomAttributes(typeof(MiniFactAttribute), false).Length > 0)
                catalog.Add(m.Name);
        }
        Debug.Assert(catalog.Contains(nameof(SampleTests.Add_works)));
        Console.WriteLine($"  discovered mini-facts: {string.Join(", ", catalog)}");
        SampleTests.Add_works();
        Console.WriteLine("  真框架: 反射/源生成发现 + runner 执行 + 结果报告");
    }

    [AttributeUsage(AttributeTargets.Method)]
    private sealed class MiniFactAttribute : Attribute;

    private static class SampleTests
    {
        [MiniFact]
        public static void Add_works()
        {
            Debug.Assert(2 + 2 == 4);
        }

        public static void NotATest() { }
    }
}
