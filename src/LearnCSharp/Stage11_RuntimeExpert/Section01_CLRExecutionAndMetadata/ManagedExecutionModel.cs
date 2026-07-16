// LearnCSharp example (filled)
// Doc      : CSharp-阶段11-运行时专家-第1部分-CLR执行模型与元数据.md
// Stage    : Stage11_RuntimeExpert
// Section  : Section01_CLRExecutionAndMetadata
// Item     : ManagedExecutionModel
// Topic id : stage11/section01/managed_execution_model
//
// Lesson: managed execution model — C# → IL → JIT → native; CLR vs native process.

using System.Diagnostics;
using System.Runtime.InteropServices;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage11.Section01;

internal static class ManagedExecutionModel
{
    [LearnTopic("stage11/section01/managed_execution_model")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ManagedExecutionModel ===");
        DemoPipeline();
        DemoRuntimeIdentity();
        DemoTypeSafetyBoundary();
        return 0;
    }

    private static void DemoPipeline()
    {
        Console.WriteLine("-- pipeline: source → IL → JIT → machine code --");
        Console.WriteLine("  1) C# compiler (Roslyn) emits IL + metadata into assembly");
        Console.WriteLine("  2) CLR loads assembly, verifies IL (type safety)");
        Console.WriteLine("  3) JIT (or R2R/AOT) turns IL into native for this CPU");
        Console.WriteLine("  4) GC / EH / security services run under the managed model");
        int x = Add(20, 22);
        Debug.Assert(x == 42);
        Console.WriteLine($"  sample managed call Add(20,22)={x}");
    }

    private static void DemoRuntimeIdentity()
    {
        Console.WriteLine("-- process is still a native OS process --");
        Console.WriteLine($"  FrameworkDescription={RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"  OS={RuntimeInformation.OSDescription}");
        Console.WriteLine($"  ProcessArchitecture={RuntimeInformation.ProcessArchitecture}");
        Console.WriteLine($"  Is64BitProcess={Environment.Is64BitProcess}");
        Debug.Assert(RuntimeInformation.FrameworkDescription.Contains(".NET", StringComparison.Ordinal));
    }

    private static void DemoTypeSafetyBoundary()
    {
        Console.WriteLine("-- managed vs unmanaged boundary --");
        object boxed = 42;
        Debug.Assert(boxed is int);
        // InvalidCast would throw — type safety enforced at runtime for cast
        try
        {
            _ = (string)boxed;
            Debug.Fail("expected InvalidCastException");
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"  cast int-box → string throws: {ex.GetType().Name}");
        }

        Console.WriteLine("  unmanaged code can break the model (P/Invoke, unsafe);");
        Console.WriteLine("  pure managed code stays verifiable and GC-tracked.");
    }

    private static int Add(int a, int b) => a + b;
}
