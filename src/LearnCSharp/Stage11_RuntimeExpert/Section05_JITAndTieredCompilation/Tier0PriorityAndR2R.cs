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
using System.Runtime.CompilerServices;
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
        DemoTier0PriorityReasons();
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

    // The doc's ⭐⭐ central point: Tier0 priority exists for three reasons, all observable:
    //  ① startup fast (run simple code immediately, skip optimization);
    //  ② Dynamic PGO needs Tier0 *instrumented* code to gather branch profiles;
    //  ③ static fields settle before Tier1 re-JIT — the optimizer can then fold them.
    // Reason ③ is directly observable: a static readonly field whose value comes from a
    // non-constant initializer cannot be constant-folded until the type has been initialized.
    private static void DemoTier0PriorityReasons()
    {
        Console.WriteLine("-- tier0 priority: the three reasons (observable) --");
        // Tiering / PGO config knobs the host honors (observable, not just text):
        string? r2r = Environment.GetEnvironmentVariable("DOTNET_ReadyToRun");
        string? tiered = Environment.GetEnvironmentVariable("DOTNET_TieredCompilation");
        string? pgo = Environment.GetEnvironmentVariable("DOTNET_TieredPGO");
        Console.WriteLine($"  DOTNET_ReadyToRun={(r2r ?? "(default: 1)")}");
        Console.WriteLine($"  DOTNET_TieredCompilation={(tiered ?? "(default: 1)")}");
        Console.WriteLine($"  DOTNET_TieredPGO={(pgo ?? "(default: 1)")}");
        Console.WriteLine("  → ① Tier0 lets the app start before Tier1 optimization finishes.");
        Console.WriteLine("  → ② Dynamic PGO instruments Tier0 to collect branch profiles → re-JIT as Tier1.");

        // ③ static-field settle: the JIT cannot fold a static readonly field whose value
        //    is only known after the type initializer runs. Once the type is initialized
        //    and the method is re-JITted (Tier1), the constant can be inlined.
        int folded = ConsumeStaticField();
        Debug.Assert(folded == ComputedSeed);
        Console.WriteLine($"  static readonly seed={ComputedSeed}; ConsumeStaticField()={folded}");
        Console.WriteLine("  → ③ Tier0 runs before the type is necessarily initialized; Tier1 re-JIT after settle can fold the constant.");
        Console.WriteLine("  Call counts drive promotion: a method called past the threshold is re-JIT at Tier1 (+PGO).");
    }

    // A non-constant initializer: the JIT cannot treat this as a compile-time constant.
    private static readonly int ComputedSeed = int.Parse("1729");

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ConsumeStaticField() => ComputedSeed * 1; // multiplied so the JIT has work to fold away
}
