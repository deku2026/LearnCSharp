// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第4部分-unsafe指针Marshal函数指针.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section04_UnsafePointersMarshalAndFunctionPointers
// Item     : UnsafeAndPointers
// Topic id : stage13/section04/unsafe_and_pointers
//
// Lesson: unsafe fence + C-like pointers (& * -> [] arithmetic).

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section04;

internal static class UnsafeAndPointers
{
    [LearnTopic("stage13/section04/unsafe_and_pointers")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== UnsafeAndPointers ===");
        DemoPointerBasics();
        DemoPointerArithmetic();
        DemoWhenUnsafe();
        return 0;
    }

    private static unsafe void DemoPointerBasics()
    {
        Console.WriteLine("-- pointer basics (C++ muscle memory) --");
        int x = 42;
        int* p = &x;
        *p = 100;
        Debug.Assert(x == 100);
        Console.WriteLine($"  int x; int* p=&x; *p=100 → x={x}");

        Point pt = default;
        Point* pp = &pt;
        pp->X = 10;
        pp->Y = 20;
        Debug.Assert(pt.X == 10 && pt.Y == 20);
        Console.WriteLine($"  Point* -> X={pt.X}, Y={pt.Y}");
        Console.WriteLine($"  sizeof(int)={sizeof(int)}, sizeof(Point)={sizeof(Point)}");
    }

    private static unsafe void DemoPointerArithmetic()
    {
        Console.WriteLine("-- pointer arithmetic --");
        int* arr = stackalloc int[3] { 1, 2, 3 };
        int* q = arr;
        for (int i = 0; i < 3; i++)
            *q++ *= 2;

        Debug.Assert(arr[0] == 2 && arr[1] == 4 && arr[2] == 6);
        Console.WriteLine($"  stackalloc *q++ *= 2 → [{arr[0]},{arr[1]},{arr[2]}]");
        Console.WriteLine($"  arr[2] via index == *(arr+2) == {*(arr + 2)}");

        void* v = arr;
        int* back = (int*)v;
        Debug.Assert(*back == 2);
        Console.WriteLine("  void* + cast works like C.");
    }

    private static void DemoWhenUnsafe()
    {
        Console.WriteLine("-- when to use unsafe --");
        Console.WriteLine("  1) interop: pass raw pointers to native");
        Console.WriteLine("  2) hot paths: skip bounds checks (measure first!)");
        Console.WriteLine("  3) non-GC memory: NativeMemory, mmap views");
        Console.WriteLine("  Prefer Span<T> for most buffer work; open unsafe fence only when needed.");
        Console.WriteLine("  C# default safe; C++ default unsafe — fence makes danger auditable.");
    }

    private struct Point
    {
        public int X;
        public int Y;
    }
}
