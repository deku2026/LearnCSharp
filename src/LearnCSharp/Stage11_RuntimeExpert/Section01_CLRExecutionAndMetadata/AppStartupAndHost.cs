// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第1部分-CLR执行模型与元数据.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section01_CLRExecutionAndMetadata
// Item     : AppStartupAndHost
// Topic id : stage11/section01/app_startup_and_host
//
// Lesson: hostfxr / runtimeconfig / entry point; environment the CLR sees at startup.

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section01;

internal static class AppStartupAndHost
{
    [LearnTopic("stage11/section01/app_startup_and_host")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AppStartupAndHost ===");
        DemoHostEnvironment();
        DemoRuntimeConfigSurfaces();
        DemoCommandLineAndMain();
        return 0;
    }

    private static void DemoHostEnvironment()
    {
        Console.WriteLine("-- host / process environment --");
        Console.WriteLine($"  ProcessId={Environment.ProcessId}");
        Console.WriteLine($"  CurrentDirectory={Environment.CurrentDirectory}");
        Console.WriteLine($"  Version={Environment.Version}");
        Console.WriteLine($"  ProcessorCount={Environment.ProcessorCount}");
        Console.WriteLine($"  RuntimeIdentifier={RuntimeInformation.RuntimeIdentifier}");
        Debug.Assert(Environment.ProcessId > 0);
        Debug.Assert(Environment.Version.Major >= 8);
    }

    private static void DemoRuntimeConfigSurfaces()
    {
        Console.WriteLine("-- runtime configuration surfaces --");
        Console.WriteLine("  App starts via apphost → hostfxr → hostpolicy → coreclr.");
        Console.WriteLine("  *.runtimeconfig.json sets TFM, frameworks, configProperties (GC, etc.).");
        Console.WriteLine("  DOTNET_* env vars can override many runtime knobs.");
        string? tiered = Environment.GetEnvironmentVariable("DOTNET_TieredCompilation");
        string? pgo = Environment.GetEnvironmentVariable("DOTNET_TieredPGO");
        Console.WriteLine($"  DOTNET_TieredCompilation={tiered ?? "(default)"}");
        Console.WriteLine($"  DOTNET_TieredPGO={pgo ?? "(default)"}");
        Console.WriteLine($"  GCSettings.IsServerGC={System.Runtime.GCSettings.IsServerGC}");
        Console.WriteLine($"  GCSettings.LatencyMode={System.Runtime.GCSettings.LatencyMode}");
    }

    private static void DemoCommandLineAndMain()
    {
        Console.WriteLine("-- entry point model --");
        string[] cli = Environment.GetCommandLineArgs();
        Console.WriteLine($"  CommandLineArgs[0] (host path)={cli[0]}");
        Console.WriteLine($"  Arg count={cli.Length}");
        Console.WriteLine("  LearnCSharp dispatches topics via [LearnTopic] — not a single Main body.");
        Debug.Assert(cli.Length >= 1);
        Console.WriteLine($"  AppContext.BaseDirectory={AppContext.BaseDirectory}");
        Debug.Assert(Directory.Exists(AppContext.BaseDirectory));
    }
}
