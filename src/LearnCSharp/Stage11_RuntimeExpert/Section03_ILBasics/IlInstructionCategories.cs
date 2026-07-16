// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第3部分-IL中间语言基础.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section03_ILBasics
// Item     : IlInstructionCategories
// Topic id : stage11/section03/il_instruction_categories
//
// Lesson: major IL opcode families — load/store, arithmetic, branch, call, object model.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section03;

internal static class IlInstructionCategories
{
    [LearnTopic("stage11/section03/il_instruction_categories")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IlInstructionCategories ===");
        DemoLoadStore();
        DemoArithmeticAndBranch();
        DemoObjectModelOpcodes();
        return 0;
    }

    private static void DemoLoadStore()
    {
        Console.WriteLine("-- load/store family --");
        Console.WriteLine("  ldarg / starg, ldloc / stloc, ldfld / stfld, ldsfld / stsfld");
        Console.WriteLine("  ldc.i4.*, ldstr, ldnull, ldtoken");
        int x = 42;
        string s = "hi";
        Debug.Assert(x == 42 && s.Length == 2);
        Console.WriteLine($"  constants + locals: x={x}, s={s}");
    }

    private static void DemoArithmeticAndBranch()
    {
        Console.WriteLine("-- arithmetic + control flow --");
        Console.WriteLine("  add/sub/mul/div/rem, and/or/xor/not, shl/shr");
        Console.WriteLine("  br, brtrue, brfalse, beq, blt, switch, leave (EH)");
        int n = 0;
        for (int i = 0; i < 5; i++)
            n += i;
        Debug.Assert(n == 10);
        string branch = n > 0 ? "positive" : "non-positive";
        Console.WriteLine($"  sum 0..4={n}, branch → {branch}");
    }

    private static void DemoObjectModelOpcodes()
    {
        Console.WriteLine("-- object model opcodes --");
        Console.WriteLine("  newobj, newarr, box, unbox.any, castclass, isinst");
        Console.WriteLine("  ldelem.*, stelem.*, ldlen, ldvirtftn");
        object o = "text";
        string? asStr = o as string; // isinst
        Debug.Assert(asStr == "text");
        int[] arr = new int[3]; // newarr
        arr[1] = 7;             // stelem
        Debug.Assert(arr.Length == 3 && arr[1] == 7);
        object boxed = 5;       // box
        int u = (int)boxed;     // unbox.any
        Debug.Assert(u == 5);
        Console.WriteLine($"  isinst/newarr/box ok: '{asStr}', arr[1]={arr[1]}, unbox={u}");
    }
}
