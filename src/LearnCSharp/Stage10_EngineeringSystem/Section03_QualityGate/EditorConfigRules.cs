// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第3部分-质量门禁.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section03_QualityGate
// Item     : EditorConfigRules
// Topic id : stage10/section03/editorconfig_rules
//
// .editorconfig：读本仓库真实文件 + EnforceCodeStyleInBuild。

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
        DemoReadRealEditorConfig();
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
        Debug.Assert(File.Exists(Path.Combine(FindRepoRoot(), ".editorconfig")));
    }

    private static void DemoReadRealEditorConfig()
    {
        Console.WriteLine("-- real .editorconfig in this repo --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string path = Path.Combine(root, ".editorconfig");
        string text = File.ReadAllText(path);
        Debug.Assert(text.Contains("root = true", StringComparison.Ordinal));
        Debug.Assert(text.Contains("indent_size", StringComparison.Ordinal));
        Debug.Assert(text.Contains("[*.cs]", StringComparison.Ordinal));
        Debug.Assert(text.Contains("dotnet_diagnostic", StringComparison.Ordinal)
                     || text.Contains("csharp_style", StringComparison.Ordinal));
        Console.WriteLine($"  path={path}; length={text.Length}");
        Console.WriteLine("  has root=true, indent_size, [*.cs] style rules");
    }

    private static void DemoDiagnosticSeverityEntries()
    {
        Console.WriteLine("-- raise/lower diagnostic severity --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string text = File.ReadAllText(Path.Combine(root, ".editorconfig"));
        string[] samples =
        [
            "dotnet_diagnostic.IDE0005.severity",
            "csharp_style_var_for_built_in_types",
        ];
        foreach (string s in samples)
        {
            bool found = text.Contains(s, StringComparison.Ordinal);
            Console.WriteLine($"  {(found ? "found" : "missing")}: {s}");
            Debug.Assert(found);
        }
    }

    private static void DemoEnforceInBuild()
    {
        Console.WriteLine("-- EnforceCodeStyleInBuild (Directory.Build.props) --");
        string? root = FindRepoRoot();
        Debug.Assert(root is not null);
        string props = File.ReadAllText(Path.Combine(root, "Directory.Build.props"));
        Debug.Assert(props.Contains("EnforceCodeStyleInBuild", StringComparison.Ordinal));
        Console.WriteLine("  this repo sets EnforceCodeStyleInBuild=true → style in CI build");
    }

    private static void DemoFormatConsistency()
    {
        Console.WriteLine("-- format consistency mini-demo --");
        string messy = "if(x){return 1;}";
        string tidy = "if (x)\n{\n    return 1;\n}";
        Debug.Assert(messy.Length < tidy.Length);
        int spaces = Encoding.UTF8.GetByteCount("    ");
        Debug.Assert(spaces == 4);
        Console.WriteLine($"  messy={messy.Length}, tidy={tidy.Length}; 4 spaces = {spaces} UTF-8 bytes");
    }

    private static string? FindRepoRoot()
    {
        foreach (string start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? dir = new(start);
            while (dir is not null)
            {
                if (File.Exists(Path.Combine(dir.FullName, ".editorconfig"))
                    && File.Exists(Path.Combine(dir.FullName, "global.json")))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }

        return null;
    }
}
