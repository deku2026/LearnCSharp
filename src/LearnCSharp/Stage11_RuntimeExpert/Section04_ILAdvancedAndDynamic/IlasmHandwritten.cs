// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第4部分-IL高级与动态生成.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section04_ILAdvancedAndDynamic
// Item     : IlasmHandwritten
// Topic id : stage11/section04/ilasm_handwritten
//
// Lesson: handwritten IL concepts without requiring ilasm tool — emit equivalent via DynamicMethod.

using System.Diagnostics;
using System.Reflection.Emit;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section04;

internal static class IlasmHandwritten
{
    [LearnTopic("stage11/section04/ilasm_handwritten")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IlasmHandwritten ===");
        DemoShowIlasmText();
        DemoEmitEquivalent();
        DemoWhenToHandwrite();
        return 0;
    }

    private static void DemoShowIlasmText()
    {
        Console.WriteLine("-- sample .il fragment (educational text) --");
        Console.WriteLine("""
          .method public static int32 Max(int32 a, int32 b) cil managed
          {
            .maxstack 2
            ldarg.0
            ldarg.1
            ble.s      use_b
            ldarg.0
            ret
          use_b:
            ldarg.1
            ret
          }
          """);
        Console.WriteLine("  ilasm Max.il /output:Max.dll  → then reference or reflection-load");
    }

    private static void DemoEmitEquivalent()
    {
        Console.WriteLine("-- same Max via DynamicMethod (no ilasm dependency) --");
        DynamicMethod dm = new DynamicMethod("Max", typeof(int), [typeof(int), typeof(int)], typeof(IlasmHandwritten).Module, true);
        ILGenerator il = dm.GetILGenerator();
        Label useB = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ble_S, useB);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ret);
        il.MarkLabel(useB);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ret);

        Func<int, int, int> max = dm.CreateDelegate<Func<int, int, int>>();
        Debug.Assert(max(3, 8) == 8);
        Debug.Assert(max(9, 2) == 9);
        Console.WriteLine($"  Max(3,8)={max(3, 8)}, Max(9,2)={max(9, 2)}");
    }

    private static void DemoWhenToHandwrite()
    {
        Console.WriteLine("-- when handwritten IL helps --");
        Console.WriteLine("  Teaching verifiable CIL, crafting edge-case EH, research.");
        Console.WriteLine("  Production: prefer C# / Expression / source generators.");
        Console.WriteLine("  Invalid IL fails at create/JIT with VerificationException / InvalidProgramException.");
    }
}
