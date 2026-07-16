// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第3部分-IL中间语言基础.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section03_ILBasics
// Item     : EvaluationStackModel
// Topic id : stage11/section03/evaluation_stack_model
//
// Lesson: IL ops push/pop evaluation stack; maxstack verified; locals separate.

using System.Diagnostics;
using System.Reflection;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section03;

internal static class EvaluationStackModel
{
    [LearnTopic("stage11/section03/evaluation_stack_model")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== EvaluationStackModel ===");
        DemoRealMaxStackAndIl();
        DemoLocalsVsStack();
        DemoDupAndPopPatterns();
        return 0;
    }

    /// <summary>
    /// Dump MethodBody.GetILAsByteArray for a known method and report MaxStackSize.
    /// Sample IL for MulAdd(a,b,c) ≈ (a*b)+c: ldarg.0 ldarg.1 mul ldarg.2 add ret
    /// </summary>
    private static void DemoRealMaxStackAndIl()
    {
        Console.WriteLine("-- real MethodBody: MaxStack + IL bytes --");
        MethodInfo m = typeof(EvaluationStackModel).GetMethod(
            nameof(MulAdd), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodBody body = m.GetMethodBody() ?? throw new InvalidOperationException("no body");
        byte[] il = body.GetILAsByteArray() ?? throw new InvalidOperationException("no IL");

        Console.WriteLine($"  MulAdd(a,b,c) => (a*b)+c; result check: {MulAdd(3, 4, 5)}");
        Debug.Assert(MulAdd(3, 4, 5) == 17);
        Console.WriteLine($"  MaxStackSize={body.MaxStackSize} (verifier limit for evaluation stack depth)");
        Console.WriteLine($"  LocalVariables.Count={body.LocalVariables.Count}");
        Console.WriteLine($"  IL length={il.Length}: {ToHex(il)}");
        Debug.Assert(body.MaxStackSize >= 2, "mul needs at least 2 stack slots");

        // Manual decode of short form opcodes (no operand)
        Console.WriteLine("  decoded (short ops):");
        int i = 0;
        while (i < il.Length)
        {
            byte op = il[i];
            string name = op switch
            {
                0x02 => "ldarg.0",
                0x03 => "ldarg.1",
                0x04 => "ldarg.2",
                0x05 => "ldarg.3",
                0x16 => "ldc.i4.0",
                0x17 => "ldc.i4.1",
                0x18 => "ldc.i4.2",
                0x19 => "ldc.i4.3",
                0x1A => "ldc.i4.4",
                0x1B => "ldc.i4.5",
                0x58 => "add",
                0x59 => "sub",
                0x5A => "mul",
                0x5B => "div",
                0x2A => "ret",
                0x00 => "nop",
                _ => $"op_0x{op:X2}"
            };
            Console.WriteLine($"    IL_{i:X4}: {name}");
            // multi-byte: not needed for this method's expected shape
            if (op is 0x0E or 0x0F or 0x10 or 0x11 or 0x12 or 0x13) // ldarg.s / starg.s / ldloc.s / stloc.s
                i += 2;
            else if (op is 0x1F) // ldc.i4.s
                i += 2;
            else if (op is 0x20) // ldc.i4
                i += 5;
            else if (op is 0x28 or 0x6F or 0x73) // call / callvirt / newobj + token
                i += 5;
            else
                i += 1;
        }

        Debug.Assert(il.Contains((byte)0x5A) || body.MaxStackSize >= 2); // mul or stack depth
    }

    private static void DemoLocalsVsStack()
    {
        Console.WriteLine("-- locals (stloc/ldloc) vs pure stack --");
        MethodInfo m = typeof(EvaluationStackModel).GetMethod(
            nameof(WithLocal), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodBody body = m.GetMethodBody()!;
        byte[] il = body.GetILAsByteArray()!;
        int r = WithLocal(5);
        Debug.Assert(r == 12);
        Console.WriteLine($"  WithLocal(5)={r}, MaxStack={body.MaxStackSize}, locals={body.LocalVariables.Count}");
        Console.WriteLine($"  IL: {ToHex(il)}");
        Debug.Assert(body.LocalVariables.Count >= 1, "method uses an explicit local");
        Console.WriteLine("  Locals survive across statements; evaluation stack is transient.");
    }

    private static void DemoDupAndPopPatterns()
    {
        Console.WriteLine("-- field++ / discard patterns --");
        var box = new Counter();
        box.Value++;
        Debug.Assert(box.Value == 1);
        Console.WriteLine($"  Counter++ → {box.Value}");
        int discarded = Identity(99);
        Debug.Assert(discarded == 99);
        MethodInfo m = typeof(EvaluationStackModel).GetMethod(
            nameof(Identity), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodBody? body = m.GetMethodBody();
        Console.WriteLine($"  Identity MaxStack={body?.MaxStackSize}, IL={ToHex(body?.GetILAsByteArray() ?? [])}");
    }

    private static int MulAdd(int a, int b, int c) => (a * b) + c;

    private static int WithLocal(int x)
    {
        int local = x;
        local = local + 7;
        return local;
    }

    private static int Identity(int x) => x;

    private static string ToHex(byte[] il)
    {
        var sb = new StringBuilder(il.Length * 3);
        for (int i = 0; i < il.Length; i++)
        {
            if (i > 0) sb.Append(' ');
            sb.Append(il[i].ToString("X2"));
        }

        return sb.ToString();
    }

    private sealed class Counter
    {
        public int Value;
    }
}
