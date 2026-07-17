// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : SourceGeneratorCaseStudies
// Topic id : stage13/section02/source_generator_case_studies
//
// Lesson: STJ / LoggerMessage / GeneratedRegex / LibraryImport — attribute → generated code.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section02;

internal static partial class SourceGeneratorCaseStudies
{
    [LearnTopic("stage13/section02/source_generator_case_studies")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SourceGeneratorCaseStudies ===");
        DemoStjRoundTripAndAlloc();
        DemoLoggerMessageConcept();
        DemoGeneratedRegex();
        DemoLibraryImport();
        return 0;
    }

    private static void DemoStjRoundTripAndAlloc()
    {
        Console.WriteLine("-- System.Text.Json source gen (round-trip + alloc sample) --");
        PersonDto person = new PersonDto { Name = "Ada", Age = 36 };
        long before = GC.GetAllocatedBytesForCurrentThread();
        string json = JsonSerializer.Serialize(person, Stage13PersonJsonContext.Default.PersonDto);
        PersonDto? back = JsonSerializer.Deserialize(json, Stage13PersonJsonContext.Default.PersonDto);
        long after = GC.GetAllocatedBytesForCurrentThread();
        Debug.Assert(back is { Name: "Ada", Age: 36 });
        Console.WriteLine($"  {json}");
        Console.WriteLine($"  round-trip Δalloc={after - before} (source-gen path; still may alloc strings)");
        Console.WriteLine("  [JsonSerializable] + JsonSerializerContext → compile-time metadata.");
    }

    private static void DemoLoggerMessageConcept()
    {
        Console.WriteLine("-- LoggerMessage (concept; needs MEL package for real generator) --");
        Console.WriteLine("  [LoggerMessage(...)] partial void LogHello(ILogger logger, string name);");
        string msg = LogHelloStandIn("Ada");
        Debug.Assert(msg.Contains("Ada", StringComparison.Ordinal));
    }

    private static string LogHelloStandIn(string name)
    {
        string msg = $"[Info] Hello {name}  (simulated LoggerMessage body)";
        Console.WriteLine($"  {msg}");
        return msg;
    }

    private static void DemoGeneratedRegex()
    {
        Console.WriteLine("-- GeneratedRegex (real source generator) --");
        Match m = DigitsOnly().Match("id=42");
        Debug.Assert(m.Success && m.Value == "42");
        // Multi match
        int count = 0;
        foreach (Match x in DigitsOnly().Matches("a1 b22 c333"))
            count++;
        Debug.Assert(count == 3);
        Console.WriteLine($"  DigitsOnly: first={m.Value}, matches in sample={count}");
    }

    private static void DemoLibraryImport()
    {
        Console.WriteLine("-- LibraryImport (P/Invoke source gen) --");
        try
        {
            if (OperatingSystem.IsWindows())
            {
                uint pid = WinGetCurrentProcessId();
                Debug.Assert(pid != 0);
                Console.WriteLine($"  kernel32 GetCurrentProcessId => {pid}");
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                int pid = UnixGetPid();
                Debug.Assert(pid > 0);
                Console.WriteLine($"  libc getpid => {pid}");
            }
            else
            {
                Console.WriteLine("  (platform not demoed)");
            }
        }
        catch (DllNotFoundException ex)
        {
            Console.WriteLine($"  DllNotFound: {ex.Message}");
        }
        catch (EntryPointNotFoundException ex)
        {
            Console.WriteLine($"  EntryPointNotFound: {ex.Message}");
        }
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitsOnly();

    [LibraryImport("kernel32.dll", EntryPoint = "GetCurrentProcessId")]
    private static partial uint WinGetCurrentProcessId();

    [LibraryImport("libc", EntryPoint = "getpid")]
    private static partial int UnixGetPid();

    private sealed class PersonDto
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [JsonSerializable(typeof(PersonDto))]
    private partial class Stage13PersonJsonContext : JsonSerializerContext;
}
