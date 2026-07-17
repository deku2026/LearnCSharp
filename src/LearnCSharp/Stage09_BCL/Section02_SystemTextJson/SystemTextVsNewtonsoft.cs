// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第2部分-System.Text.Json.md
// Stage    : Stage09_BCL
// Section  : Section02_SystemTextJson
// Item     : SystemTextVsNewtonsoft
// Topic id : stage09/section02/system_text_vs_newtonsoft
//
// 步骤 6：STJ vs Newtonsoft 差异要点 + C++ 无标准 JSON 库（教育对比，不引 Newtonsoft 包）

using System.Diagnostics;
using System.Text.Json;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section02;

internal static class SystemTextVsNewtonsoft
{
    private sealed class Payload
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }

    [LearnTopic("stage09/section02/system_text_vs_newtonsoft")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SystemTextVsNewtonsoft ===");
        DemoStrictByDefault();
        DemoCaseSensitivityDifference();
        DemoModernDefaults();
        return 0;
    }

    private static void DemoStrictByDefault()
    {
        Console.WriteLine("-- STJ is stricter by default than classic Newtonsoft --");
        // trailing commas / comments rejected unless enabled
        const string withComment = """{"Name":"x" /* c */, "Count":1}""";
        bool threw = false;
        try
        {
            _ = JsonSerializer.Deserialize<Payload>(withComment);
        }
        catch (JsonException)
        {
            threw = true;
        }
        Debug.Assert(threw);
        JsonSerializerOptions loose = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
        Payload? ok = JsonSerializer.Deserialize<Payload>("""{"Name":"x", "Count":1,}""", loose);
        Debug.Assert(ok is { Name: "x", Count: 1 });
        Console.WriteLine("  comments/trailing commas: off by default; opt-in via options");
    }

    private static void DemoCaseSensitivityDifference()
    {
        Console.WriteLine("-- property names case-sensitive by default (Newtonsoft was not) --");
        const string camel = """{"name":"Ada","count":3}""";
        Payload? strict = JsonSerializer.Deserialize<Payload>(camel);
        Debug.Assert(strict is { Name: "", Count: 0 } or { Name: null }); // no match without options
        JsonSerializerOptions opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        Payload? loose = JsonSerializer.Deserialize<Payload>(camel, opts);
        Debug.Assert(loose is { Name: "Ada", Count: 3 });
        Console.WriteLine($"  insensitive → Name={loose.Name}");
    }

    private static void DemoModernDefaults()
    {
        Console.WriteLine("-- STJ: UTF-8 native, AOT source-gen, no Newtonsoft package needed --");
        string json = JsonSerializer.Serialize(new Payload { Name = "BCL", Count = 1 });
        Debug.Assert(json.Contains("BCL", StringComparison.Ordinal));
        Console.WriteLine("  Prefer System.Text.Json for new code; Newtonsoft only for legacy quirks");
        Console.WriteLine("  C++ has no std JSON — nlohmann/RapidJSON/simdjson; no free reflection serialize");
    }
}
