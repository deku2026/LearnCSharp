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
        DemoWalkthrough();
        DemoCompareReleaseDebugNote();
        return 0;
    }

    private static void DemoToolingList()
    {
        Console.WriteLine("-- IL tooling (not run here; no external tools required) --");
        Console.WriteLine("  ildasm / ilasm          — classic disassembler/assembler");
        Console.WriteLine("  ILSpy / ilspycmd        — modern decompiler");
        Console.WriteLine("  SharpLab.io             — C# ↔ IL in browser");
        Console.WriteLine("  dotnet tool: ilspycmd, ildasm packages");
        Console.WriteLine("  System.Reflection.Metadata — read PE tables programmatically");
    }

    private static void DemoWalkthrough()
    {
        Console.WriteLine("-- walkthrough: AbsDiff IL shape --");
        Console.WriteLine("  int AbsDiff(int a,int b) => a>b ? a-b : b-a;");
        Console.WriteLine("  Rough IL:");
        Console.WriteLine("    ldarg.0 / ldarg.1 / ble.s else");
        Console.WriteLine("    ldarg.0 / ldarg.1 / sub / ret");
        Console.WriteLine("  else:");
        Console.WriteLine("    ldarg.1 / ldarg.0 / sub / ret");
        int r1 = AbsDiff(10, 3);
        int r2 = AbsDiff(3, 10);
        Debug.Assert(r1 == 7 && r2 == 7);
        MethodInfo m = typeof(IlToolingAndWalkthrough).GetMethod(nameof(AbsDiff), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodBody? body = m.GetMethodBody();
        byte[]? il = body?.GetILAsByteArray();
        Console.WriteLine($"  AbsDiff(10,3)={r1}, AbsDiff(3,10)={r2}, IL len={il?.Length}");
        if (il is not null)
            Console.WriteLine($"  raw: {ToHex(il)}");
    }

    private static void DemoCompareReleaseDebugNote()
    {
        Console.WriteLine("-- Debug vs Release IL --");
        Console.WriteLine("  Debug: more locals, nop, sequence points for PDB.");
        Console.WriteLine("  Release: tighter IL; JIT still does the heavy optimization.");
        Console.WriteLine($"  This process bitness={IntPtr.Size * 8}-bit, build config via debugger attached={Debugger.IsAttached}");
    }

    private static int AbsDiff(int a, int b) => a > b ? a - b : b - a;

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
}
