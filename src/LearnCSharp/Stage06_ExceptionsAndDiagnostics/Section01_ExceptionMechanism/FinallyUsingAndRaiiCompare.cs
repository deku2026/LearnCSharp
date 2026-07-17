// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第1部分-异常处理机制.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section01_ExceptionMechanism
// Item     : FinallyUsingAndRaiiCompare
// Topic id : stage06/section01/finally_using_and_raii_compare
//
// 步骤 5：finally 总执行、using=try/finally、别在 finally 抛、C++ RAII 对照

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section01;

internal static class FinallyUsingAndRaiiCompare
{
    [LearnTopic("stage06/section01/finally_using_and_raii_compare")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== FinallyUsingAndRaiiCompare ===");
        DemoFinallyOnThrowReturnNormal();
        DemoUsingDisposesOnException();
        DemoUsingDeclaration();
        DemoFinallyMustNotMask();
        DemoRaiiCompareNote();
        return 0;
    }

    private static void DemoFinallyOnThrowReturnNormal()
    {
        Console.WriteLine("-- finally runs for throw / return / normal --");
        List<string> log = new List<string>();

        try
        {
            try
            {
                log.Add("try-throw");
                throw new InvalidOperationException("x");
            }
            finally
            {
                log.Add("finally-throw");
            }
        }
        catch (InvalidOperationException)
        {
            log.Add("caught");
        }

        int Ret()
        {
            try
            {
                log.Add("try-return");
                return 1;
            }
            finally
            {
                log.Add("finally-return");
            }
        }

        Debug.Assert(Ret() == 1);

        try
        {
            log.Add("try-normal");
        }
        finally
        {
            log.Add("finally-normal");
        }

        Debug.Assert(log is ["try-throw", "finally-throw", "caught", "try-return", "finally-return", "try-normal", "finally-normal"]);
        Console.WriteLine($"  sequence: {string.Join(" → ", log)}");
    }

    private static void DemoUsingDisposesOnException()
    {
        Console.WriteLine("-- using disposes even when body throws --");
        TrackedResource resource = new TrackedResource("fs");
        try
        {
            using (resource)
            {
                Debug.Assert(!resource.Disposed);
                throw new IOException("read failed");
            }
        }
        catch (IOException)
        {
            // intentional
        }

        Debug.Assert(resource.Disposed);
        Console.WriteLine($"  disposed after exception: {resource.Disposed}");
    }

    private static void DemoUsingDeclaration()
    {
        Console.WriteLine("-- using declaration (C# 8) --");
        TrackedResource a = new TrackedResource("a");
        TrackedResource b = new TrackedResource("b");
        {
            using TrackedResource ua = a;
            using TrackedResource ub = b;
            Debug.Assert(!a.Disposed && !b.Disposed);
        }

        Debug.Assert(a.Disposed && b.Disposed);
        Console.WriteLine("  both disposed at end of scope");
    }

    private static void DemoFinallyMustNotMask()
    {
        Console.WriteLine("-- finally throwing masks original (CA2219 anti-pattern) --");
        Exception? seen = null;
        try
        {
            try
            {
                throw new InvalidOperationException("original-A");
            }
            finally
            {
                // BAD: demonstrates masking; production code must not do this.
                throw new InvalidOperationException("cleanup-B");
            }
        }
        catch (InvalidOperationException ex)
        {
            seen = ex;
        }

        Debug.Assert(seen is not null);
        Debug.Assert(seen.Message == "cleanup-B");
        Console.WriteLine($"  only outer exception survives: {seen.Message} (A was masked)");
    }

    private static void DemoRaiiCompareNote()
    {
        Console.WriteLine("-- C# using/finally vs C++ RAII --");
        // C++: stack objects destruct automatically on unwind (no finally needed).
        // C#: GC is non-deterministic → opt-in IDisposable + using/finally for cleanup.
        using TrackedResource r = new TrackedResource("raii-like");
        Debug.Assert(!r.Disposed);
        Console.WriteLine("  C# using ≈ C++ RAII scope guard; C++ has no finally");
    }

    private sealed class TrackedResource : IDisposable
    {
        private readonly string _name;

        public TrackedResource(string name) => _name = name;

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Disposed = true;
            Console.WriteLine($"  Dispose({_name})");
        }
    }
}
