// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第5部分-JIT编译与分层编译.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section05_JITAndTieredCompilation
// Item     : Tier0PriorityAndR2R
// Topic id : stage11/section05/tier0_priority_and_r2r
//
// Lesson: ReadyToRun pre-generates code; tiering can still re-JIT hot methods.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section05;

internal static class Tier0PriorityAndR2R
{
    [LearnTopic("stage11/section05/tier0_priority_and_r2r")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Tier0PriorityAndR2R ===");
        DemoR2RIdea();
        DemoReadyToRunFlag();
        DemoPriority();
        return 0;
    }

    private static void DemoR2RIdea()
    {
        Console.WriteLine("-- ReadyToRun (R2R) --");
        Console.WriteLine("  Crossgen/ILC produces managed PE with native code blobs for common methods.");
        Console.WriteLine("  Startup skips full JIT for those methods; version bubbles handle churn.");
        Console.WriteLine("  Publish: dotnet publish -p:PublishReadyToRun=true");
    }

    private static void DemoReadyToRunFlag()
    {
        Console.WriteLine("-- assembly ReadyToRun flag (if present) --");
        Assembly asm = Assembly.GetExecutingAssembly();
        // ReadyToRunAttribute may be absent on framework-dependent dev builds
        bool hasR2R = asm.GetCustomAttributes(inherit: false)
            .Any(a => a.GetType().Name.Contains("ReadyToRun", StringComparison.Ordinal));
        Console.WriteLine($"  Executing assembly name={asm.GetName().Name}");
        Console.WriteLine($"  ReadyToRun-related attribute present={hasR2R} (often false in Debug F5)");
        Console.WriteLine($"  Location={asm.Location}");
        Debug.Assert(asm.GetName().Name is not null);
    }

    private static void DemoPriority()
    {
        Console.WriteLine("-- tier0 priority / call counting --");
        Console.WriteLine("  Cold methods stay Tier0; hot methods scheduled for Tier1 rejit.");
        Console.WriteLine("  R2R code can still be replaced by Tier1+PGO for peak throughput.");
        int v = 0;
        for (int i = 0; i < 1000; i++)
            v += i;
        Debug.Assert(v == 999 * 1000 / 2);
        Console.WriteLine($"  warm sum={v}");
    }
}
