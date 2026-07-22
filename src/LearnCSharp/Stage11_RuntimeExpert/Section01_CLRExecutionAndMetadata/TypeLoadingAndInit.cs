// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第1部分-CLR执行模型与元数据.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section01_CLRExecutionAndMetadata
// Item     : TypeLoadingAndInit
// Topic id : stage11/section01/type_loading_and_init
//
// Lesson: type load → beforefieldinit / .cctor; static ctor runs once, thread-safe.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section01;

internal static class TypeLoadingAndInit
{
    private static int s_order;

    [LearnTopic("stage11/section01/type_loading_and_init")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TypeLoadingAndInit ===");
        DemoLazyTypeInit();
        DemoStaticCtorOnce();
        DemoBeforeFieldInitNote();
        return 0;
    }

    private static void DemoLazyTypeInit()
    {
        Console.WriteLine("-- type initialization is on-demand --");
        Console.WriteLine("  Referencing typeof(T) does not always run .cctor;");
        Console.WriteLine("  first static field access / static method usually does.");
        // Counter lives outside LazyHolder so we can observe cctor without touching LazyHolder first.
        Debug.Assert(InitProbe.Count == 0);
        int v = LazyHolder.Value;
        Console.WriteLine($"  LazyHolder.Value={v}, InitProbe.Count={InitProbe.Count}");
        Debug.Assert(v == 100);
        Debug.Assert(InitProbe.Count == 1);
        _ = LazyHolder.Value;
        Debug.Assert(InitProbe.Count == 1, "cctor runs only once");
    }

    private static void DemoStaticCtorOnce()
    {
        Console.WriteLine("-- static constructor is thread-safe & once --");
        int a = OnceType.Id;
        int b = OnceType.Id;
        Debug.Assert(a == b);
        Console.WriteLine($"  OnceType.Id={a} (same on re-read)");
        Debug.Assert(OnceType.InitCount == 1);
    }

    private static void DemoBeforeFieldInitNote()
    {
        Console.WriteLine("-- beforefieldinit vs precise .cctor --");
        Console.WriteLine("  Types with only static field initializers may get beforefieldinit:");
        Console.WriteLine("  runtime may run .cctor earlier/lazier than C# source order suggests.");
        Console.WriteLine("  Explicit static constructor forces precise timing (before first static use).");
        int n = PreciseInit.N;
        Console.WriteLine($"  PreciseInit.N={n}");
        Debug.Assert(n == 7);
        Console.WriteLine($"  init order marker s_order ended at {s_order}");
    }

    private static class InitProbe
    {
        public static int Count;
    }

    private static class LazyHolder
    {
        public static readonly int Value = Init();

        private static int Init()
        {
            InitProbe.Count++;
            s_order++;
            Console.WriteLine("  [LazyHolder .cctor ran]");
            return 100;
        }
    }

    private static class OnceType
    {
        public static readonly int InitCount;
        public static readonly int Id;

        static OnceType()
        {
            InitCount++;
            Id = Environment.CurrentManagedThreadId;
            s_order++;
            Console.WriteLine($"  [OnceType .cctor on thread {Id}]");
        }
    }

    private static class PreciseInit
    {
        public static readonly int N;

        static PreciseInit()
        {
            N = 7;
            s_order++;
            Console.WriteLine("  [PreciseInit explicit static ctor]");
        }
    }
}
