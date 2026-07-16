// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第3部分-PInvoke与原生互操作.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section03_PInvokeAndInterop
// Item     : SafeHandleCallingConvention
// Topic id : stage13/section03/safe_handle_calling_convention
//
// Lesson: SafeHandle RAII; UnmanagedCallConv; SetLastError; pinning vs GC moves.

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;
using Microsoft.Win32.SafeHandles;

namespace LearnCSharp.Stage13.Section03;

internal static partial class SafeHandleCallingConvention
{
    [LearnTopic("stage13/section03/safe_handle_calling_convention")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SafeHandleCallingConvention ===");
        DemoSafeHandle();
        DemoCallingConventionAndLastError();
        DemoPinning();
        return 0;
    }

    private static void DemoSafeHandle()
    {
        Console.WriteLine("-- SafeHandle vs bare IntPtr --");
        Console.WriteLine("  IntPtr: manual CloseHandle — leaks, double-free, race with GC finalizer");
        Console.WriteLine("  SafeHandle: IDisposable + critical finalizer + refcount during P/Invoke");

        // SafeFileHandle is the BCL pattern for file handles
        string path = Path.Combine(Path.GetTempPath(), $"learncsharp-stage13-{Guid.NewGuid():N}.tmp");
        try
        {
            using (FileStream fs = new(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                SafeFileHandle h = fs.SafeFileHandle;
                Debug.Assert(!h.IsInvalid && !h.IsClosed);
                Console.WriteLine($"  FileStream.SafeFileHandle IsInvalid={h.IsInvalid}");
                fs.WriteByte(42);
            }

            Console.WriteLine("  Dispose/using closed the handle (RAII ≈ unique_ptr<HANDLE,deleter>).");
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    private static void DemoCallingConventionAndLastError()
    {
        Console.WriteLine("-- calling convention + SetLastError --");
        Console.WriteLine("  Cdecl = caller cleans stack (__cdecl); StdCall = callee (__stdcall)");
        Console.WriteLine("  Mismatch → stack corruption. UnmanagedCallConv documents it for LibraryImport.");

        try
        {
            if (OperatingSystem.IsWindows())
            {
                // Intentionally invalid handle → CloseHandle fails, last error set
                bool ok = CloseHandle(nint.Zero);
                int err = Marshal.GetLastPInvokeError();
                Debug.Assert(!ok);
                Console.WriteLine($"  CloseHandle(0) => {ok}, GetLastPInvokeError={err}");
                Console.WriteLine("  Rule: check return first, then read error immediately (don't interleave).");
            }
            else
            {
                Console.WriteLine("  (Windows CloseHandle/SetLastError demo skipped)");
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

    private static void DemoPinning()
    {
        Console.WriteLine("-- pinning (only truly new idea for C++ veterans) --");
        Console.WriteLine("  GC may move objects; native wants a stable address → pin.");
        Console.WriteLine("  Auto: marshaller pins blittable arrays for call duration.");
        Console.WriteLine("  Long-lived: fixed / GCHandle.Pinned / GC.AllocateArray(pinned:true)");

        byte[] data = [1, 2, 3, 4];
        GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            nint ptr = h.AddrOfPinnedObject();
            Debug.Assert(ptr != 0);
            Console.WriteLine($"  GCHandle.Pinned AddrOfPinnedObject=0x{ptr:X}");
            // short-lived fixed also works (Section04)
        }
        finally
        {
            h.Free();
        }

        Console.WriteLine("  Unpin ASAP — long pins fragment the heap (POH exists for this).");
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(nint handle);
}
