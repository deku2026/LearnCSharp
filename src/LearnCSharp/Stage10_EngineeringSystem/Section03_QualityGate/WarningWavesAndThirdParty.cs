// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第3部分-质量门禁.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section03_QualityGate
// Item     : WarningWavesAndThirdParty
// Topic id : stage10/section03/warning_waves_and_third_party
//
// 警告波次、第三方分析器、遗留代码升级战术。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section03;

internal static class WarningWavesAndThirdParty
{
    [LearnTopic("stage10/section03/warning_waves_and_third_party")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WarningWavesAndThirdParty ===");
        DemoWarningWaves();
        DemoThirdPartyNoise();
        DemoLegacyUpgradeTactics();
        DemoBaselineStrategy();
        DemoChecklist();
        return 0;
    }

    private static void DemoWarningWaves()
    {
        Console.WriteLine("-- warning waves --");
        Console.WriteLine("  编译器/分析器随 SDK 版本引入新警告（波次）");
        Console.WriteLine("  AnalysisLevel / LangVersion 升级可能突然“变红”");
        Console.WriteLine("  策略: 钉住 SDK 与 AnalysisLevel，有计划地升波次");
        string[] levers = ["AnalysisLevel", "AnalysisMode", "WarningLevel", "SDK band"];
        foreach (string l in levers)
            Console.WriteLine($"  lever: {l}");
        Debug.Assert(levers.Contains("AnalysisLevel"));
    }

    private static void DemoThirdPartyNoise()
    {
        Console.WriteLine("-- third-party analyzer noise --");
        Console.WriteLine("  多包叠加 → 规则冲突/重复/过严");
        Console.WriteLine("  做法: 一次引入一个包；用 editorconfig 调 severity");
        Console.WriteLine("  生成代码: generated_code = true 段放宽");
        string[] generated =
        [
            "[*.g.cs]",
            "generated_code = true",
            "dotnet_diagnostic.CA*.severity = none",
        ];
        foreach (string line in generated)
            Console.WriteLine($"  {line}");
        Debug.Assert(generated[1].Contains("generated_code", StringComparison.Ordinal));
    }

    private static void DemoLegacyUpgradeTactics()
    {
        Console.WriteLine("-- legacy codebase upgrade tactics --");
        string[] steps =
        [
            "1. 先能 build（关掉 warnaserror）建立基线计数",
            "2. 按目录/项目开启 Nullable / 分析器",
            "3. 新文件更严（editorconfig path 规则）",
            "4. 清零一批后再 TreatWarningsAsErrors",
            "5. 第三方与生成代码单独策略",
        ];
        foreach (string s in steps)
            Console.WriteLine($"  {s}");
        Debug.Assert(steps.Length == 5);
    }

    private static void DemoBaselineStrategy()
    {
        Console.WriteLine("-- baseline / ratchet --");
        // 棘轮：只允许警告数下降
        int baseline = 120;
        int current = 95;
        bool ok = current <= baseline;
        Debug.Assert(ok);
        Console.WriteLine($"  baseline={baseline}, current={current}, ratchet ok={ok}");
        Console.WriteLine("  CI 可比较 warning 计数或 SARIF；禁止只增不减");
    }

    private static void DemoChecklist()
    {
        Console.WriteLine("-- practical checklist --");
        string[] items =
        [
            "Directory.Build.props 统一 AnalysisLevel",
            "根 .editorconfig + EnforceCodeStyleInBuild 视团队而定",
            "第三方分析器 PrivateAssets=all",
            "升级 SDK 时单开 PR 处理新波次",
            "文档化允许的 suppress 流程",
        ];
        foreach (string i in items)
            Console.WriteLine($"  ☐ {i}");
        Debug.Assert(items.Length == 5);
    }
}
