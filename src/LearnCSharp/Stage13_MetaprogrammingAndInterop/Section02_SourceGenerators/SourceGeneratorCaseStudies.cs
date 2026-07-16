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
        DemoStj();
        DemoLoggerMessageConcept();
        DemoGeneratedRegex();
        DemoLibraryImport();
        return 0;
    }

    private static void DemoStj()
    {
        Console.WriteLine("-- System.Text.Json source gen --");
        var person = new PersonDto { Name = "Ada", Age = 36 };
        string json = JsonSerializer.Serialize(person, Stage13PersonJsonContext.Default.PersonDto);
        PersonDto? back = JsonSerializer.Deserialize(json, Stage13PersonJsonContext.Default.PersonDto);
        Debug.Assert(back is { Name: "Ada", Age: 36 });
        Console.WriteLine($"  {json}");
        Console.WriteLine("  [JsonSerializable] + JsonSerializerContext → compile-time metadata.");
    }

    private static void DemoLoggerMessageConcept()
    {
        Console.WriteLine("-- LoggerMessage (concept; needs Microsoft.Extensions.Logging package) --");
        Console.WriteLine("  [LoggerMessage(EventId=1, Level=Information, Message=\"Hello {Name}\")]");
        Console.WriteLine("  partial void LogHello(ILogger logger, string name);");
        Console.WriteLine("  Generator emits: high-perf Write with cached EventId (no runtime template parse).");
        // Educational stand-in of generated body
        LogHelloStandIn("Ada");
    }

    private static void LogHelloStandIn(string name)
        => Console.WriteLine($"  [Info] Hello {name}  (simulated LoggerMessage body)");

    private static void DemoGeneratedRegex()
    {
        Console.WriteLine("-- GeneratedRegex --");
        Match m = DigitsOnly().Match("id=42");
        Debug.Assert(m.Success && m.Value == "42");
        Console.WriteLine($"  DigitsOnly on \"id=42\" => {m.Value}");
        Console.WriteLine("  Regex compiled into methods at build time (AOT-safe).");
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
