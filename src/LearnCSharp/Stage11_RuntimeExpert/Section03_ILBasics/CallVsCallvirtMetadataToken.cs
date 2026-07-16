// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第3部分-IL中间语言基础.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section03_ILBasics
// Item     : CallVsCallvirtMetadataToken
// Topic id : stage11/section03/call_vs_callvirt_metadata_token
//
// Lesson: call (static/nonvirt) vs callvirt (virtual + null check); tokens name methods.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section03;

internal static class CallVsCallvirtMetadataToken
{
    [LearnTopic("stage11/section03/call_vs_callvirt_metadata_token")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CallVsCallvirtMetadataToken ===");
        DemoCallVsCallvirt();
        DemoNullCheckSemantics();
        DemoMetadataTokens();
        return 0;
    }

    private static void DemoCallVsCallvirt()
    {
        Console.WriteLine("-- call vs callvirt --");
        Console.WriteLine("  call: static methods, non-virtual, base calls, some constrained paths");
        Console.WriteLine("  callvirt: instance virtual & most C# instance calls (null-check)");
        int s = StaticAdd(2, 3); // call
        Base b = new Derived();
        string v = b.Name();     // callvirt virtual
        Debug.Assert(s == 5 && v == "derived");
        Console.WriteLine($"  StaticAdd={s}, virtual Name={v}");
    }

    private static void DemoNullCheckSemantics()
    {
        Console.WriteLine("-- callvirt null check even for non-virtual instance methods --");
        Helper? h = null;
        try
        {
            _ = h!.Tag();
            Debug.Fail("expected NRE");
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("  null.Tag() → NullReferenceException (callvirt null check)");
        }

        h = new Helper();
        Debug.Assert(h.Tag() == "ok");
    }

    private static void DemoMetadataTokens()
    {
        Console.WriteLine("-- call operand is a metadata token --");
        MethodInfo mStatic = typeof(CallVsCallvirtMetadataToken).GetMethod(nameof(StaticAdd), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodInfo mVirt = typeof(Base).GetMethod(nameof(Base.Name))!;
        Console.WriteLine($"  StaticAdd token=0x{mStatic.MetadataToken:X8}");
        Console.WriteLine($"  Base.Name token=0x{mVirt.MetadataToken:X8}");
        Debug.Assert(mStatic.MetadataToken != 0 && mVirt.MetadataToken != 0);
        Console.WriteLine("  JIT resolves token → MethodDesc / entry point / vtable slot.");
    }

    private static int StaticAdd(int a, int b) => a + b;

    private class Base
    {
        public virtual string Name() => "base";
    }

    private sealed class Derived : Base
    {
        public override string Name() => "derived";
    }

    private sealed class Helper
    {
        public string Tag() => "ok";
    }
}
