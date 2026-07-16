// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第4部分-IL高级与动态生成.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section04_ILAdvancedAndDynamic
// Item     : DynamicIlReflectionEmit
// Topic id : stage11/section04/dynamic_il_reflection_emit
//
// Lesson: DynamicMethod / ILGenerator emit IL at runtime and invoke.

using System.Diagnostics;
using System.Reflection.Emit;
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
        return 0;
    }

    private static void DemoDynamicAdd()
    {
        Console.WriteLine("-- DynamicMethod: int Add(int,int) --");
        var dm = new DynamicMethod(
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

        var add = dm.CreateDelegate<Func<int, int, int>>();
        int r = add(20, 22);
        Debug.Assert(r == 42);
        Console.WriteLine($"  emitted Add(20,22)={r}");
    }

    private static void DemoDynamicMulByConstant()
    {
        Console.WriteLine("-- DynamicMethod: multiply by 3 with locals --");
        var dm = new DynamicMethod("Mul3", typeof(int), [typeof(int)], typeof(DynamicIlReflectionEmit).Module, true);
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
}
