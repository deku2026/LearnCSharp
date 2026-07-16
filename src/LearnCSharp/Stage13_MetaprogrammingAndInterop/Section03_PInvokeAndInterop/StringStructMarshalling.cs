// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第3部分-PInvoke与原生互操作.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section03_PInvokeAndInterop
// Item     : StringStructMarshalling
// Topic id : stage13/section03/string_struct_marshalling
//
// Lesson: StringMarshalling + StructLayout Sequential/Pack/Explicit match C ABI.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section03;

internal static partial class StringStructMarshalling
{
    [LearnTopic("stage13/section03/string_struct_marshalling")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== StringStructMarshalling ===");
        DemoStringMarshalling();
        DemoStructLayouts();
        DemoOsStringApi();
        return 0;
    }

    private static void DemoStringMarshalling()
    {
        Console.WriteLine("-- string marshalling --");
        Console.WriteLine("  LibraryImport StringMarshalling.Utf8 / Utf16 (replaces CharSet)");
        Console.WriteLine("  Strings always non-blittable → convert + free temporary buffers");

        nint utf8 = Marshal.StringToCoTaskMemUTF8("hello");
        nint utf16 = Marshal.StringToHGlobalUni("hello");
        try
        {
            string back8 = Marshal.PtrToStringUTF8(utf8)!;
            string back16 = Marshal.PtrToStringUni(utf16)!;
            Debug.Assert(back8 == "hello" && back16 == "hello");
            Console.WriteLine($"  UTF-8 round-trip: {back8}");
            Console.WriteLine($"  UTF-16 round-trip: {back16}");
            Console.WriteLine($"  UTF-8 bytes: {BitConverter.ToString(Encoding.UTF8.GetBytes("hello"))}");
        }
        finally
        {
            Marshal.FreeCoTaskMem(utf8);
            Marshal.FreeHGlobal(utf16);
        }
    }

    private static void DemoStructLayouts()
    {
        Console.WriteLine("-- StructLayout: Sequential / Pack / Explicit --");
        Console.WriteLine($"  Sequential Point: size={Marshal.SizeOf<Point>()} (expect 8)");
        Console.WriteLine($"  Pack=1 Header: size={Marshal.SizeOf<PackedHeader>()} (byte+int → 5)");
        Console.WriteLine($"  default-aligned Header: size={Marshal.SizeOf<AlignedHeader>()} (often 8)");
        Debug.Assert(Marshal.SizeOf<Point>() == 8);
        Debug.Assert(Marshal.SizeOf<PackedHeader>() == 5);
        Debug.Assert(Marshal.SizeOf<AlignedHeader>() >= 5);

        var u = new Overlap { AsInt = 0x3F800000 }; // IEEE 1.0f bit pattern
        float f = u.AsFloat;
        Debug.Assert(Math.Abs(f - 1.0f) < 1e-6);
        Console.WriteLine($"  Explicit union: AsInt=0x{u.AsInt:X8} → AsFloat={f}");
        Console.WriteLine("  Match C: Sequential≈declaration order, Pack≈#pragma pack, Explicit≈union.");
    }

    private static void DemoOsStringApi()
    {
        Console.WriteLine("-- OS string API (guarded) --");
        try
        {
            if (OperatingSystem.IsWindows())
            {
                // GetEnvironmentVariableW via LibraryImport Utf16
                Span<char> buf = stackalloc char[256];
                uint n = GetEnvironmentVariableW("OS", buf, (uint)buf.Length);
                if (n > 0 && n < buf.Length)
                {
                    string os = new(buf[..(int)n]);
                    Console.WriteLine($"  GetEnvironmentVariableW(\"OS\") => {os}");
                    Debug.Assert(os.Length > 0);
                }
                else
                {
                    Console.WriteLine($"  GetEnvironmentVariableW returned {n}");
                }
            }
            else
            {
                Console.WriteLine("  (Windows GetEnvironmentVariableW demo skipped)");
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

    [LibraryImport("kernel32.dll", EntryPoint = "GetEnvironmentVariableW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial uint GetEnvironmentVariableW(string lpName, Span<char> lpBuffer, uint nSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct PackedHeader
    {
        public byte Tag;
        public int Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AlignedHeader
    {
        public byte Tag;
        public int Length;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct Overlap
    {
        [FieldOffset(0)] public int AsInt;
        [FieldOffset(0)] public float AsFloat;
    }
}
