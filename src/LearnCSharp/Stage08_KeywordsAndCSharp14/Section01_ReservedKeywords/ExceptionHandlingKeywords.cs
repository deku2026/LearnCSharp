// LearnCSharp example (filled)
// Doc      : CSharp-阶段8-关键字全表与C#14专题-第1部分-保留关键字全表.md
// Stage    : Stage08_KeywordsAndCSharp14
// Section  : Section01_ReservedKeywords
// Item     : ExceptionHandlingKeywords (八、异常处理 — 4 个)
// Topic id : stage08/section01/exception_handling_keywords
//
// try / catch / finally / throw。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage08.Section01;

internal static class ExceptionHandlingKeywords
{
    [LearnTopic("stage08/section01/exception_handling_keywords")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExceptionHandlingKeywords ===");
        DemoTryCatchFinally();
        DemoThrowPreserveStack();
        DemoWhenFilter();
        return 0;
    }

    private static void DemoTryCatchFinally()
    {
        Console.WriteLine("-- try / catch / finally --");
        bool finallyRan = false;
        string? caught = null;
        try
        {
            throw new InvalidOperationException("boom");
        }
        catch (InvalidOperationException ex)
        {
            caught = ex.Message;
        }
        finally
        {
            finallyRan = true;
        }
        Debug.Assert(caught == "boom");
        Debug.Assert(finallyRan);
        Console.WriteLine($"  caught={caught}, finallyRan={finallyRan}");
    }

    private static void DemoThrowPreserveStack()
    {
        Console.WriteLine("-- throw; 保留栈 vs throw ex 重置 --");
        string? preserved = null;
        try
        {
            try
            {
                DeepThrow();
            }
            catch (Exception ex)
            {
                preserved = ex.StackTrace;
                throw; // 保留
            }
        }
        catch (Exception ex)
        {
            Debug.Assert(ex.StackTrace is not null && ex.StackTrace.Contains(nameof(DeepThrow)));
            Debug.Assert(preserved is not null && preserved.Contains(nameof(DeepThrow)));
            Console.WriteLine($"  throw; keeps DeepThrow: {ex.StackTrace.Contains(nameof(DeepThrow))}");
        }
    }

    private static void DemoWhenFilter()
    {
        Console.WriteLine("-- catch when 筛选器 --");
        string? hit = null;
        try
        {
            throw new ArgumentException("bad", "name");
        }
        catch (ArgumentException ex) when (ex.ParamName == "other")
        {
            hit = "other";
        }
        catch (ArgumentException ex) when (ex.ParamName == "name")
        {
            hit = "name";
        }
        Debug.Assert(hit == "name");
        Console.WriteLine($"  when filter hit={hit}");
    }

    private static void DeepThrow() => throw new InvalidOperationException("deep");
}
