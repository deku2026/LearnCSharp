// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第5部分-发布裁剪与AOT.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section05_PublishTrimmingAndAOT
// Item     : DotnetPublishModes
// Topic id : stage10/section05/dotnet_publish_modes
//
// framework-dependent / self-contained / single-file 发布模式。

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section05;

internal static class DotnetPublishModes
{
    [LearnTopic("stage10/section05/dotnet_publish_modes")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DotnetPublishModes ===");
        DemoThreeModes();
        DemoSizeTradeoffs();
        DemoCliFlags();
        DemoCurrentProcessHints();
        DemoWhenToChoose();
        return 0;
    }

    private static void DemoThreeModes()
    {
        Console.WriteLine("-- three publish modes --");
        (string Mode, string Ships, string NeedsOnTarget)[] modes =
        [
            ("framework-dependent (default)", "app DLLs + apphost", "预装 .NET 运行时"),
            ("self-contained", "app + 完整运行时", "无需预装 .NET"),
            ("single-file", "打成一个主文件（可叠加上两者）", "取决于是否 self-contained"),
        ];
        foreach (var (mode, ships, needs) in modes)
            Console.WriteLine($"  {mode}\n    ships: {ships}\n    target: {needs}");
        Debug.Assert(modes.Length == 3);
    }

    private static void DemoSizeTradeoffs()
    {
        Console.WriteLine("-- size / portability tradeoffs --");
        Console.WriteLine("  FDD: 数百 KB–数 MB；跨平台托管 DLL，apphost 平台相关");
        Console.WriteLine("  SCD: ~60–100MB+；必须指定 RID");
        Console.WriteLine("  single-file: 便于分发；解压/捆绑策略影响启动与诊断");
        (string Mode, int ApproxMb)[] sizes = [("FDD", 2), ("SCD", 80), ("SCD+trim", 30)];
        foreach (var (mode, mb) in sizes)
            Console.WriteLine($"  ~{mode}: {mb} MB (order-of-magnitude demo)");
        Debug.Assert(sizes[0].ApproxMb < sizes[1].ApproxMb);
    }

    private static void DemoCliFlags()
    {
        Console.WriteLine("-- CLI / properties --");
        string[] cmds =
        [
            "dotnet publish -c Release",
            "dotnet publish -c Release -r win-x64 --self-contained true",
            "dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true",
            "dotnet publish -c Release -r win-x64 -p:SelfContained=true -p:PublishSingleFile=true",
        ];
        foreach (string c in cmds)
            Console.WriteLine($"  $ {c}");
        Debug.Assert(cmds.Any(c => c.Contains("PublishSingleFile", StringComparison.Ordinal)));
        Console.WriteLine("  输出目录: bin/<Config>/<TFM>/<RID>/publish/");
    }

    private static void DemoCurrentProcessHints()
    {
        Console.WriteLine("-- this process (dev build, not necessarily published) --");
        Console.WriteLine($"  Framework={RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"  RID={RuntimeInformation.RuntimeIdentifier}");
        Console.WriteLine($"  ProcessPath={Environment.ProcessPath}");
        bool singleFileHint = string.IsNullOrEmpty(typeof(DotnetPublishModes).Assembly.Location);
        Console.WriteLine($"  Assembly.Location empty? {singleFileHint} (true often means single-file bundle)");
        Debug.Assert(!string.IsNullOrEmpty(RuntimeInformation.FrameworkDescription));
    }

    private static void DemoWhenToChoose()
    {
        Console.WriteLine("-- when to choose --");
        (string Scenario, string Choice)[] table =
        [
            ("服务器已装运行时、滚动升级", "FDD"),
            ("桌面工具发给无 .NET 用户", "SCD (+ optional single-file)"),
            ("容器基础镜像含 aspnet 运行时", "FDD 更小"),
            ("最小依赖原生部署", "Native AOT（另专题）"),
        ];
        foreach (var (scenario, choice) in table)
            Console.WriteLine($"  {scenario,-28} → {choice}");
        Debug.Assert(table.Length == 4);
    }
}
