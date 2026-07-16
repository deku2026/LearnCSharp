// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第8部分-DATAS与终结化调优.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section08_DATASAndFinalization
// Item     : Finalization
// Topic id : stage11/section08/finalization
//
// Lesson: finalizers need extra GC; freachable queue; prefer IDisposable.

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section08;

internal static class Finalization
{
    private static int s_finalized;

    [LearnTopic("stage11/section08/finalization")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== Finalization ===");
        DemoNeedsTwoGcs();
        DemoResurrectionNote();
        DemoPreferDispose();
        return 0;
    }

    private static void DemoNeedsTwoGcs()
    {
        Console.WriteLine("-- finalizable objects survive an extra GC --");
        s_finalized = 0;
        CreateFinalizable();
        int genHint = 0;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        int afterFirst = s_finalized;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        int afterSecond = s_finalized;
        Console.WriteLine($"  finalized count after 1st GC+wait={afterFirst}, after 2nd={afterSecond}");
        Debug.Assert(afterSecond >= 1);
        Console.WriteLine("  Path: finalization queue → freachable → finalizer thread → next GC reclaims.");
        _ = genHint;
    }

    private static void DemoResurrectionNote()
    {
        Console.WriteLine("-- resurrection --");
        Console.WriteLine("  Finalizer can store 'this' into a static → object lives again (discouraged).");
        Console.WriteLine("  Finalizer runs on dedicated thread; never block it.");
    }

    private static void DemoPreferDispose()
    {
        Console.WriteLine("-- prefer deterministic Dispose --");
        Console.WriteLine("  Finalizer = safety net for unmanaged resources; timing uncertain.");
        Console.WriteLine("  IDisposable + SuppressFinalize is the standard pattern (next topic).");
        using var d = new QuickDisposable();
        Debug.Assert(!d.Disposed);
        d.Dispose();
        Debug.Assert(d.Disposed);
        Console.WriteLine("  QuickDisposable disposed deterministically.");
    }

    private static void CreateFinalizable() => _ = new FinalizableMarker();

    private sealed class FinalizableMarker
    {
        ~FinalizableMarker()
        {
            Interlocked.Increment(ref s_finalized);
        }
    }

    private sealed class QuickDisposable : IDisposable
    {
        public bool Disposed { get; private set; }
        public void Dispose() => Disposed = true;
    }
}
