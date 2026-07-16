// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第1部分-CLR执行模型与元数据.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section01_CLRExecutionAndMetadata
// Item     : AssembliesAndModules
// Topic id : stage11/section01/assemblies_and_modules
//
// Lesson: assembly = deployment/version unit; module = physical PE; metadata tables.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section01;

internal static class AssembliesAndModules
{
    [LearnTopic("stage11/section01/assemblies_and_modules")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AssembliesAndModules ===");
        DemoEntryAssembly();
        DemoModulesAndTypes();
        DemoReferencedAssemblies();
        return 0;
    }

    private static void DemoEntryAssembly()
    {
        Console.WriteLine("-- entry / executing assembly --");
        Assembly entry = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        Assembly exec = Assembly.GetExecutingAssembly();
        Console.WriteLine($"  Entry: {entry.GetName().Name} v{entry.GetName().Version}");
        Console.WriteLine($"  Executing: {exec.GetName().Name}");
        Console.WriteLine($"  Location: {exec.Location}");
        Console.WriteLine($"  IsDynamic={exec.IsDynamic}, IsCollectible={exec.IsCollectible}");
        Debug.Assert(exec.GetName().Name is not null);
        Debug.Assert(exec.GetTypes().Length > 0);
    }

    private static void DemoModulesAndTypes()
    {
        Console.WriteLine("-- modules (physical PE files inside assembly) --");
        Assembly asm = Assembly.GetExecutingAssembly();
        Module[] modules = asm.GetModules();
        Console.WriteLine($"  Module count={modules.Length}");
        foreach (Module m in modules)
        {
            Console.WriteLine($"  Module: {m.Name}, ScopeName={m.ScopeName}, MDStreamVersion={m.MDStreamVersion}");
            Debug.Assert(m.Assembly == asm);
        }

        Type t = typeof(AssembliesAndModules);
        Console.WriteLine($"  Type {t.FullName} lives in module {t.Module.Name}");
        Debug.Assert(t.Assembly == asm);
    }

    private static void DemoReferencedAssemblies()
    {
        Console.WriteLine("-- assembly references (metadata) --");
        Assembly asm = Assembly.GetExecutingAssembly();
        AssemblyName[] refs = asm.GetReferencedAssemblies();
        Console.WriteLine($"  Referenced assemblies: {refs.Length}");
        foreach (AssemblyName r in refs.Take(8))
            Console.WriteLine($"    → {r.Name} {r.Version}");
        Debug.Assert(refs.Any(r => r.Name is "System.Runtime" or "System.Private.CoreLib" or "netstandard"));
        Console.WriteLine("  Multi-module assemblies are rare in modern .NET; one module per DLL is the norm.");
    }
}
