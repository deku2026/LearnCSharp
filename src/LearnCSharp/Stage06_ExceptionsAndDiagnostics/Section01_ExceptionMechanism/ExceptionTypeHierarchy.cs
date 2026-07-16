// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第1部分-异常处理机制.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section01_ExceptionMechanism
// Item     : ExceptionTypeHierarchy
// Topic id : stage06/section01/exception_type_hierarchy
//
// 步骤 2：异常类型层次、Exception 属性、InnerException 异常链

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section01;

internal static class ExceptionTypeHierarchy
{
    [LearnTopic("stage06/section01/exception_type_hierarchy")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ExceptionTypeHierarchy ===");
        DemoCommonExceptionTypes();
        DemoExceptionProperties();
        DemoInnerExceptionChain();
        DemoCatchByBaseType();
        return 0;
    }

    private static void DemoCommonExceptionTypes()
    {
        Console.WriteLine("-- common BCL exception types --");
        Exception[] samples =
        [
            new ArgumentNullException("name"),
            new ArgumentOutOfRangeException("count", "must be >= 0"),
            new InvalidOperationException("not ready"),
            new FormatException("bad format"),
            new KeyNotFoundException("missing key"),
        ];

        foreach (Exception ex in samples)
        {
            Debug.Assert(ex is Exception);
            Debug.Assert(ex.GetType().IsSubclassOf(typeof(Exception)) || ex.GetType() == typeof(Exception));
            Console.WriteLine($"  {ex.GetType().Name}: {ex.Message}");
        }

        Debug.Assert(typeof(ArgumentNullException).IsSubclassOf(typeof(ArgumentException)));
        Debug.Assert(typeof(ArgumentException).IsSubclassOf(typeof(SystemException)));
        Debug.Assert(typeof(SystemException).IsSubclassOf(typeof(Exception)));
        Console.WriteLine("  hierarchy: ArgumentNull → Argument → SystemException → Exception");
    }

    private static void DemoExceptionProperties()
    {
        Console.WriteLine("-- Exception properties --");
        try
        {
            ThrowWithData();
        }
        catch (Exception ex)
        {
            Debug.Assert(!string.IsNullOrEmpty(ex.Message));
            Debug.Assert(ex.StackTrace is not null);
            Debug.Assert(ex.TargetSite is not null);
            Debug.Assert(ex.Data.Contains("hint"));
            Console.WriteLine($"  Message     = {ex.Message}");
            Console.WriteLine($"  TargetSite  = {ex.TargetSite!.Name}");
            Console.WriteLine($"  HResult     = 0x{ex.HResult:X8}");
            Console.WriteLine($"  Data[hint]  = {ex.Data["hint"]}");
            Console.WriteLine($"  StackTrace lines ≈ {ex.StackTrace.Split('\n').Length}");
        }
    }

    private static void ThrowWithData()
    {
        var ex = new InvalidOperationException("demo failure");
        ex.Data["hint"] = "check config";
        throw ex;
    }

    private static void DemoInnerExceptionChain()
    {
        Console.WriteLine("-- InnerException chain --");
        try
        {
            LoadConfig();
        }
        catch (ConfigurationLoadException ex)
        {
            Debug.Assert(ex.InnerException is FileNotFoundException);
            Console.WriteLine($"  outer : {ex.GetType().Name} — {ex.Message}");
            Console.WriteLine($"  inner : {ex.InnerException!.GetType().Name} — {ex.InnerException.Message}");
        }
    }

    private static void LoadConfig()
    {
        try
        {
            throw new FileNotFoundException("config.json not found", "config.json");
        }
        catch (FileNotFoundException ex)
        {
            throw new ConfigurationLoadException("failed to load configuration", ex);
        }
    }

    private static void DemoCatchByBaseType()
    {
        Console.WriteLine("-- catch base type catches derived --");
        string? kind = null;
        try
        {
            throw new ArgumentOutOfRangeException(nameof(kind), 99, "out of range");
        }
        catch (ArgumentException ex)
        {
            kind = ex.GetType().Name;
            Debug.Assert(ex is ArgumentOutOfRangeException);
        }

        Debug.Assert(kind == nameof(ArgumentOutOfRangeException));
        Console.WriteLine($"  catch (ArgumentException) got {kind}");
    }

    private sealed class ConfigurationLoadException : Exception
    {
        public ConfigurationLoadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
