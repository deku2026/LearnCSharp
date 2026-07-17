// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第3部分-IL中间语言基础.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section03_ILBasics
// Item     : IlToolingAndWalkthrough
// Topic id : stage11/section03/il_tooling_and_walkthrough
//
// Lesson: tools for IL — ildasm/ilspycmd/SharpLab; walk a tiny method's body.

using System.Diagnostics;
using System.Reflection;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section03;

internal static class IlToolingAndWalkthrough
{
    [LearnTopic("stage11/section03/il_tooling_and_walkthrough")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IlToolingAndWalkthrough ===");
        DemoToolingList();
        DemoWalkthroughRealIl();
        DemoCallOpcode();
        return 0;
    }

    private static void DemoToolingList()
    {
        Console.WriteLine("-- IL tooling (external; this demo uses Reflection in-process) --");
        Console.WriteLine("  ildasm / ilasm, ILSpy / ilspycmd, SharpLab.io");
        Console.WriteLine("  MethodBody.GetILAsByteArray + manual decode (below)");
        Console.WriteLine("  System.Reflection.Metadata for PE tables");
    }

    private static void DemoWalkthroughRealIl()
    {
        Console.WriteLine("-- walkthrough: AbsDiff real IL --");
        int r1 = AbsDiff(10, 3);
        int r2 = AbsDiff(3, 10);
        Debug.Assert(r1 == 7 && r2 == 7);

        MethodInfo m = typeof(IlToolingAndWalkthrough).GetMethod(
            nameof(AbsDiff), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodBody body = m.GetMethodBody()!;
        byte[] il = body.GetILAsByteArray()!;
        Console.WriteLine($"  AbsDiff(10,3)={r1}, AbsDiff(3,10)={r2}");
        Console.WriteLine($"  MaxStackSize={body.MaxStackSize}, InitLocals={body.InitLocals}");
        Console.WriteLine($"  Local count={body.LocalVariables.Count}, IL len={il.Length}");
        Console.WriteLine($"  raw: {ToHex(il)}");
        Debug.Assert(body.MaxStackSize >= 2);
        Debug.Assert(il.Length > 0);

        Console.WriteLine("  decoded:");
        int i = 0;
        while (i < il.Length)
        {
            (string name, int size) = DecodeOp(il, i);
            Console.WriteLine($"    IL_{i:X4}: {name}");
            i += size;
        }

        // Expect ldarg and a branch or conditional compare
        Debug.Assert(il.Contains((byte)0x02) || il.Contains((byte)0x03), "expect ldarg.0/1");
    }

    private static void DemoCallOpcode()
    {
        Console.WriteLine("-- call opcode (0x28) in CallerOfAbs --");
        MethodInfo m = typeof(IlToolingAndWalkthrough).GetMethod(
            nameof(CallerOfAbs), BindingFlags.NonPublic | BindingFlags.Static)!;
        byte[] il = m.GetMethodBody()!.GetILAsByteArray()!;
        int v = CallerOfAbs(8, 3);
        Debug.Assert(v == 5);
        Console.WriteLine($"  CallerOfAbs(8,3)={v}");
        Console.WriteLine($"  IL: {ToHex(il)}");
        bool hasCall = il.Contains((byte)0x28);
        Console.WriteLine($"  contains call (0x28): {hasCall}");
        Debug.Assert(hasCall, "CallerOfAbs should emit call AbsDiff");
        int idx = Array.IndexOf(il, (byte)0x28);
        if (idx >= 0 && idx + 4 < il.Length)
        {
            int token = BitConverter.ToInt32(il, idx + 1);
            Console.WriteLine($"  call metadata token=0x{token:X8}");
        }
    }

    private static int AbsDiff(int a, int b) => a > b ? a - b : b - a;

    private static int CallerOfAbs(int a, int b) => AbsDiff(a, b);

    private static (string name, int size) DecodeOp(byte[] il, int i)
    {
        byte op = il[i];
        return op switch
        {
            0x00 => ("nop", 1),
            0x02 => ("ldarg.0", 1),
            0x03 => ("ldarg.1", 1),
            0x04 => ("ldarg.2", 1),
            0x06 => ("ldloc.0", 1),
            0x0A => ("stloc.0", 1),
            0x16 => ("ldc.i4.0", 1),
            0x17 => ("ldc.i4.1", 1),
            0x25 => ("dup", 1),
            0x28 => ($"call token=0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x2A => ("ret", 1),
            0x2B => ($"br.s +{unchecked((sbyte)il[i + 1])}", 2),
            0x2C => ($"brfalse.s +{unchecked((sbyte)il[i + 1])}", 2),
            0x2D => ($"brtrue.s +{unchecked((sbyte)il[i + 1])}", 2),
            0x2F => ($"bge.s +{unchecked((sbyte)il[i + 1])}", 2),
            0x30 => ($"bgt.s +{unchecked((sbyte)il[i + 1])}", 2),
            0x31 => ($"ble.s +{unchecked((sbyte)il[i + 1])}", 2),
            0x32 => ($"blt.s +{unchecked((sbyte)il[i + 1])}", 2),
            0x58 => ("add", 1),
            0x59 => ("sub", 1),
            0x5A => ("mul", 1),
            0xFE when i + 1 < il.Length && il[i + 1] == 0x01 => ("ceq", 2),
            0xFE when i + 1 < il.Length && il[i + 1] == 0x02 => ("cgt", 2),
            0xFE when i + 1 < il.Length && il[i + 1] == 0x04 => ("clt", 2),
            _ => ($"op_0x{op:X2}", 1)
        };
    }

    private static string ToHex(byte[] il)
    {
        StringBuilder sb = new StringBuilder(il.Length * 3);
        for (int i = 0; i < il.Length; i++)
        {
            if (i > 0) sb.Append(' ');
            sb.Append(il[i].ToString("X2"));
        }

        return sb.ToString();
    }
}
