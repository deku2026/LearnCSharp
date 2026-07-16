// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第2部分-NuGet包管理.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section02_NuGetPackageManagement
// Item     : DotnetToolsDnx
// Topic id : stage10/section02/dotnet_tools_dnx
//
// 全局/本地工具 + .NET 10 dnx / dotnet tool exec。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section02;

internal static class DotnetToolsDnx
{
    [LearnTopic("stage10/section02/dotnet_tools_dnx")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DotnetToolsDnx ===");
        DemoWhatIsTool();
        DemoGlobalVsLocal();
        DemoManifest();
        DemoDnxAndToolExec();
        DemoWhenToUse();
        return 0;
    }

    private static void DemoWhatIsTool()
    {
        Console.WriteLine("-- what is a .NET tool --");
        Console.WriteLine("  特殊 NuGet 包：打包为可执行 CLI（非库引用）");
        Console.WriteLine("  例: dotnet-ef、dotnet-format、自定义代码生成器");
        Console.WriteLine("  安装后通过 shims 调用，不必自己写 PATH 到 DLL");
        string[] examples = ["dotnet-ef", "dotnetsay", "csharpier"];
        Debug.Assert(examples.Length >= 2);
        Console.WriteLine($"  samples: {string.Join(", ", examples)}");
    }

    private static void DemoGlobalVsLocal()
    {
        Console.WriteLine("-- global vs local tools --");
        Console.WriteLine("  全局: dotnet tool install -g <package>");
        Console.WriteLine("        装到 ~/.dotnet/tools，用户级，版本易漂移");
        Console.WriteLine("  本地: dotnet new tool-manifest && dotnet tool install <package>");
        Console.WriteLine("        写入 .config/dotnet-tools.json，随仓库共享 → 推荐团队");
        (string Kind, string Scope)[] rows =
        [
            ("global", "用户机器"),
            ("local", "仓库/目录清单"),
        ];
        foreach (var (kind, scope) in rows)
            Console.WriteLine($"  {kind,-8} → {scope}");
        Debug.Assert(rows.Length == 2);
    }

    private static void DemoManifest()
    {
        Console.WriteLine("-- local tool manifest (conceptual JSON) --");
        string[] json =
        [
            "{",
            "  \"version\": 1,",
            "  \"isRoot\": true,",
            "  \"tools\": {",
            "    \"dotnet-ef\": { \"version\": \"9.0.0\", \"commands\": [\"dotnet-ef\"] }",
            "  }",
            "}",
        ];
        foreach (string line in json)
            Console.WriteLine($"  {line}");
        Debug.Assert(json.Any(l => l.Contains("tools", StringComparison.Ordinal)));
        Console.WriteLine("  restore: dotnet tool restore");
    }

    private static void DemoDnxAndToolExec()
    {
        Console.WriteLine("-- .NET 10: one-shot run (npx-like) --");
        Console.WriteLine("  dotnet tool exec <package> [args]  // 不必先 install");
        Console.WriteLine("  dnx <package> [args]               // 短别名，转发到 CLI");
        Console.WriteLine("  适合: CI 临时工具、文档中的一键命令、试用新工具");
        string[] cmds =
        [
            "dotnet tool exec dotnetsay Hello",
            "dnx dotnetsay Hello",
        ];
        foreach (string c in cmds)
            Console.WriteLine($"  $ {c}");
        Debug.Assert(cmds.Any(c => c.StartsWith("dnx ", StringComparison.Ordinal)));
    }

    private static void DemoWhenToUse()
    {
        Console.WriteLine("-- choose wisely --");
        (string Scenario, string Choice)[] table =
        [
            ("团队统一 EF 迁移工具", "local tool + manifest"),
            ("个人本机便利脚本", "global tool"),
            ("README 一次性示例", "dnx / tool exec"),
            ("库依赖 API", "PackageReference（不是 tool）"),
        ];
        foreach (var (scenario, choice) in table)
            Console.WriteLine($"  {scenario,-22} → {choice}");
        Debug.Assert(table.Length == 4);
    }
}
