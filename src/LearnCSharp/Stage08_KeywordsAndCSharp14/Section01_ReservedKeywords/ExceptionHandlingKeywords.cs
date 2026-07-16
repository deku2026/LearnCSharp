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
        DemoThrowVsThrowExStackTrace();
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

    private static void DemoThrowVsThrowExStackTrace()
    {
        Console.WriteLine("-- throw; preserves stack vs throw ex resets --");

        string? preservedStack = null;
        try
        {
            try
            {
                DeepThrow();
            }
            catch (Exception ex)
            {
                // rethrow without reset
                throw;
            }
        }
        catch (Exception ex)
        {
            preservedStack = ex.StackTrace ?? string.Empty;
            Debug.Assert(preservedStack.Contains(nameof(DeepThrow), StringComparison.Ordinal));
            Console.WriteLine($"  throw; keeps DeepThrow frame: {preservedStack.Contains(nameof(DeepThrow))}");
        }

        string? resetStack = null;
        try
        {
            try
            {
                DeepThrow();
            }
            catch (Exception ex)
            {
                // BAD: resets stack to this catch site (DeepThrow often disappears).
                throw ex;
            }
        }
        catch (Exception ex)
        {
            resetStack = ex.StackTrace ?? string.Empty;
            bool stillHasDeep = resetStack.Contains(nameof(DeepThrow), StringComparison.Ordinal);
            bool hasCatchSite = resetStack.Contains(nameof(DemoThrowVsThrowExStackTrace), StringComparison.Ordinal);
            Console.WriteLine($"  throw ex: DeepThrow frame kept={stillHasDeep} (often false); catch-site present={hasCatchSite}");
            // On modern runtimes DeepThrow may still appear via enhanced traces; the reset is the catch rethrow site.
            Debug.Assert(hasCatchSite || !stillHasDeep || resetStack != preservedStack);
            Debug.Assert(preservedStack is not null && resetStack is not null);
            // Clear contract: throw; retains original throw site; throw ex starts rethrow from catch.
            Console.WriteLine("  prefer throw; (or ExceptionDispatchInfo) — never throw ex for rethrow");
        }

        Debug.Assert(preservedStack is not null && resetStack is not null);
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
