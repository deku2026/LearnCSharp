// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第1部分-SDK项目与MSBuild构建.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section01_SdkProjectAndMSBuild
// Item     : RuntimeIdentifierRid
// Topic id : stage10/section01/runtime_identifier_rid
//
// RID：目标 OS+架构；.NET 8+ 可移植 RID 图；对接 publish/AOT。

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section01;

internal static class RuntimeIdentifierRid
{
    [LearnTopic("stage10/section01/runtime_identifier_rid")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== RuntimeIdentifierRid ===");
        DemoWhatIsRid();
        DemoCurrentProcessRidHints();
        DemoPortableRidGraph();
        DemoCsprojAndCli();
        return 0;
    }

    private static void DemoWhatIsRid()
    {
        Console.WriteLine("-- what is RID --");
        Console.WriteLine("  Runtime Identifier = OS + architecture (+ optional version/flavor)");
        Console.WriteLine("  用途: 选原生依赖、self-contained 运行时包、Native AOT 目标");
        string[] examples = ["win-x64", "linux-x64", "osx-arm64", "linux-musl-x64", "browser-wasm"];
        foreach (string rid in examples)
            Console.WriteLine($"  e.g. {rid}");
        Debug.Assert(examples.All(r => r.Contains('-')));
    }

    private static void DemoCurrentProcessRidHints()
    {
        Console.WriteLine("-- current process environment --");
        Console.WriteLine($"  OSDescription={RuntimeInformation.OSDescription}");
        Console.WriteLine($"  OSArchitecture={RuntimeInformation.OSArchitecture}");
        Console.WriteLine($"  ProcessArchitecture={RuntimeInformation.ProcessArchitecture}");
        Console.WriteLine($"  RuntimeIdentifier={RuntimeInformation.RuntimeIdentifier}");
        Console.WriteLine($"  Framework={RuntimeInformation.FrameworkDescription}");
        Debug.Assert(!string.IsNullOrEmpty(RuntimeInformation.RuntimeIdentifier));
        Debug.Assert(Enum.IsDefined(RuntimeInformation.OSArchitecture));
    }

    private static void DemoPortableRidGraph()
    {
        Console.WriteLine("-- .NET 8+ portable RID graph --");
        Console.WriteLine("  旧: 巨大 RID 图（win10-x64 等细分）→ 包体积/复杂度高");
        Console.WriteLine("  新: 可移植 RID（win-x64, linux-x64…）为主，减少细分条目");
        Console.WriteLine("  发布仍可指定具体 RID；any 表示平台无关托管资产");
        (string Rid, string Note)[] portable =
        [
            ("win-x64", "Windows x64 可移植"),
            ("linux-x64", "glibc Linux x64"),
            ("linux-musl-x64", "Alpine/musl"),
            ("osx-arm64", "Apple Silicon"),
            ("any", "纯托管 / 未知平台资产"),
        ];
        foreach ((string? rid, string? note) in portable)
            Console.WriteLine($"  {rid,-16} {note}");
        Debug.Assert(portable.Any(p => p.Rid == "any"));
    }

    private static void DemoCsprojAndCli()
    {
        Console.WriteLine("-- project + CLI --");
        Console.WriteLine("  <RuntimeIdentifier>win-x64</RuntimeIdentifier>  // 固定单一 RID");
        Console.WriteLine("  <RuntimeIdentifiers>win-x64;linux-x64;any</RuntimeIdentifiers>");
        Console.WriteLine("  CLI: dotnet publish -r win-x64 --self-contained");
        Console.WriteLine("  与 publish/trim/AOT 联动：无 RID 无法打出原生 self-contained");
        string cmd = "dotnet publish -c Release -r linux-x64";
        Debug.Assert(cmd.Contains("-r "));
        Console.WriteLine($"  sample: {cmd}");
    }
}
