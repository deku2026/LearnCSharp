// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第3部分-IL中间语言基础.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section03_ILBasics
// Item     : Ecma335Overview
// Topic id : stage11/section03/ecma_335_overview
//
// Lesson: ECMA-335 defines CLI: PE + CLI header + metadata + IL; CIL is stack machine.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section03;

internal static class Ecma335Overview
{
    [LearnTopic("stage11/section03/ecma_335_overview")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Ecma335Overview ===");
        DemoCliLayers();
        DemoIlIsStackBased();
        DemoMethodBodyApi();
        return 0;
    }

    private static void DemoCliLayers()
    {
        Console.WriteLine("-- ECMA-335 CLI layers --");
        Console.WriteLine("  Partition I: architecture / type system");
        Console.WriteLine("  Partition II: metadata");
        Console.WriteLine("  Partition III: CIL instruction set");
        Console.WriteLine("  Partition VI: assembly formats (PE)");
        Assembly asm = Assembly.GetExecutingAssembly();
        Console.WriteLine($"  This module ImageRuntimeVersion={asm.ImageRuntimeVersion}");
        Debug.Assert(!string.IsNullOrEmpty(asm.ImageRuntimeVersion));
    }

    private static void DemoIlIsStackBased()
    {
        Console.WriteLine("-- CIL evaluation stack (not x86 registers) --");
        // Conceptual IL for c = a + b:
        // ldarg.0 / ldarg.1 / add / stloc.0
        int a = 10, b = 32;
        int c = a + b;
        Debug.Assert(c == 42);
        Console.WriteLine($"  C# a+b={c} ≈ IL: load args → add → store local");
        Console.WriteLine("  JIT maps stack ops onto real CPU registers/stack.");
    }

    private static void DemoMethodBodyApi()
    {
        Console.WriteLine("-- MethodBody reflection (IL bytes when available) --");
        MethodInfo m = typeof(Ecma335Overview).GetMethod(nameof(TinyAdd), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodBody? body = m.GetMethodBody();
        if (body is null)
        {
            Console.WriteLine("  MethodBody unavailable (AOT/dynamic) — skip byte dump");
            return;
        }

        byte[]? il = body.GetILAsByteArray();
        Console.WriteLine($"  TinyAdd MaxStack={body.MaxStackSize}, Locals={body.LocalVariables.Count}, IL bytes={il?.Length}");
        if (il is { Length: > 0 })
        {
            string hex = string.Join(' ', il.Take(16).Select(b => b.ToString("X2")));
            Console.WriteLine($"  IL head: {hex}{(il.Length > 16 ? " ..." : "")}");
        }

        Debug.Assert(body.MaxStackSize >= 1);
        Debug.Assert(TinyAdd(1, 2) == 3);
    }

    private static int TinyAdd(int x, int y) => x + y;
}
