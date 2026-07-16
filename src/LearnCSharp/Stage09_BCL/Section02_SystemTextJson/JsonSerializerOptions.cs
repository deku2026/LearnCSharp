// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第2部分-System.Text.Json.md
// Stage    : Stage09_BCL
// Section  : Section02_SystemTextJson
// Item     : JsonSerializerOptions
// Topic id : stage09/section02/json_serializer_options
//
// 步骤 2：命名策略/缩进/忽略 null/枚举字符串；复用 options 实例

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section02;

internal static class JsonSerializerOptionsDemo
{
    private enum Sky { Clear, Cloudy, Rain }

    private sealed class Sample
    {
        public string Name { get; set; } = "";
        public string? Nickname { get; set; }
        public Sky Weather { get; set; }
    }

    private static readonly JsonSerializerOptions s_options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    [LearnTopic("stage09/section02/json_serializer_options")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== JsonSerializerOptions ===");
        DemoCommonOptions();
        DemoReuseInstance();
        DemoCaseInsensitive();
        return 0;
    }

    private static void DemoCommonOptions()
    {
        Console.WriteLine("-- CamelCase + ignore null + enum as string --");
        var s = new Sample { Name = "Ada", Nickname = null, Weather = Sky.Clear };
        string json = JsonSerializer.Serialize(s, s_options);
        Debug.Assert(json.Contains("\"name\"", StringComparison.Ordinal));
        Debug.Assert(!json.Contains("nickname", StringComparison.OrdinalIgnoreCase));
        Debug.Assert(json.Contains("Clear", StringComparison.Ordinal));
        Console.WriteLine(json);
    }

    private static void DemoReuseInstance()
    {
        Console.WriteLine("-- reuse static options (metadata cache) --");
        // first use builds cache; subsequent uses share it
        string a = JsonSerializer.Serialize(new Sample { Name = "A", Weather = Sky.Rain }, s_options);
        string b = JsonSerializer.Serialize(new Sample { Name = "B", Weather = Sky.Cloudy }, s_options);
        Debug.Assert(a.Contains("name", StringComparison.Ordinal) && b.Contains("name", StringComparison.Ordinal));
        Console.WriteLine("  static readonly JsonSerializerOptions → thread-safe after first use");
        // JsonSerializerOptions.Default for defaults without new()
        string d = JsonSerializer.Serialize(new Sample { Name = "D", Weather = Sky.Clear }, JsonSerializerOptions.Default);
        Debug.Assert(d.Contains("Name", StringComparison.Ordinal)); // PascalCase default
        Console.WriteLine($"  Default options keep PascalCase: has Name={d.Contains("Name", StringComparison.Ordinal)}");
    }

    private static void DemoCaseInsensitive()
    {
        Console.WriteLine("-- PropertyNameCaseInsensitive on deserialize --");
        const string json = """{"name":"Lin","weather":"Rain"}""";
        Sample? back = JsonSerializer.Deserialize<Sample>(json, s_options);
        Debug.Assert(back is { Name: "Lin", Weather: Sky.Rain });
        Console.WriteLine($"  deserialized Name={back.Name}, Weather={back.Weather}");
    }
}
