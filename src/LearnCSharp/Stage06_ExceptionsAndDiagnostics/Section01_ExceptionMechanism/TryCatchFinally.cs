// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第1部分-异常处理机制.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section01_ExceptionMechanism
// Item     : TryCatchFinally
// Topic id : stage06/section01/try_catch_finally
//
// 步骤 1：try/catch/finally 基本结构、多 catch 顺序、finally 总执行、沿调用栈传播

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section01;

internal static class TryCatchFinally
{
    [LearnTopic("stage06/section01/try_catch_finally")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== TryCatchFinally ===");
        DemoBasicTryCatchFinally();
        DemoMultipleCatchOrder();
        DemoFinallyAlwaysRuns();
        DemoStackPropagation();
        return 0;
    }

    private static void DemoBasicTryCatchFinally()
    {
        Console.WriteLine("-- basic try / catch / finally --");
        string? caught = null;
        try
        {
            int[] arr = { 1, 2, 3 };
            _ = arr[10];
        }
        catch (IndexOutOfRangeException ex)
        {
            caught = ex.GetType().Name;
            Console.WriteLine($"  caught IndexOutOfRange: {ex.Message}");
        }
        catch (Exception ex)
        {
            caught = ex.GetType().Name;
            Console.WriteLine($"  fallback: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("  finally: cleanup always runs");
        }

        Debug.Assert(caught == nameof(IndexOutOfRangeException));
    }

    private static void DemoMultipleCatchOrder()
    {
        Console.WriteLine("-- multi-catch: specific before general --");
        // catch (Exception) before IndexOutOfRangeException would be a compile error.
        string path = HandleDivide(10, 0);
        string path2 = HandleDivide(10, 2);
        Debug.Assert(path == "div0");
        Debug.Assert(path2 == "ok:5");
        Console.WriteLine($"  10/0 → {path}; 10/2 → {path2}");
    }

    private static string HandleDivide(int a, int b)
    {
        try
        {
            return $"ok:{a / b}";
        }
        catch (DivideByZeroException)
        {
            return "div0";
        }
        catch (Exception ex)
        {
            return $"other:{ex.GetType().Name}";
        }
    }

    private static void DemoFinallyAlwaysRuns()
    {
        Console.WriteLine("-- finally on throw / return / normal --");
        int cleanupCount = 0;
        void Cleanup() => cleanupCount++;

        try
        {
            try
            {
                throw new InvalidOperationException("boom");
            }
            finally
            {
                Cleanup();
            }
        }
        catch (InvalidOperationException)
        {
            // intentional
        }

        int WithReturn()
        {
            try
            {
                return 42;
            }
            finally
            {
                Cleanup();
            }
        }

        Debug.Assert(WithReturn() == 42);

        try
        {
            Cleanup(); // normal path also counts via explicit call pattern
        }
        finally
        {
            // already cleaned above for normal path demo
        }

        // throw + return each ran finally once; plus one normal Cleanup = 3
        Debug.Assert(cleanupCount == 3);
        Console.WriteLine($"  cleanup invocations = {cleanupCount} (throw + return + normal)");
    }

    private static void DemoStackPropagation()
    {
        Console.WriteLine("-- exception propagates up the call stack --");
        string? site = null;
        try
        {
            Level1();
        }
        catch (InvalidOperationException ex)
        {
            site = ex.Message;
            Debug.Assert(ex.StackTrace is not null && ex.StackTrace.Contains(nameof(Level3)));
            Console.WriteLine($"  outer catch: {ex.Message}");
            Console.WriteLine($"  stack includes Level3: {ex.StackTrace.Contains(nameof(Level3))}");
        }

        Debug.Assert(site == "from Level3");
    }

    private static void Level1() => Level2();
    private static void Level2() => Level3();
    private static void Level3() => throw new InvalidOperationException("from Level3");
}
