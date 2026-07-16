// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第4部分-测试.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section04_Testing
// Item     : WhyTestAndTestProject
// Topic id : stage10/section04/why_test_and_test_project
//
// 为何自动化测试 + 测试项目结构。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section04;

internal static class WhyTestAndTestProject
{
    [LearnTopic("stage10/section04/why_test_and_test_project")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WhyTestAndTestProject ===");
        DemoWhyAutomate();
        DemoSolutionLayout();
        DemoTestProjectCsproj();
        DemoWhatBelongsWhere();
        DemoSafetyNetDemo();
        return 0;
    }

    private static void DemoWhyAutomate()
    {
        Console.WriteLine("-- why automated tests --");
        string[] reasons =
        [
            "回归防护：改 A 不悄悄弄坏 B",
            "设计反馈：难测的代码往往耦合过重",
            "文档：可执行的用法示例",
            "重构勇气：绿测支撑大改",
            "CI 门禁：合并前客观信号",
        ];
        foreach (string r in reasons)
            Console.WriteLine($"  • {r}");
        Debug.Assert(reasons.Length >= 4);
    }

    private static void DemoSolutionLayout()
    {
        Console.WriteLine("-- typical layout --");
        string[] tree =
        [
            "MyApp.sln",
            "  src/MyApp/MyApp.csproj",
            "  src/MyApp.Core/MyApp.Core.csproj",
            "  tests/MyApp.Tests/MyApp.Tests.csproj  → ProjectReference → Core/App",
        ];
        foreach (string line in tree)
            Console.WriteLine($"  {line}");
        Debug.Assert(tree.Any(t => t.Contains("Tests", StringComparison.Ordinal)));
        Console.WriteLine("  测试程序集通常不 pack、不发布给用户");
    }

    private static void DemoTestProjectCsproj()
    {
        Console.WriteLine("-- test project essentials --");
        string[] lines =
        [
            """<Project Sdk="Microsoft.NET.Sdk">""",
            "  <PropertyGroup>",
            "    <TargetFramework>net10.0</TargetFramework>",
            "    <IsPackable>false</IsPackable>",
            "    <IsTestProject>true</IsTestProject>",
            "  </PropertyGroup>",
            "  <ItemGroup>",
            "    <PackageReference Include=\"xunit\" />",
            "    <PackageReference Include=\"xunit.runner.visualstudio\" />",
            "    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" />",
            "    <ProjectReference Include=\"..\\..\\src\\MyApp.Core\\MyApp.Core.csproj\" />",
            "  </ItemGroup>",
            "</Project>",
        ];
        foreach (string line in lines)
            Console.WriteLine($"  {line}");
        Debug.Assert(lines.Any(l => l.Contains("IsTestProject", StringComparison.Ordinal)));
    }

    private static void DemoWhatBelongsWhere()
    {
        Console.WriteLine("-- what belongs where --");
        (string Layer, string Content)[] map =
        [
            ("生产项目", "业务 API、无测试框架引用（理想）"),
            ("单元测试项目", "纯逻辑、假依赖、快"),
            ("集成测试项目", "真实 IO/容器/HTTP，可标签过滤"),
        ];
        foreach (var (layer, content) in map)
            Console.WriteLine($"  {layer,-12} {content}");
        Debug.Assert(map.Length == 3);
    }

    private static void DemoSafetyNetDemo()
    {
        Console.WriteLine("-- safety net micro-demo --");
        int before = Calculator.Add(2, 3);
        Debug.Assert(before == 5);
        // “重构”实现仍保持行为
        int after = Calculator.AddRefactored(2, 3);
        Debug.Assert(after == before);
        Console.WriteLine($"  Add(2,3)={before}; after refactor still {after}");
        Console.WriteLine("  没有断言，重构只能靠运气");
    }

    private static class Calculator
    {
        public static int Add(int a, int b) => a + b;
        public static int AddRefactored(int a, int b) => a - -b;
    }
}
