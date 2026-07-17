// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第1部分-异常处理机制.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section01_ExceptionMechanism
// Item     : ExceptionFilterWhen
// Topic id : stage06/section01/exception_filter_when
//
// 步骤 4：异常筛选器 when、按条件捕获、log-and-propagate

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section01;

internal static class ExceptionFilterWhen
{
    [LearnTopic("stage06/section01/exception_filter_when")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExceptionFilterWhen ===");
        DemoWhenByErrorCode();
        DemoWhenFalsePropagates();
        DemoLogAndPropagate();
        DemoWhenVsCatchThenRethrow();
        return 0;
    }

    private static void DemoWhenByErrorCode()
    {
        Console.WriteLine("-- catch when (error code) --");
        string Handle(int code)
        {
            try
            {
                throw new ApiException(code, $"HTTP {code}");
            }
            catch (ApiException ex) when (ex.StatusCode == 404)
            {
                return "not-found";
            }
            catch (ApiException ex) when (ex.StatusCode == 401)
            {
                return "unauthorized";
            }
            catch (ApiException)
            {
                return "other-http";
            }
        }

        Debug.Assert(Handle(404) == "not-found");
        Debug.Assert(Handle(401) == "unauthorized");
        Debug.Assert(Handle(500) == "other-http");
        Console.WriteLine($"  404→{Handle(404)}, 401→{Handle(401)}, 500→{Handle(500)}");
    }

    private static void DemoWhenFalsePropagates()
    {
        Console.WriteLine("-- when false: not caught, stack intact --");
        string? outer = null;
        try
        {
            try
            {
                throw new ApiException(503, "unavailable");
            }
            catch (ApiException ex) when (ex.StatusCode == 404)
            {
                outer = "should-not-run";
            }
        }
        catch (ApiException ex)
        {
            outer = ex.Message;
            Debug.Assert(ex.StackTrace is not null);
        }

        Debug.Assert(outer == "unavailable");
        Console.WriteLine($"  503 skipped 404-when filter → outer catch: {outer}");
    }

    private static void DemoLogAndPropagate()
    {
        Console.WriteLine("-- log-and-propagate (when Log returns false) --");
        int logHits = 0;
        bool Log(Exception ex)
        {
            logHits++;
            Console.WriteLine($"  [filter-log] {ex.GetType().Name}: {ex.Message}");
            return false; // never capture
        }

        string? outer = null;
        try
        {
            try
            {
                throw new InvalidOperationException("still bubbles");
            }
            catch (Exception ex) when (Log(ex))
            {
                outer = "unreachable";
            }
        }
        catch (InvalidOperationException ex)
        {
            outer = ex.Message;
        }

        Debug.Assert(logHits == 1);
        Debug.Assert(outer == "still bubbles");
        Console.WriteLine($"  log hits={logHits}, outer message={outer}");
    }

    private static void DemoWhenVsCatchThenRethrow()
    {
        Console.WriteLine("-- when filters before capture (cleaner than catch+if+throw) --");
        int handled = 0;
        try
        {
            throw new ApiException(404, "missing");
        }
        catch (ApiException ex) when (ex.StatusCode is >= 400 and < 500)
        {
            handled = ex.StatusCode;
        }

        Debug.Assert(handled == 404);
        Console.WriteLine($"  client-error when matched status={handled}");
    }

    private sealed class ApiException : Exception
    {
        public ApiException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}
