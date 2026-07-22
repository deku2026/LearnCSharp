// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第4部分-IL高级与动态生成.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section04_ILAdvancedAndDynamic
// Item     : ExceptionHandlingClauses
// Topic id : stage11/section04/exception_handling_clauses
//
// Lesson: IL EH clauses — catch, finally, filter; leave exits protected regions.

using System.Diagnostics;
using System.Reflection;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section04;

internal static class ExceptionHandlingClauses
{
    [LearnTopic("stage11/section04/exception_handling_clauses")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExceptionHandlingClauses ===");
        DemoCatchFinally();
        DemoWhenFilter();
        DemoEhClauseMetadata();
        return 0;
    }

    private static void DemoCatchFinally()
    {
        Console.WriteLine("-- catch + finally (leave / endfinally in IL) --");
        bool finallyRan = false;
        string path;
        try
        {
            path = "try";
            ThrowIf(true);
        }
        catch (InvalidOperationException ex)
        {
            path = "catch:" + ex.Message;
        }
        finally
        {
            finallyRan = true;
        }

        Debug.Assert(finallyRan);
        Debug.Assert(path.StartsWith("catch:", StringComparison.Ordinal));
        Console.WriteLine($"  path={path}, finallyRan={finallyRan}");
    }

    private static void DemoWhenFilter()
    {
        Console.WriteLine("-- exception filter (when) → filter clause in IL --");
        string result;
        try
        {
            throw new InvalidOperationException("code=42");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("42", StringComparison.Ordinal))
        {
            result = "filtered";
        }

        Debug.Assert(result == "filtered");
        Console.WriteLine($"  filter matched → {result}");
        Console.WriteLine("  Filter runs before handler; stack still has exception object.");
    }

    private static void DemoEhClauseMetadata()
    {
        Console.WriteLine("-- ExceptionHandlingClause via reflection --");
        MethodInfo m = typeof(ExceptionHandlingClauses).GetMethod(nameof(SampleTryCatch), BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodBody? body = m.GetMethodBody();
        if (body is null)
        {
            Console.WriteLine("  no MethodBody");
            return;
        }

        Console.WriteLine($"  SampleTryCatch EH clauses: {body.ExceptionHandlingClauses.Count}");
        foreach (ExceptionHandlingClause c in body.ExceptionHandlingClauses)
        {
            Console.WriteLine($"    Flags={c.Flags}, TryOffset={c.TryOffset}, HandlerOffset={c.HandlerOffset}, CatchType={c.CatchType?.Name}");
        }

        Debug.Assert(SampleTryCatch(false) == "ok");
        Debug.Assert(SampleTryCatch(true) == "caught");
    }

    private static void ThrowIf(bool cond)
    {
        if (cond) throw new InvalidOperationException("boom");
    }

    private static string SampleTryCatch(bool fail)
    {
        try
        {
            if (fail) throw new InvalidOperationException();
            return "ok";
        }
        catch (InvalidOperationException)
        {
            return "caught";
        }
    }
}
