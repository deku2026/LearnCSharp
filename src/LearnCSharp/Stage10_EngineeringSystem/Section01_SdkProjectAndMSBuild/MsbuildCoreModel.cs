// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第1部分-SDK项目与MSBuild构建.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section01_SdkProjectAndMSBuild
// Item     : MsbuildCoreModel
// Topic id : stage10/section01/msbuild_core_model
//
// MSBuild 四件套：Property / Item / Target / Task + 两阶段求值。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section01;

internal static class MsbuildCoreModel
{
    [LearnTopic("stage10/section01/msbuild_core_model")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MsbuildCoreModel ===");
        DemoFourConcepts();
        DemoPropertyEvaluationOrder();
        DemoItemMetadata();
        DemoTargetTaskGraph();
        DemoTwoPhaseEvaluation();
        return 0;
    }

    private static void DemoFourConcepts()
    {
        Console.WriteLine("-- four MSBuild concepts --");
        (string Name, string Role, string Syntax)[] concepts =
        [
            ("Property", "标量字符串键值", "$(Name)"),
            ("Item", "列表 + 元数据", "@(Name) / %(Meta)"),
            ("Target", "有序工作单元 + 条件", "BeforeTargets/AfterTargets"),
            ("Task", "可执行原子操作（.NET 类）", "Csc / Copy / Message"),
        ];
        foreach (var (name, role, syntax) in concepts)
            Console.WriteLine($"  {name,-8} {role,-28} {syntax}");
        Debug.Assert(concepts.Length == 4);
    }

    private static void DemoPropertyEvaluationOrder()
    {
        Console.WriteLine("-- property: later wins (evaluation order) --");
        // 模拟 MSBuild：同名属性后写覆盖前写
        var bag = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["TargetFramework"] = "net8.0",
            ["MyFlag"] = "false",
        };
        bag["TargetFramework"] = "net10.0"; // later definition wins
        bag["MyFlag"] = "true";
        string expanded = $"{bag["TargetFramework"]}-{bag["MyFlag"]}";
        Debug.Assert(expanded == "net10.0-true");
        Console.WriteLine($"  after override: TargetFramework={bag["TargetFramework"]}, MyFlag={bag["MyFlag"]}");
        Console.WriteLine("  引用语法: $(PropertyName)；条件: Condition=\"'$(Configuration)'=='Release'\"");
    }

    private static void DemoItemMetadata()
    {
        Console.WriteLine("-- item list + metadata --");
        var items = new List<(string Include, string? CopyToOutput)>
        {
            ("Program.cs", null),
            ("appsettings.json", "PreserveNewest"),
            ("Serilog", "PackageReference"),
        };
        foreach (var (include, meta) in items)
            Console.WriteLine($"  Include={include}, Meta={meta ?? "(none)"}");
        Debug.Assert(items.Count == 3);
        Console.WriteLine("  常见 Item: Compile / PackageReference / ProjectReference / None / Content");
    }

    private static void DemoTargetTaskGraph()
    {
        Console.WriteLine("-- target graph (simplified) --");
        string[] pipeline = ["Restore", "BeforeBuild", "CoreBuild(Csc)", "AfterBuild", "Publish"];
        foreach (string step in pipeline)
            Console.WriteLine($"  → {step}");
        Debug.Assert(pipeline.Contains("CoreBuild(Csc)"));
        Console.WriteLine("  自定义: <Target Name=\"Hi\" BeforeTargets=\"Build\"><Message .../></Target>");
        Console.WriteLine("  Task 是 ITask 的 .NET 类；Csc 调 Roslyn，平时不必手写");
    }

    private static void DemoTwoPhaseEvaluation()
    {
        Console.WriteLine("-- two-phase: evaluation then execution --");
        // 模拟：求值阶段展开属性；执行阶段才跑 Target
        var props = new Dictionary<string, string> { ["Later"] = "" };
        // phase 1: Later 尚未定义时引用 → 空
        string early = props.GetValueOrDefault("Out") ?? $"prefix-{props["Later"]}";
        props["Later"] = "value";
        // 若 Out 在 Later 定义前被求值，会“钉死”空值
        string wrongOrder = $"prefix-{""}"; // what early evaluation captured
        string rightOrder = $"prefix-{props["Later"]}";
        Debug.Assert(wrongOrder == "prefix-");
        Debug.Assert(rightOrder == "prefix-value");
        Console.WriteLine($"  wrong order Out='{wrongOrder}' vs right order Out='{rightOrder}'");
        Console.WriteLine("  阶段1 求值全部 Property/Item；阶段2 才执行 Target/Task");
        Console.WriteLine("  🔶 CMake configure vs build；Make recipe 是 shell，MSBuild Task 是托管类");
        _ = early;
    }
}
