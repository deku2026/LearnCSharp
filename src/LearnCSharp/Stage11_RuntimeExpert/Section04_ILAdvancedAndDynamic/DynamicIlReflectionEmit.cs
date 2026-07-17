// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第4部分-IL高级与动态生成.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section04_ILAdvancedAndDynamic
// Item     : DynamicIlReflectionEmit
// Topic id : stage11/section04/dynamic_il_reflection_emit
//
// Lesson: DynamicMethod / ILGenerator emit IL at runtime and invoke.

using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section04;

internal static class DynamicIlReflectionEmit
{
    [LearnTopic("stage11/section04/dynamic_il_reflection_emit")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DynamicIlReflectionEmit ===");
        DemoDynamicAdd();
        DemoDynamicMulByConstant();
        DemoPersistedAssemblyBuilder();
        return 0;
    }

    private static void DemoDynamicAdd()
    {
        Console.WriteLine("-- DynamicMethod: int Add(int,int) --");
        DynamicMethod dm = new DynamicMethod(
            name: "Add",
            returnType: typeof(int),
            parameterTypes: [typeof(int), typeof(int)],
            m: typeof(DynamicIlReflectionEmit).Module,
            skipVisibility: true);

        ILGenerator il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Ret);

        Func<int, int, int> add = dm.CreateDelegate<Func<int, int, int>>();
        int r = add(20, 22);
        Debug.Assert(r == 42);
        Console.WriteLine($"  emitted Add(20,22)={r}");
    }

    private static void DemoDynamicMulByConstant()
    {
        Console.WriteLine("-- DynamicMethod: multiply by 3 with locals --");
        DynamicMethod dm = new DynamicMethod("Mul3", typeof(int), [typeof(int)], typeof(DynamicIlReflectionEmit).Module, true);
        ILGenerator il = dm.GetILGenerator();
        LocalBuilder local = il.DeclareLocal(typeof(int));
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4_3);
        il.Emit(OpCodes.Mul);
        il.Emit(OpCodes.Stloc, local);
        il.Emit(OpCodes.Ldloc, local);
        il.Emit(OpCodes.Ret);

        Func<int, int> mul3 = dm.CreateDelegate<Func<int, int>>();
        Debug.Assert(mul3(7) == 21);
        Console.WriteLine($"  Mul3(7)={mul3(7)}");
        Console.WriteLine("  Use cases: serializers, expression compilers, mappers — emit once, call many.");
    }

    // The doc's ⭐ .NET 9 feature: PersistedAssemblyBuilder can save emitted IL to a PE
    // stream (a real .dll) — the thing DynamicMethod cannot do. Also observe the AOT
    // limitation: under AOT, dynamic IL is unavailable (RuntimeFeature.IsDynamicCodeCompiled=false).
    private static void DemoPersistedAssemblyBuilder()
    {
        Console.WriteLine("-- .NET 9 PersistedAssemblyBuilder: emit a saveable assembly --");
        AssemblyName an = new AssemblyName("EmittedLib");
        PersistedAssemblyBuilder ab = new PersistedAssemblyBuilder(an, typeof(object).Assembly, null);
        ModuleBuilder mod = ab.DefineDynamicModule("EmittedLib.dll");
        TypeBuilder tb = mod.DefineType("Calc", TypeAttributes.Public | TypeAttributes.Class);
        MethodBuilder mb = tb.DefineMethod("Inc", MethodAttributes.Public | MethodAttributes.Static,
            typeof(int), [typeof(int)]);
        ILGenerator il = mb.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Ret);
        Type calc = tb.CreateType();
        Console.WriteLine($"  emitted type {calc.FullName} with static Inc(int)");

        // Persist to a PE stream (MemoryStream) — the headline capability that
        // DynamicMethod cannot do. (Invoking the method in-memory from a persisted
        // assembly is restricted; the in-memory call path is covered by DemoDynamicAdd.)
        using MemoryStream pe = new MemoryStream();
        ab.Save(pe);
        long peLen = pe.Length;
        Console.WriteLine($"  Save() → PE stream length={peLen} bytes (non-zero: IL persisted to a real .dll)");
        Debug.Assert(peLen > 0, "persisted assembly PE stream must be non-empty");

        // AOT observability: [RequiresDynamicCode]/IL3050 → false under AOT.
        bool dynamicAvailable = RuntimeFeature.IsDynamicCodeCompiled;
        Console.WriteLine($"  RuntimeFeature.IsDynamicCodeCompiled={dynamicAvailable} (false under AOT → Reflection.Emit unavailable)");
        Console.WriteLine("  DynamicMethod/PersistedAssemblyBuilder require dynamic code → annotate [RequiresDynamicCode] / IL3050 for AOT.");
    }
}
