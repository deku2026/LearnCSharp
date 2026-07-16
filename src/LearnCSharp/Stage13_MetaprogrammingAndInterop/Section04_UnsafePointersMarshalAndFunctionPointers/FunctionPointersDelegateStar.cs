// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第4部分-unsafe指针Marshal函数指针.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section04_UnsafePointersMarshalAndFunctionPointers
// Item     : FunctionPointersDelegateStar
// Topic id : stage13/section04/function_pointers_delegate_star
//
// Lesson: delegate* = raw code address; vs delegate + keep-alive trap.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section04;

internal static class FunctionPointersDelegateStar
{
    // Keep callback alive when marshalled to native (classic trap).
    private static NativeCallback? s_keepAlive;

    private delegate void NativeCallback(int code);

    [LearnTopic("stage13/section04/function_pointers_delegate_star")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== FunctionPointersDelegateStar ===");
        DemoManagedFunctionPointer();
        DemoDelegateBridgeAndKeepAlive();
        DemoUnmanagedCallersOnlyConcept();
        return 0;
    }

    private static unsafe void DemoManagedFunctionPointer()
    {
        Console.WriteLine("-- managed function pointer (no delegate object) --");
        delegate*<int, int> fp = &Square;
        int r = fp(5);
        Debug.Assert(r == 25);
        Console.WriteLine($"  delegate*<int,int> fp = &Square; fp(5) => {r}");
        Console.WriteLine("  No allocation, no indirection through MulticastDelegate.");
    }

    private static void DemoDelegateBridgeAndKeepAlive()
    {
        Console.WriteLine("-- delegate ↔ function pointer + keep-alive --");
        // Marshal requires a non-generic delegate type
        NativeCallback del = static code => Console.WriteLine($"  callback code={code}");
        s_keepAlive = del; // prevent GC reclaim while native would hold the pointer

        nint fp = Marshal.GetFunctionPointerForDelegate(del);
        NativeCallback back = Marshal.GetDelegateForFunctionPointer<NativeCallback>(fp);
        back(7);
        Debug.Assert(fp != 0);
        Console.WriteLine($"  GetFunctionPointerForDelegate => 0x{fp:X}");
        Console.WriteLine("  Without a root (field/GCHandle), GC may collect del → AV on native call.");
        Console.WriteLine("  &static + [UnmanagedCallersOnly] has fixed address (no keep-alive needed).");
        s_keepAlive = null;
    }

    private static unsafe void DemoUnmanagedCallersOnlyConcept()
    {
        Console.WriteLine("-- UnmanagedCallersOnly (native-callable static) --");
        // Take address of unmanaged-callable method for educational purposes.
        // Note: cannot call OnNativeEvent from managed code directly.
        delegate* unmanaged[Cdecl]<int, void> nativeFp = &OnNativeEvent;
        Debug.Assert(nativeFp != null);
        Console.WriteLine($"  &OnNativeEvent as unmanaged[Cdecl] => non-null function pointer");
        Console.WriteLine("  Use for C callbacks: register_cb(&OnNativeEvent);");
        Console.WriteLine("  Trade-off: fast/static only; need instance state → use delegate instead.");
    }

    private static int Square(int x) => x * x;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnNativeEvent(int code)
    {
        // Would be invoked only from native code.
        _ = code;
    }
}
