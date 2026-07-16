// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第2部分-System.Text.Json.md
// Stage    : Stage09_BCL
// Section  : Section02_SystemTextJson
// Item     : JsonDomAndCustomConverters
// Topic id : stage09/section02/json_dom_and_custom_converters
//
// 步骤 5：JsonNode 可变 DOM / JsonDocument 只读；JsonConverter<T>

using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section02;

internal static class JsonDomAndCustomConverters
{
    private sealed class EventDto
    {
        public DateTime When { get; set; }
        public string Title { get; set; } = "";
    }

    private sealed class CompactDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            return DateTime.ParseExact(s!, Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }

    [LearnTopic("stage09/section02/json_dom_and_custom_converters")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== JsonDomAndCustomConverters ===");
        DemoJsonNode();
        DemoJsonDocument();
        DemoCustomConverter();
        return 0;
    }

    private static void DemoJsonNode()
    {
        Console.WriteLine("-- JsonNode mutable DOM --");
        JsonNode root = JsonNode.Parse("""{"items":[{"name":"alpha"},{"name":"beta"}]}""")!;
        string first = root["items"]![0]!["name"]!.GetValue<string>();
        Debug.Assert(first == "alpha");
        root["items"]![0]!["name"] = "ALPHA";
        string modified = root.ToJsonString();
        Debug.Assert(modified.Contains("ALPHA", StringComparison.Ordinal));
        var obj = new JsonObject
        {
            ["name"] = "Ada",
            ["scores"] = new JsonArray(90, 85, 95)
        };
        Debug.Assert(obj["scores"]!.AsArray().Count == 3);
        Console.WriteLine($"  first→ALPHA; built name={obj["name"]}");
    }

    private static void DemoJsonDocument()
    {
        Console.WriteLine("-- JsonDocument/JsonElement (IDisposable, pooled) --");
        using JsonDocument doc = JsonDocument.Parse("""{"name":"Ada","age":30}""");
        JsonElement root = doc.RootElement;
        string name = root.GetProperty("name").GetString()!;
        int age = root.GetProperty("age").GetInt32();
        Debug.Assert(name == "Ada" && age == 30);
        JsonElement cloned = root.GetProperty("name").Clone();
        Debug.Assert(cloned.GetString() == "Ada");
        Console.WriteLine($"  name={name}, age={age}; Clone keeps value after Dispose of other docs");
    }

    private static void DemoCustomConverter()
    {
        Console.WriteLine("-- JsonConverter<DateTime> compact format --");
        var options = new JsonSerializerOptions();
        options.Converters.Add(new CompactDateTimeConverter());
        var dto = new EventDto
        {
            When = new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            Title = "Demo"
        };
        string json = JsonSerializer.Serialize(dto, options);
        Debug.Assert(json.Contains("2026-07-16 10:00:00", StringComparison.Ordinal));
        EventDto? back = JsonSerializer.Deserialize<EventDto>(json, options);
        Debug.Assert(back is { Title: "Demo" });
        Console.WriteLine($"  json={json}");
    }
}
