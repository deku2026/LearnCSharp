// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第3部分-PInvoke与原生互操作.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section03_PInvokeAndInterop
// Item     : PInvokeIntro
// Topic id : stage13/section03/pinvoke_intro
//
// Lesson: P/Invoke = managed calls into native C ABI; CLR finds lib, marshals, switches.

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section03;

internal static partial class PInvokeIntro
{
    [LearnTopic("stage13/section03/pinvoke_intro")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== PInvokeIntro ===");
        DemoConcept();
        DemoWellKnownOsApi();
        DemoNativeLibraryResolve();
        return 0;
    }

    private static void DemoConcept()
    {
        Console.WriteLine("-- what P/Invoke does --");
        Console.WriteLine("  C#:  [DllImport/LibraryImport] static partial/extern int Add(int,int);");
        Console.WriteLine("  C:   extern \"C\" int add(int a, int b);");
        Console.WriteLine("  CLR: locate lib → find entry → marshal args → native call → marshal return");
        Console.WriteLine("  Cross-platform: same attribute, different lib name (.dll / .so / .dylib).");
    }

    private static void DemoWellKnownOsApi()
    {
        Console.WriteLine("-- call well-known OS API (guarded) --");
        try
        {
            if (OperatingSystem.IsWindows())
            {
                uint pid = GetCurrentProcessId();
                Debug.Assert(pid != 0 && pid == (uint)Environment.ProcessId);
                Console.WriteLine($"  Windows GetCurrentProcessId => {pid}");
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())
            {
                int pid = GetPid();
                Debug.Assert(pid > 0 && pid == Environment.ProcessId);
                Console.WriteLine($"  POSIX getpid => {pid}");
            }
            else
            {
                Console.WriteLine($"  skip: OS={RuntimeInformation.OSDescription}");
            }
        }
        catch (DllNotFoundException ex)
        {
            Console.WriteLine($"  DllNotFound: {ex.Message}");
        }
        catch (EntryPointNotFoundException ex)
        {
            Console.WriteLine($"  EntryPointNotFound: {ex.Message}");
        }
    }

    private static void DemoNativeLibraryResolve()
    {
        Console.WriteLine("-- NativeLibrary (runtime load) --");
        string? name = OperatingSystem.IsWindows() ? "kernel32"
            : OperatingSystem.IsMacOS() ? "libSystem.B.dylib"
            : OperatingSystem.IsLinux() ? "libc.so.6"
            : null;

        if (name is null)
        {
            Console.WriteLine("  (no demo library for this OS)");
            return;
        }

        try
        {
            if (NativeLibrary.TryLoad(name, out nint handle))
            {
                Debug.Assert(handle != 0);
                Console.WriteLine($"  TryLoad(\"{name}\") => handle=0x{handle:X}");
                NativeLibrary.Free(handle);
                Console.WriteLine("  Free(handle) done");
            }
            else
            {
                Console.WriteLine($"  TryLoad(\"{name}\") failed (ok to skip)");
            }
        }
        catch (DllNotFoundException ex)
        {
            Console.WriteLine($"  DllNotFound: {ex.Message}");
        }
    }

    [LibraryImport("kernel32.dll", EntryPoint = "GetCurrentProcessId")]
    private static partial uint GetCurrentProcessId();

    [LibraryImport("libc", EntryPoint = "getpid")]
    private static partial int GetPid();
}
