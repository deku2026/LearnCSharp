// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第2部分-异常实践模式与诊断.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section02_ExceptionPracticeAndDiagnostics
// Item     : ExceptionBestPracticesThrowIf
// Topic id : stage06/section02/exception_best_practices_throwif
//
// 步骤 2：异常最佳实践、ThrowIf 守卫、不吞异常、不用异常控制流程

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section02;

internal static class ExceptionBestPracticesThrowIf
{
    [LearnTopic("stage06/section02/exception_best_practices_throwif")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExceptionBestPracticesThrowIf ===");
        DemoThrowIfGuards();
        DemoRightExceptionTypes();
        DemoDoNotSwallow();
        DemoAvoidExceptionForControlFlow();
        return 0;
    }

    private static void DemoThrowIfGuards()
    {
        Console.WriteLine("-- Argument*.ThrowIf* guards (.NET 6+) --");
        try
        {
            Process(name: null!, count: 1);
            Debug.Assert(false);
        }
        catch (ArgumentNullException ex)
        {
            Debug.Assert(ex.ParamName == "name");
            Console.WriteLine($"  ThrowIfNull → {ex.ParamName}");
        }

        try
        {
            Process(name: "  ", count: 1);
            Debug.Assert(false);
        }
        catch (ArgumentException ex)
        {
            Debug.Assert(ex.ParamName == "name");
            Console.WriteLine($"  ThrowIfNullOrWhiteSpace → {ex.ParamName}");
        }

        try
        {
            Process(name: "ok", count: -3);
            Debug.Assert(false);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.Assert(ex.ParamName == "count");
            Console.WriteLine($"  ThrowIfNegative → {ex.ParamName}");
        }

        Process(name: "ok", count: 2);
        Console.WriteLine("  valid call passes");
    }

    private static void Process(string name, int count)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, 100);
        Console.WriteLine($"  Process({name}, {count})");
    }

    private static void DemoRightExceptionTypes()
    {
        Console.WriteLine("-- use predefined types, not bare Exception --");
        try
        {
            var svc = new Service();
            svc.RunBeforeInit();
            Debug.Assert(false);
        }
        catch (InvalidOperationException ex)
        {
            Debug.Assert(ex.Message.Contains("not initialized", StringComparison.OrdinalIgnoreCase)
                         || ex.Message.Length > 0);
            Console.WriteLine($"  InvalidOperationException: {ex.Message}");
        }
    }

    private static void DemoDoNotSwallow()
    {
        Console.WriteLine("-- never empty catch; log then rethrow if needed --");
        string? loggedType = null;
        try
        {
            try
            {
                throw new FileNotFoundException("missing.dat");
            }
            catch (FileNotFoundException ex)
            {
                loggedType = ex.GetType().Name;
                Console.WriteLine($"  logged full exception: {ex}");
                // can handle here — do not rethrow
            }
        }
        catch
        {
            Debug.Assert(false, "should have been handled");
        }

        Debug.Assert(loggedType == nameof(FileNotFoundException));

        string? rethrown = null;
        try
        {
            try
            {
                throw new InvalidOperationException("cannot handle here");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  logged then bare throw: {ex.GetType().Name}");
                throw; // preserve stack
            }
        }
        catch (InvalidOperationException ex)
        {
            rethrown = ex.Message;
        }

        Debug.Assert(rethrown == "cannot handle here");
    }

    private static void DemoAvoidExceptionForControlFlow()
    {
        Console.WriteLine("-- TryParse instead of Parse+catch in loops --");
        string[] inputs = ["10", "x", "20", "bad", "30"];
        int sumParseCatch = 0;
        int sumTryParse = 0;

        foreach (string s in inputs)
        {
            try
            {
                sumParseCatch += int.Parse(s);
            }
            catch (FormatException)
            {
                // anti-pattern for expected invalid input
            }
        }

        foreach (string s in inputs)
        {
            if (int.TryParse(s, out int n))
                sumTryParse += n;
        }

        Debug.Assert(sumParseCatch == 60 && sumTryParse == 60);
        Console.WriteLine($"  both sums=60; prefer TryParse (no throw on 'x'/'bad')");
    }

    private sealed class Service
    {
        private bool _ready;

        public void RunBeforeInit()
        {
            if (!_ready)
                throw new InvalidOperationException("service not initialized");
        }
    }
}
