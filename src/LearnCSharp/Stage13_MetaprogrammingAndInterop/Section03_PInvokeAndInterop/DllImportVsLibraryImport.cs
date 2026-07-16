// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第3部分-PInvoke与原生互操作.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section03_PInvokeAndInterop
// Item     : DllImportVsLibraryImport
// Topic id : stage13/section03/dll_import_vs_library_import
//
// Lesson: DllImport = runtime IL stubs; LibraryImport = compile-time marshalling (AOT-friendly).

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section03;

internal static partial class DllImportVsLibraryImport
{
    [LearnTopic("stage13/section03/dll_import_vs_library_import")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DllImportVsLibraryImport ===");
        DemoComparison();
        DemoBothStyles();
        DemoMigrationNotes();
        return 0;
    }

    private static void DemoComparison()
    {
        Console.WriteLine("-- comparison --");
        Console.WriteLine("  DllImport:  static extern; runtime generates IL stub; hard for Native AOT");
        Console.WriteLine("  LibraryImport: static partial; Roslyn emits marshalling source; AOT-ok, visible");
        Console.WriteLine("  CharSet → StringMarshalling; CallingConvention → UnmanagedCallConv");
        Console.WriteLine("  Prefer LibraryImport for new code (SYSLIB1054 suggests migration).");
    }

    private static void DemoBothStyles()
    {
        Console.WriteLine("-- same OS API, both declaration styles --");
        try
        {
            if (OperatingSystem.IsWindows())
            {
                ulong a = DllImportGetTickCount64();
                ulong b = LibraryImportGetTickCount64();
                Debug.Assert(a > 0 && b > 0);
                Console.WriteLine($"  DllImport GetTickCount64    => {a}");
                Console.WriteLine($"  LibraryImport GetTickCount64 => {b}");
            }
            else
            {
                Console.WriteLine("  (Windows-only dual-style demo skipped)");
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

    private static void DemoMigrationNotes()
    {
        Console.WriteLine("-- migration checklist --");
        Console.WriteLine("  1) extern → partial");
        Console.WriteLine("  2) [DllImport] → [LibraryImport]");
        Console.WriteLine("  3) CharSet.Unicode → StringMarshalling.Utf16");
        Console.WriteLine("  4) explicit MarshalAs for bool etc. (no implicit surprises)");
        Console.WriteLine("  5) rare unsupported configs still use DllImport (SYSLIB1050-1069)");
    }

    [DllImport("kernel32.dll", EntryPoint = "GetTickCount64")]
    private static extern ulong DllImportGetTickCount64();

    [LibraryImport("kernel32.dll", EntryPoint = "GetTickCount64")]
    private static partial ulong LibraryImportGetTickCount64();
}
