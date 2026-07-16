// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第2部分-CLR对象模型与方法表.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section02_CLRObjectModelAndMethodTable
// Item     : AssemblyLoadContextTopic
// Topic id : stage11/section02/assembly_load_context
//
// Lesson: ALC isolates loads; collectible contexts can unload; default ALC for app.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section02;

internal static class AssemblyLoadContextTopic
{
    [LearnTopic("stage11/section02/assembly_load_context")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AssemblyLoadContextTopic ===");
        DemoDefaultContext();
        DemoCollectibleConcept();
        DemoTypeIdentityAcrossContexts();
        return 0;
    }

    private static void DemoDefaultContext()
    {
        Console.WriteLine("-- default AssemblyLoadContext --");
        AssemblyLoadContext def = AssemblyLoadContext.Default;
        Assembly asm = typeof(AssemblyLoadContextTopic).Assembly;
        AssemblyLoadContext? ctx = AssemblyLoadContext.GetLoadContext(asm);
        Console.WriteLine($"  Default.Name={def.Name ?? "(default)"}");
        Console.WriteLine($"  IsCollectible={def.IsCollectible}");
        Console.WriteLine($"  Our assembly ALC same as Default: {ReferenceEquals(ctx, def)}");
        Debug.Assert(ReferenceEquals(ctx, def));
        Console.WriteLine($"  Assemblies in default (sample): {def.Assemblies.Count()}");
    }

    private static void DemoCollectibleConcept()
    {
        Console.WriteLine("-- collectible ALC (plugin unload pattern) --");
        var alc = new AssemblyLoadContext("demo-collectible", isCollectible: true);
        Console.WriteLine($"  Created '{alc.Name}', IsCollectible={alc.IsCollectible}");
        WeakReference weakAlc = new(alc, trackResurrection: false);
        alc.Unload();
        alc = null!;
        for (int i = 0; i < 3 && weakAlc.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Console.WriteLine($"  After Unload + GC, ALC alive={weakAlc.IsAlive} (may still be briefly alive)");
        Console.WriteLine("  Real plugins: load from stream/path into collectible ALC, then drop roots & Unload.");
    }

    private static void DemoTypeIdentityAcrossContexts()
    {
        Console.WriteLine("-- type identity is ALC-scoped --");
        Console.WriteLine("  Same assembly bytes loaded in two ALCs ⇒ two distinct Type instances.");
        Console.WriteLine("  Casts/assignments across ALCs fail even if names match (plugin pitfall).");
        Type t = typeof(string);
        Console.WriteLine($"  typeof(string).Assembly.FullName={t.Assembly.GetName().Name}");
        Debug.Assert(t.Assembly.GetName().Name is "System.Private.CoreLib" or "System.Runtime" or not null);
    }
}
