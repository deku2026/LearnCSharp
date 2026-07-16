// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第3部分-质量门禁.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section03_QualityGate
// Item     : EditorConfigRules
// Topic id : stage10/section03/editorconfig_rules
//
// .editorconfig：格式 + C# 偏好 + 诊断严重性；EnforceCodeStyleInBuild。

using System.Diagnostics;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section03;

internal static class EditorConfigRules
{
    [LearnTopic("stage10/section03/editorconfig_rules")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== EditorConfigRules ===");
        DemoWhatIsEditorConfig();
        DemoSampleFile();
        DemoDiagnosticSeverityEntries();
        DemoEnforceInBuild();
        DemoFormatConsistency();
        return 0;
    }

    private static void DemoWhatIsEditorConfig()
    {
        Console.WriteLine("-- .editorconfig is cross-IDE --");
        Console.WriteLine("  层级: 从文件目录向上合并，root=true 停止");
        Console.WriteLine("  管: 缩进/换行/字符集 + C# 风格 + 诊断严重性");
        Console.WriteLine("  与 StyleCop 等可并存；优先团队一份根配置");
        Debug.Assert(true);
    }

    private static void DemoSampleFile()
    {
        Console.WriteLine("-- sample root .editorconfig --");
        string[] lines =
        [
            "root = true",
            "",
            "[*]",
            "indent_style = space",
            "indent_size = 4",
            "end_of_line = crlf",
            "charset = utf-8",
            "insert_final_newline = true",
            "trim_trailing_whitespace = true",
            "",
            "[*.cs]",
            "dotnet_sort_system_directives_first = true",
            "csharp_new_line_before_open_brace = all",
            "csharp_style_var_for_built_in_types = false:suggestion",
        ];
        foreach (string line in lines)
            Console.WriteLine($"  {line}");
        Debug.Assert(lines[0].StartsWith("root", StringComparison.Ordinal));
    }

    private static void DemoDiagnosticSeverityEntries()
    {
        Console.WriteLine("-- raise/lower diagnostic severity --");
        string[] rules =
        [
            "dotnet_diagnostic.CA1062.severity = error",
            "dotnet_diagnostic.IDE0007.severity = none",
            "dotnet_diagnostic.CS1591.severity = warning",
        ];
        foreach (string r in rules)
            Console.WriteLine($"  {r}");
        Debug.Assert(rules.All(r => r.Contains("dotnet_diagnostic.", StringComparison.Ordinal)));
        Console.WriteLine("  也可用 glob: [test/**.cs] 下放宽某些规则");
    }

    private static void DemoEnforceInBuild()
    {
        Console.WriteLine("-- EnforceCodeStyleInBuild --");
        Console.WriteLine("  默认: 许多 IDE* 风格只在编辑器显示，build 不失败");
        Console.WriteLine("  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>");
        Console.WriteLine("  → 风格规则参与编译诊断，CI 与本地一致");
        string prop = "EnforceCodeStyleInBuild";
        Debug.Assert(prop.Contains("Build", StringComparison.Ordinal));
    }

    private static void DemoFormatConsistency()
    {
        Console.WriteLine("-- format consistency mini-demo --");
        // 展示“风格偏好”的可检测差异（教育用，非真 IDE 引擎）
        string messy = "if(x){return 1;}";
        string tidy = "if (x)\n{\n    return 1;\n}";
        Debug.Assert(messy.Length < tidy.Length);
        Console.WriteLine($"  messy length={messy.Length}, tidy length={tidy.Length}");
        Console.WriteLine("  工具: dotnet format（可按 editorconfig 修复）");
        int spaces = Encoding.UTF8.GetByteCount("    ");
        Debug.Assert(spaces == 4);
        Console.WriteLine($"  indent_size=4 → UTF-8 bytes for 4 spaces: {spaces}");
    }
}
