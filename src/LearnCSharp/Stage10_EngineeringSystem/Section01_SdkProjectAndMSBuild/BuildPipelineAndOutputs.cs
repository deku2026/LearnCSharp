// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第1部分-SDK项目与MSBuild构建.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section01_SdkProjectAndMSBuild
// Item     : BuildPipelineAndOutputs
// Topic id : stage10/section01/build_pipeline_and_outputs
//
// restore → build → bin/obj、增量编译、Debug vs Release。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section01;

internal static class BuildPipelineAndOutputs
{
    [LearnTopic("stage10/section01/build_pipeline_and_outputs")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== BuildPipelineAndOutputs ===");
        DemoPipelineStages();
        DemoBinVsObj();
        DemoIncrementalBuildModel();
        DemoDebugVsRelease();
        DemoCliCommands();
        return 0;
    }

    private static void DemoPipelineStages()
    {
        Console.WriteLine("-- source → runnable pipeline --");
        (string Stage, string What)[] stages =
        [
            ("restore", "解析 PackageReference，下载/缓存包，写 project.assets.json"),
            ("build", "编译 + 复制内容 → 输出程序集"),
            ("test", "发现并运行测试程序集（另一阶段）"),
            ("pack", "打 nupkg（库）"),
            ("publish", "为部署整理输出（可含运行时/裁剪/AOT）"),
        ];
        foreach ((string? stage, string? what) in stages)
            Console.WriteLine($"  {stage,-8} {what}");
        Debug.Assert(stages[0].Stage == "restore");
    }

    private static void DemoBinVsObj()
    {
        Console.WriteLine("-- bin vs obj (do NOT commit) --");
        Console.WriteLine("  obj/: 中间产物 — assets、生成文件、缓存、编译器临时输出");
        Console.WriteLine("  bin/: 对外输出 — DLL/EXE、deps.json、runtimeconfig.json、复制的内容");
        Console.WriteLine("  典型: bin/Debug/net10.0/ 与 obj/Debug/net10.0/");
        string[] ignore = ["bin/", "obj/"];
        Debug.Assert(ignore.All(p => p.EndsWith('/')));
        Console.WriteLine("  .gitignore 必忽略 bin/obj；干净构建: dotnet clean 或删目录");

        string? location = typeof(BuildPipelineAndOutputs).Assembly.Location;
        if (!string.IsNullOrEmpty(location))
        {
            string dir = Path.GetDirectoryName(location) ?? "";
            Console.WriteLine($"  本程序集目录片段: ...{Path.DirectorySeparatorChar}{Path.GetFileName(dir)}");
            Debug.Assert(Directory.Exists(dir));
        }
        else
        {
            Console.WriteLine("  Assembly.Location empty (可能 single-file) — 仍说明产物布局由 SDK 管理");
        }
    }

    private static void DemoIncrementalBuildModel()
    {
        Console.WriteLine("-- incremental build --");
        Console.WriteLine("  MSBuild 用 Inputs/Outputs 时间戳判断 Target 是否跳过");
        Console.WriteLine("  源未改 → 第二次 build 几乎只检查依赖图 → 很快");
        Console.WriteLine("  改 csproj/包版本/条件编译 → 可能全量重编");
        // 微型“缓存”模型
        Dictionary<string, DateTimeOffset> cache = new Dictionary<string, DateTimeOffset>();
        DateTimeOffset t0 = DateTimeOffset.UtcNow;
        cache["Program.cs"] = t0;
        DateTimeOffset outTime = t0.AddSeconds(1);
        bool skip = cache["Program.cs"] < outTime;
        Debug.Assert(skip);
        Console.WriteLine($"  demo skip rebuild when input older than output: {skip}");
    }

    private static void DemoDebugVsRelease()
    {
        Console.WriteLine("-- Debug vs Release --");
        Console.WriteLine("  Debug: 优化少、符号全、DEBUG 常量、便于调试");
        Console.WriteLine("  Release: Optimize=true、常去 DEBUG、更适合部署/基准");
        Console.WriteLine("  ⚠ 二者是不同构建：性能对比必须用 Release");
#if DEBUG
        const string mode = "DEBUG";
#else
        const string mode = "RELEASE";
#endif
        Console.WriteLine($"  当前编译符号模式: {mode}");
        Console.WriteLine($"  Debugger.IsAttached={Debugger.IsAttached}");
        // mode 受 #if 切换为 DEBUG/RELEASE 之一；用变量承载避免常量折叠 (CS8793)
        string modeValue = mode;
        Debug.Assert(modeValue is "DEBUG" or "RELEASE");
    }

    private static void DemoCliCommands()
    {
        Console.WriteLine("-- essential CLI --");
        string[] cmds =
        [
            "dotnet restore",
            "dotnet build -c Debug",
            "dotnet build -c Release --no-restore",
            "dotnet clean",
            "dotnet run --no-build",
            "dotnet publish -c Release",
        ];
        foreach (string c in cmds)
            Console.WriteLine($"  $ {c}");
        Debug.Assert(cmds.Length >= 5);
    }
}
