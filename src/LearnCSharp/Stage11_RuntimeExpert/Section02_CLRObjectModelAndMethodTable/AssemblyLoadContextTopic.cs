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
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
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
        DemoCollectibleLoadUnload();
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
        Debug.Assert(!def.IsCollectible);
        Console.WriteLine($"  Assemblies in default (count): {def.Assemblies.Count()}");
    }

    /// <summary>
    /// Emit a tiny PE with PersistedAssemblyBuilder, load into a collectible ALC,
    /// invoke a static method, drop roots, Unload, assert WeakReference.IsAlive is false.
    /// </summary>
    private static void DemoCollectibleLoadUnload()
    {
        Console.WriteLine("-- collectible ALC: persist PE → LoadFromStream → invoke → unload → GC --");
        WeakReference weakAlc;
        WeakReference weakAsm;
        int pluginResult = RunPluginInCollectible(out weakAlc, out weakAsm);

        Debug.Assert(pluginResult == 42);
        Console.WriteLine($"  Plugin Adder.Add(20,22)={pluginResult}");

        for (int i = 0; i < 10 && (weakAlc.IsAlive || weakAsm.IsAlive); i++)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
        }

        Console.WriteLine($"  After Unload+GC: ALC.IsAlive={weakAlc.IsAlive}, Assembly.IsAlive={weakAsm.IsAlive}");
        Debug.Assert(!weakAlc.IsAlive, "collectible ALC should be reclaimed after Unload + GC");
        Debug.Assert(!weakAsm.IsAlive, "collectible assembly should be reclaimed after Unload + GC");
        Console.WriteLine("  Plugin pattern: LoadFromStream/path into collectible ALC, drop roots, Unload.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int RunPluginInCollectible(out WeakReference weakAlc, out WeakReference weakAsm)
    {
        byte[] pe = BuildPluginPe();
        var alc = new AssemblyLoadContext("plugin-demo-" + Guid.NewGuid().ToString("N")[..8], isCollectible: true);
        weakAlc = new WeakReference(alc, trackResurrection: false);

        using var ms = new MemoryStream(pe, writable: false);
        Assembly asm = alc.LoadFromStream(ms);
        weakAsm = new WeakReference(asm, trackResurrection: false);

        Debug.Assert(alc.IsCollectible);
        Debug.Assert(ReferenceEquals(AssemblyLoadContext.GetLoadContext(asm), alc));

        Type pluginType = asm.GetType("LearnCSharp.Plugin.Adder", throwOnError: true)!;
        MethodInfo add = pluginType.GetMethod("Add", BindingFlags.Public | BindingFlags.Static)!;
        int result = (int)add.Invoke(null, [20, 22])!;
        Console.WriteLine($"  Host ALC.IsCollectible={alc.IsCollectible}, Name={alc.Name}");

        // Drop all roots into the plugin, then unload.
        pluginType = null!;
        add = null!;
        asm = null!;
        alc.Unload();
        return result;
    }

    private static byte[] BuildPluginPe()
    {
        // .NET 9+ PersistedAssemblyBuilder: emit a real PE we can LoadFromStream.
        var ab = new PersistedAssemblyBuilder(
            new AssemblyName("LearnCSharp.Plugin." + Guid.NewGuid().ToString("N")[..8]),
            typeof(object).Assembly);

        ModuleBuilder mb = ab.DefineDynamicModule("Main");
        TypeBuilder tb = mb.DefineType(
            "LearnCSharp.Plugin.Adder",
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Class);

        MethodBuilder add = tb.DefineMethod(
            "Add",
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            typeof(int),
            [typeof(int), typeof(int)]);
        ILGenerator il = add.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Ret);
        tb.CreateType();

        using var pe = new MemoryStream();
        ab.Save(pe);
        return pe.ToArray();
    }

    private static void DemoTypeIdentityAcrossContexts()
    {
        Console.WriteLine("-- type identity is ALC-scoped --");
        Type t1 = LoadNamedPluginType("IdA");
        Type t2 = LoadNamedPluginType("IdB");
        Console.WriteLine($"  FullName equal: {t1.FullName == t2.FullName}");
        Console.WriteLine($"  ReferenceEquals(Type): {ReferenceEquals(t1, t2)}");
        Debug.Assert(t1.FullName == t2.FullName);
        Debug.Assert(!ReferenceEquals(t1, t2));
        Debug.Assert(t1.Assembly != t2.Assembly);
        Console.WriteLine("  Same name in two ALCs ⇒ two Types; casts across ALCs fail.");

        UnloadIfCollectible(t1.Assembly);
        UnloadIfCollectible(t2.Assembly);
        ForceCollect();
    }

    private static Type LoadNamedPluginType(string suffix)
    {
        byte[] pe = BuildNamedTypePe(suffix);
        var alc = new AssemblyLoadContext("id-" + suffix, isCollectible: true);
        using var ms = new MemoryStream(pe, writable: false);
        Assembly asm = alc.LoadFromStream(ms);
        return asm.GetType("Shared.Name.PluginType", throwOnError: true)!;
    }

    private static byte[] BuildNamedTypePe(string suffix)
    {
        var ab = new PersistedAssemblyBuilder(
            new AssemblyName("IdDemo." + suffix),
            typeof(object).Assembly);
        ModuleBuilder mb = ab.DefineDynamicModule("M");
        TypeBuilder tb = mb.DefineType("Shared.Name.PluginType", TypeAttributes.Public);
        tb.CreateType();
        using var pe = new MemoryStream();
        ab.Save(pe);
        return pe.ToArray();
    }

    private static void UnloadIfCollectible(Assembly asm)
    {
        AssemblyLoadContext? ctx = AssemblyLoadContext.GetLoadContext(asm);
        if (ctx is { IsCollectible: true })
            ctx.Unload();
    }

    private static void ForceCollect()
    {
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
    }
}
