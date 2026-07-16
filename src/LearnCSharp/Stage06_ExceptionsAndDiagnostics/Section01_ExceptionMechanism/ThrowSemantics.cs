// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第1部分-异常处理机制.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section01_ExceptionMechanism
// Item     : ThrowSemantics
// Topic id : stage06/section01/throw_semantics
//
// 步骤 3：throw new、throw vs throw ex、throw 表达式、ExceptionDispatchInfo

using System.Diagnostics;
using System.Runtime.ExceptionServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section01;

internal static class ThrowSemantics
{
    [LearnTopic("stage06/section01/throw_semantics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ThrowSemantics ===");
        DemoThrowNew();
        DemoThrowVsThrowEx();
        DemoThrowExpression();
        DemoExceptionDispatchInfo();
        return 0;
    }

    private static void DemoThrowNew()
    {
        Console.WriteLine("-- throw new ArgumentOutOfRangeException --");
        try
        {
            ValidateAge(-1);
            Debug.Assert(false);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.Assert(ex.ParamName == "age");
            Console.WriteLine($"  {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void ValidateAge(int age)
    {
        if (age < 0)
            throw new ArgumentOutOfRangeException(nameof(age), "age must not be negative");
    }

    private static void DemoThrowVsThrowEx()
    {
        Console.WriteLine("-- throw; preserves stack; throw ex; resets it (CA2200) --");

        string? preserved = null;
        try
        {
            MiddleRethrowBare();
        }
        catch (InvalidOperationException ex)
        {
            preserved = ex.StackTrace;
            Debug.Assert(preserved is not null);
            Debug.Assert(preserved.Contains(nameof(DeepThrow)));
            Console.WriteLine($"  throw;  stack has DeepThrow: {preserved.Contains(nameof(DeepThrow))}");
        }

        string? reset = null;
        try
        {
            MiddleRethrowEx();
        }
        catch (InvalidOperationException ex)
        {
            reset = ex.StackTrace;
            Debug.Assert(reset is not null);
            // After throw ex;, the original throw site is typically no longer in StackTrace.
            bool lostDeep = !reset.Contains(nameof(DeepThrow));
            Debug.Assert(lostDeep || reset.Contains(nameof(MiddleRethrowEx)));
            Console.WriteLine($"  throw ex; stack has DeepThrow: {reset.Contains(nameof(DeepThrow))}");
            Console.WriteLine($"  throw ex; starts near MiddleRethrowEx: {reset.Contains(nameof(MiddleRethrowEx))}");
        }

        Debug.Assert(preserved is not null && reset is not null);
        Debug.Assert(preserved.Contains(nameof(DeepThrow)));
    }

    private static void MiddleRethrowBare()
    {
        try
        {
            DeepThrow();
        }
        catch (InvalidOperationException)
        {
            throw; // preserve original stack
        }
    }

    private static void MiddleRethrowEx()
    {
        try
        {
            DeepThrow();
        }
        catch (InvalidOperationException ex)
        {
#pragma warning disable CA2200 // intentional demo of stack reset
            throw ex;
#pragma warning restore CA2200
        }
    }

    private static void DeepThrow() => throw new InvalidOperationException("deep failure");

    private static void DemoThrowExpression()
    {
        Console.WriteLine("-- throw expressions (C# 7) --");
        string? input = null;
        try
        {
            string name = input ?? throw new ArgumentNullException(nameof(input));
            _ = name;
            Debug.Assert(false);
        }
        catch (ArgumentNullException ex)
        {
            Debug.Assert(ex.ParamName == "input");
            Console.WriteLine($"  ?? throw → {ex.GetType().Name}");
        }

        string[] empty = [];
        try
        {
            string first = empty.Length >= 1 ? empty[0] : throw new ArgumentException("missing arg");
            _ = first;
            Debug.Assert(false);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("  ternary throw → ArgumentException");
        }

        try
        {
            _ = NotReady();
            Debug.Assert(false);
        }
        catch (NotImplementedException)
        {
            Console.WriteLine("  expression-bodied throw → NotImplementedException");
        }
    }

    private static int NotReady() => throw new NotImplementedException();

    private static void DemoExceptionDispatchInfo()
    {
        Console.WriteLine("-- ExceptionDispatchInfo.Capture / Throw --");
        ExceptionDispatchInfo? captured = null;
        try
        {
            DeepThrow();
        }
        catch (InvalidOperationException ex)
        {
            captured = ExceptionDispatchInfo.Capture(ex);
        }

        Debug.Assert(captured is not null);
        try
        {
            captured.Throw();
            Debug.Assert(false);
        }
        catch (InvalidOperationException ex)
        {
            Debug.Assert(ex.StackTrace is not null && ex.StackTrace.Contains(nameof(DeepThrow)));
            Console.WriteLine($"  delayed rethrow keeps DeepThrow: {ex.StackTrace.Contains(nameof(DeepThrow))}");
        }
    }
}
