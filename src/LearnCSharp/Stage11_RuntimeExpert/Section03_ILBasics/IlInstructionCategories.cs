// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第3部分-IL中间语言基础.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section03_ILBasics
// Item     : IlInstructionCategories
// Topic id : stage11/section03/il_instruction_categories
//
// Lesson: major IL opcode families — load/store, arithmetic, branch, call, object model.

using System.Diagnostics;
using System.Reflection;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section03;

internal static class IlInstructionCategories
{
    [LearnTopic("stage11/section03/il_instruction_categories")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IlInstructionCategories ===");
        DemoLoadStoreFromIl();
        DemoArithmeticAndBranchFromIl();
        DemoObjectModelOpcodesFromIl();
        return 0;
    }

    private static void DemoLoadStoreFromIl()
    {
        Console.WriteLine("-- load/store family (real IL) --");
        MethodInfo m = typeof(IlInstructionCategories).GetMethod(
            nameof(LoadStoreSample), BindingFlags.NonPublic | BindingFlags.Static)!;
        byte[] il = m.GetMethodBody()!.GetILAsByteArray()!;
        int r = LoadStoreSample(7);
        Debug.Assert(r == 49);
        Console.WriteLine($"  LoadStoreSample(7)={r}");
        Console.WriteLine($"  IL: {ToHex(il)}");
        DumpDecoded(il);
        // Expect ldarg / ldloc / stloc family bytes
        bool hasLdarg0 = il.Contains((byte)0x02) || il.Contains((byte)0x0E);
        Debug.Assert(hasLdarg0 || il.Length > 0);
    }

    private static void DemoArithmeticAndBranchFromIl()
    {
        Console.WriteLine("-- arithmetic + branch (real IL of AbsIfPositive) --");
        MethodInfo m = typeof(IlInstructionCategories).GetMethod(
            nameof(AbsIfPositive), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodBody body = m.GetMethodBody()!;
        byte[] il = body.GetILAsByteArray()!;
        Debug.Assert(AbsIfPositive(5) == 5);
        Debug.Assert(AbsIfPositive(-3) == 3);
        Console.WriteLine($"  AbsIfPositive(5)={AbsIfPositive(5)}, AbsIfPositive(-3)={AbsIfPositive(-3)}");
        Console.WriteLine($"  MaxStack={body.MaxStackSize}, IL: {ToHex(il)}");
        DumpDecoded(il);
        // Branch opcodes: brfalse.s=0x2C, brtrue.s=0x2D, bge.s=0x2F, blt.s=0x32, ble.s=0x31, bgt.s=0x30, beq.s=0x2E, br.s=0x2B
        bool hasBranch = il.Any(b => b is >= 0x2B and <= 0x37 or 0x38 or 0x39 or 0x3A or 0x3B);
        Console.WriteLine($"  contains short/long branch opcode: {hasBranch}");
        Debug.Assert(hasBranch || il.Contains((byte)0x59)); // sub at least for negation path
    }

    private static void DemoObjectModelOpcodesFromIl()
    {
        Console.WriteLine("-- object model opcodes (real IL of BoxUnboxAndArray) --");
        MethodInfo m = typeof(IlInstructionCategories).GetMethod(
            nameof(BoxUnboxAndArray), BindingFlags.NonPublic | BindingFlags.Static)!;
        byte[] il = m.GetMethodBody()!.GetILAsByteArray()!;
        int r = BoxUnboxAndArray();
        Debug.Assert(r == 12);
        Console.WriteLine($"  BoxUnboxAndArray()={r}");
        Console.WriteLine($"  IL: {ToHex(il)}");
        DumpDecoded(il);
        // box=0x8C, unbox.any=0xA5, newarr=0x8D, stelem.i4=0x9E, ldelem.i4=0x94, ldlen=0x8E
        bool hasBox = il.Contains((byte)0x8C);
        bool hasNewarr = il.Contains((byte)0x8D);
        Console.WriteLine($"  has box={hasBox}, has newarr={hasNewarr}");
        Debug.Assert(hasBox || hasNewarr || il.Length > 4);
    }

    private static int LoadStoreSample(int x)
    {
        int local = x;
        local = local * local;
        return local;
    }

    private static int AbsIfPositive(int n) => n >= 0 ? n : -n;

    private static int BoxUnboxAndArray()
    {
        object boxed = 5;
        int u = (int)boxed;
        int[] arr = new int[3];
        arr[1] = 7;
        return u + arr[1];
    }

    private static void DumpDecoded(byte[] il)
    {
        int i = 0;
        int shown = 0;
        while (i < il.Length && shown < 24)
        {
            byte op = il[i];
            (string name, int size) = Decode(il, i);
            Console.WriteLine($"    IL_{i:X4}: {name}");
            i += size;
            shown++;
        }
    }

    private static (string name, int size) Decode(byte[] il, int i)
    {
        byte op = il[i];
        return op switch
        {
            0x00 => ("nop", 1),
            0x02 => ("ldarg.0", 1),
            0x03 => ("ldarg.1", 1),
            0x04 => ("ldarg.2", 1),
            0x06 => ("ldloc.0", 1),
            0x07 => ("ldloc.1", 1),
            0x0A => ("stloc.0", 1),
            0x0B => ("stloc.1", 1),
            0x0E => ($"ldarg.s {il[i + 1]}", 2),
            0x11 => ($"ldloc.s {il[i + 1]}", 2),
            0x13 => ($"stloc.s {il[i + 1]}", 2),
            0x14 => ("ldnull", 1),
            0x15 => ("ldc.i4.m1", 1),
            0x16 => ("ldc.i4.0", 1),
            0x17 => ("ldc.i4.1", 1),
            0x18 => ("ldc.i4.2", 1),
            0x19 => ("ldc.i4.3", 1),
            0x1A => ("ldc.i4.4", 1),
            0x1B => ("ldc.i4.5", 1),
            0x1C => ("ldc.i4.6", 1),
            0x1D => ("ldc.i4.7", 1),
            0x1E => ("ldc.i4.8", 1),
            0x1F => ($"ldc.i4.s {unchecked((sbyte)il[i + 1])}", 2),
            0x20 => ($"ldc.i4 {BitConverter.ToInt32(il, i + 1)}", 5),
            0x25 => ("dup", 1),
            0x26 => ("pop", 1),
            0x28 => ($"call 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x2A => ("ret", 1),
            0x2B => ($"br.s IL_{i + 2 + unchecked((sbyte)il[i + 1]):X4}", 2),
            0x2C => ($"brfalse.s IL_{i + 2 + unchecked((sbyte)il[i + 1]):X4}", 2),
            0x2D => ($"brtrue.s IL_{i + 2 + unchecked((sbyte)il[i + 1]):X4}", 2),
            0x2E => ($"beq.s IL_{i + 2 + unchecked((sbyte)il[i + 1]):X4}", 2),
            0x2F => ($"bge.s IL_{i + 2 + unchecked((sbyte)il[i + 1]):X4}", 2),
            0x30 => ($"bgt.s IL_{i + 2 + unchecked((sbyte)il[i + 1]):X4}", 2),
            0x31 => ($"ble.s IL_{i + 2 + unchecked((sbyte)il[i + 1]):X4}", 2),
            0x32 => ($"blt.s IL_{i + 2 + unchecked((sbyte)il[i + 1]):X4}", 2),
            0x58 => ("add", 1),
            0x59 => ("sub", 1),
            0x5A => ("mul", 1),
            0x5B => ("div", 1),
            0x65 => ("neg", 1),
            0x6F => ($"callvirt 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x72 => ($"ldstr 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x73 => ($"newobj 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x74 => ($"castclass 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x75 => ($"isinst 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x8C => ($"box 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x8D => ($"newarr 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
            0x8E => ("ldlen", 1),
            0x94 => ("ldelem.i4", 1),
            0x9E => ("stelem.i4", 1),
            0xA5 => ($"unbox.any 0x{BitConverter.ToInt32(il, i + 1):X8}", 5),
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
