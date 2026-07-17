// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第2部分-异常实践模式与诊断.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section02_ExceptionPracticeAndDiagnostics
// Item     : ErrorHandlingPatternsComparison
// Topic id : stage06/section02/error_handling_patterns_comparison
//
// 步骤 3：异常 vs TryParse vs Result vs 错误码

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section02;

internal static class ErrorHandlingPatternsComparison
{
    [LearnTopic("stage06/section02/error_handling_patterns_comparison")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ErrorHandlingPatternsComparison ===");
        DemoExceptionStyle();
        DemoTryPattern();
        DemoResultPattern();
        DemoErrorCodeDiscouraged();
        DemoWhenToUseWhich();
        return 0;
    }

    private static void DemoExceptionStyle()
    {
        Console.WriteLine("-- ① exception style (unexpected / rare) --");
        try
        {
            _ = ParseThrowing("not-a-number");
            Debug.Assert(false);
        }
        catch (FormatException ex)
        {
            Debug.Assert(ex.StackTrace is not null);
            Console.WriteLine($"  int.Parse failed: {ex.GetType().Name}");
        }

        Debug.Assert(ParseThrowing("42") == 42);
    }

    private static int ParseThrowing(string s) => int.Parse(s);

    private static void DemoTryPattern()
    {
        Console.WriteLine("-- ② Try pattern (expected failure, hot path) --");
        Debug.Assert(int.TryParse("42", out int ok) && ok == 42);
        Debug.Assert(!int.TryParse("nope", out _));

        bool found = TryGetPositive("7", out int value);
        Debug.Assert(found && value == 7);
        Debug.Assert(!TryGetPositive("-1", out _));
        Debug.Assert(!TryGetPositive("x", out _));
        Console.WriteLine("  TryParse / TryGetPositive return bool, never throw on format");
    }

    private static bool TryGetPositive(string s, out int value)
    {
        if (int.TryParse(s, out value) && value > 0)
            return true;
        value = 0;
        return false;
    }

    private static void DemoResultPattern()
    {
        Console.WriteLine("-- ③ Result pattern (error as data; ≈ C++23 std::expected) --");
        Result<int> good = ParseResult("99");
        Result<int> bad = ParseResult("zz");

        Debug.Assert(good.IsSuccess && good.Value == 99);
        Debug.Assert(!bad.IsSuccess && bad.Error == "invalid number");

        if (good.IsSuccess)
            Console.WriteLine($"  Ok: {good.Value}");
        if (!bad.IsSuccess)
            Console.WriteLine($"  Fail: {bad.Error}");
    }

    private static Result<int> ParseResult(string s)
        => int.TryParse(s, out int v) ? Result<int>.Ok(v) : Result<int>.Fail("invalid number");

    private static void DemoErrorCodeDiscouraged()
    {
        Console.WriteLine("-- ④ error codes (C-style; easy to ignore in C#) --");
        int code = TryDoSomething("ok", out string? output);
        Debug.Assert(code == 0 && output == "done");

        code = TryDoSomething(null, out output);
        Debug.Assert(code != 0 && output is null);
        Console.WriteLine($"  codes work but callers may forget to check (code={code})");
    }

    private static int TryDoSomething(string? input, out string? output)
    {
        if (input is null)
        {
            output = null;
            return 1; // error
        }

        output = "done";
        return 0; // success
    }

    private static void DemoWhenToUseWhich()
    {
        Console.WriteLine("-- decision guide --");
        // rare network / invariant break → exception
        // parse user input at scale → Try
        // frequent business rule failures → Result
        string[] lines =
        [
            "rare unexpected → exception (stack + cannot ignore)",
            "expected parse miss → TryParse (fast, bool+out)",
            "frequent business fail → Result (data, no stack walk)",
            "error codes → avoid as primary style in C#",
        ];
        foreach (string line in lines)
            Console.WriteLine($"  • {line}");

        Debug.Assert(ParseResult("1").IsSuccess);
        Debug.Assert(int.TryParse("1", out _));
    }

    private readonly struct Result<T>
    {
        private readonly T? _value;
        private readonly string? _error;

        private Result(T value)
        {
            _value = value;
            _error = null;
            IsSuccess = true;
        }

        private Result(string error)
        {
            _value = default;
            _error = error;
            IsSuccess = false;
        }

        public bool IsSuccess { get; }
        public T Value => IsSuccess ? _value! : throw new InvalidOperationException("no value");
        public string Error => !IsSuccess ? _error! : throw new InvalidOperationException("no error");

        public static Result<T> Ok(T value) => new(value);
        public static Result<T> Fail(string error) => new(error);
    }
}
