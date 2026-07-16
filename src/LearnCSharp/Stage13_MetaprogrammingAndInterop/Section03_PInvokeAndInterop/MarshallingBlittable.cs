// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第3部分-PInvoke与原生互操作.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section03_PInvokeAndInterop
// Item     : MarshallingBlittable
// Topic id : stage13/section03/marshalling_blittable
//
// Lesson: marshalling converts representations; blittable = same bytes both sides.

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section03;

internal static class MarshallingBlittable
{
    [LearnTopic("stage13/section03/marshalling_blittable")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== MarshallingBlittable ===");
        DemoWhatIsMarshalling();
        DemoBlittableVsNot();
        DemoBlittableStructLayout();
        return 0;
    }

    private static void DemoWhatIsMarshalling()
    {
        Console.WriteLine("-- marshalling --");
        Console.WriteLine("  Managed string = UTF-16 GC object; C char* = different representation.");
        Console.WriteLine("  Boundary call converts args/returns when layouts differ.");
        Console.WriteLine("  Blittable: skip conversion (pass memory as-is / pin + pointer).");
    }

    private static void DemoBlittableVsNot()
    {
        Console.WriteLine("-- blittable vs non-blittable --");
        Console.WriteLine("  Blittable: byte/short/int/long/float/double/nint + pure blittable structs/arrays");
        Console.WriteLine("  Non-blittable: bool (size varies), char (encoding), string, classes, mixed structs");

        Debug.Assert(IsBlittable(typeof(int)));
        Debug.Assert(IsBlittable(typeof(Point2)));
        Debug.Assert(!IsBlittable(typeof(string)));
        Debug.Assert(!IsBlittable(typeof(bool)));
        Console.WriteLine($"  int blittable? {IsBlittable(typeof(int))}");
        Console.WriteLine($"  Point2 (two ints) blittable? {IsBlittable(typeof(Point2))}");
        Console.WriteLine($"  string blittable? {IsBlittable(typeof(string))}");
        Console.WriteLine($"  bool blittable? {IsBlittable(typeof(bool))}");
        Console.WriteLine("  Prefer blittable params for hot interop (zero copy/convert).");
    }

    private static void DemoBlittableStructLayout()
    {
        Console.WriteLine("-- blittable struct size/offsets --");
        int size = Marshal.SizeOf<Point2>();
        nint offY = Marshal.OffsetOf<Point2>(nameof(Point2.Y));
        Debug.Assert(size == 8 && offY == 4);
        Console.WriteLine($"  Point2 SizeOf={size}, OffsetOf(Y)={offY}");

        // Simulate native write via Marshal (manual marshalling toolbox)
        nint buf = Marshal.AllocHGlobal(size);
        try
        {
            var p = new Point2 { X = 3, Y = 4 };
            Marshal.StructureToPtr(p, buf, fDeleteOld: false);
            Point2 back = Marshal.PtrToStructure<Point2>(buf);
            Debug.Assert(back.X == 3 && back.Y == 4);
            Console.WriteLine($"  StructureToPtr/PtrToStructure round-trip: ({back.X},{back.Y})");
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }
    }

    private static bool IsBlittable(Type t)
    {
        // Educational approximation (not a complete runtime definition)
        if (t == typeof(bool) || t == typeof(char) || t == typeof(string) || t == typeof(decimal))
            return false;
        if (t.IsPrimitive || t.IsEnum)
            return true;
        if (t == typeof(nint) || t == typeof(nuint) || t == typeof(IntPtr) || t == typeof(UIntPtr))
            return true;
        if (!t.IsValueType || t.IsClass)
            return false;
        foreach (System.Reflection.FieldInfo f in t.GetFields(
                     System.Reflection.BindingFlags.Instance |
                     System.Reflection.BindingFlags.Public |
                     System.Reflection.BindingFlags.NonPublic))
        {
            if (!IsBlittable(f.FieldType))
                return false;
        }

        return true;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point2
    {
        public int X;
        public int Y;
    }
}
