// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第3部分-质量门禁.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section03_QualityGate
// Item     : WarningGovernance
// Topic id : stage10/section03/warning_governance
//
// TreatWarningsAsErrors 与细粒度警告治理。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section03;

internal static class WarningGovernance
{
    [LearnTopic("stage10/section03/warning_governance")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== WarningGovernance ===");
        DemoTreatWarningsAsErrors();
        DemoGranularControls();
        DemoPragmaAndSuppress();
        DemoGovernancePolicy();
        DemoMiniGateSimulator();
        return 0;
    }

    private static void DemoTreatWarningsAsErrors()
    {
        Console.WriteLine("-- TreatWarningsAsErrors --");
        Console.WriteLine("  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>");
        Console.WriteLine("  或: dotnet build -warnaserror");
        Console.WriteLine("  效果: 任意警告 → 构建失败 → CI 红灯");
        Console.WriteLine("  适合: 新项目从一开始开启；老项目逐步迁移");
        string[] switches = ["TreatWarningsAsErrors", "-warnaserror", "/warnaserror"];
        Debug.Assert(switches.Length == 3);
    }

    private static void DemoGranularControls()
    {
        Console.WriteLine("-- granular MSBuild knobs --");
        (string Prop, string Role)[] knobs =
        [
            ("WarningsAsErrors", "指定 ID 列表当错误（可不全开）"),
            ("WarningsNotAsErrors", "全局 warnaserror 时的豁免列表"),
            ("NoWarn", "完全压制某些 ID（慎用）"),
            ("WarningLevel", "编译器警告等级 0–5"),
        ];
        foreach ((string? prop, string? role) in knobs)
            Console.WriteLine($"  {prop,-22} {role}");
        Debug.Assert(knobs.Length == 4);
        Console.WriteLine("  例: <WarningsAsErrors>CS8618;CA1062</WarningsAsErrors>");
    }

    private static void DemoPragmaAndSuppress()
    {
        Console.WriteLine("-- local suppression (last resort) --");
        Console.WriteLine("  #pragma warning disable CSXXXX");
        Console.WriteLine("  #pragma warning restore CSXXXX");
        Console.WriteLine("  [SuppressMessage(\"Category\", \"CAXXXX:...\")]");
        Console.WriteLine("  .editorconfig: severity = none（团队级）");
        Console.WriteLine("  要求: 注释写明为什么 + 跟踪 issue");
        string[] tools = ["pragma", "SuppressMessage", "editorconfig", "NoWarn"];
        Debug.Assert(tools.Length == 4);
    }

    private static void DemoGovernancePolicy()
    {
        Console.WriteLine("-- recommended policy --");
        string[] policy =
        [
            "新代码: TreatWarningsAsErrors=true",
            "先修零成本警告，再升分析级别",
            "禁止无说明的大范围 NoWarn",
            "抑制必须 scoped（文件/行），并有过期计划",
            "目录级 Directory.Build.props 统一门禁",
        ];
        foreach (string p in policy)
            Console.WriteLine($"  • {p}");
        Debug.Assert(policy.Length >= 4);
    }

    private static void DemoMiniGateSimulator()
    {
        Console.WriteLine("-- mini gate simulator --");
        List<(string Id, string Severity)> findings = new List<(string Id, string Severity)>
        {
            ("CS0219", "warning"),
            ("CA1822", "warning"),
            ("CS0029", "error"),
        };
        bool warnAsError = true;
        int errors = findings.Count(f =>
            f.Severity == "error" || (warnAsError && f.Severity == "warning"));
        Debug.Assert(errors == 3);
        Console.WriteLine($"  findings={findings.Count}, warnAsError={warnAsError} → fail count={errors}");
        Console.WriteLine("  CI 应: build 失败即阻断合并");
    }
}
