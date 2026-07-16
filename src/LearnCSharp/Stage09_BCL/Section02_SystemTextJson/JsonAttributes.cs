// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第2部分-System.Text.Json.md
// Stage    : Stage09_BCL
// Section  : Section02_SystemTextJson
// Item     : JsonAttributes
// Topic id : stage09/section02/json_attributes
//
// 步骤 3：[JsonPropertyName]/[JsonIgnore]/[JsonExtensionData]/[JsonConstructor]

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section02;

internal static class JsonAttributes
{
    private sealed class Person
    {
        [JsonPropertyName("full_name")]
        public string Name { get; set; } = "";

        [JsonPropertyOrder(1)]
        public int Age { get; set; }

        [JsonIgnore]
        public string Password { get; set; } = "";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Nickname { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? Extra { get; set; }

        [JsonConstructor]
        public Person() { }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    [LearnTopic("stage09/section02/json_attributes")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== JsonAttributes ===");
        DemoPropertyNameAndIgnore();
        DemoExtensionData();
        return 0;
    }

    private static void DemoPropertyNameAndIgnore()
    {
        Console.WriteLine("-- JsonPropertyName + JsonIgnore --");
        var p = new Person { Name = "Ada", Age = 36, Password = "secret", Nickname = null };
        string json = JsonSerializer.Serialize(p);
        Debug.Assert(json.Contains("full_name", StringComparison.Ordinal));
        Debug.Assert(!json.Contains("Password", StringComparison.OrdinalIgnoreCase));
        Debug.Assert(!json.Contains("Nickname", StringComparison.OrdinalIgnoreCase));
        Person? back = JsonSerializer.Deserialize<Person>(json);
        Debug.Assert(back is { Name: "Ada", Age: 36 });
        Console.WriteLine($"  json={json}");
    }

    private static void DemoExtensionData()
    {
        Console.WriteLine("-- JsonExtensionData captures unmapped members --");
        const string json = """{"full_name":"Lin","Age":30,"role":"admin","score":99}""";
        Person? p = JsonSerializer.Deserialize<Person>(json);
        Debug.Assert(p is not null && p.Extra is not null);
        Debug.Assert(p.Extra.ContainsKey("role"));
        Debug.Assert(p.Extra["score"].GetInt32() == 99);
        Console.WriteLine($"  Extra keys: {string.Join(", ", p.Extra.Keys)}");
    }
}
