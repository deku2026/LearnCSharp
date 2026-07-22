// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第2部分-System.Text.Json.md
// Stage    : Stage09_BCL
// Section  : Section02_SystemTextJson
// Item     : JsonSerializerBasics
// Topic id : stage09/section02/json_serializer_basics
//
// 步骤 1：Serialize/Deserialize、异步 Stream、DeserializeAsyncEnumerable

using System.Diagnostics;
using System.Text.Json;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section02;

internal static class JsonSerializerBasics
{
    private sealed class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
    }

    [LearnTopic("stage09/section02/json_serializer_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== JsonSerializerBasics ===");
        DemoSerializeDeserialize();
        DemoAsyncStream().GetAwaiter().GetResult();
        DemoAsyncEnumerable().GetAwaiter().GetResult();
        return 0;
    }

    private static void DemoSerializeDeserialize()
    {
        Console.WriteLine("-- Serialize / Deserialize --");
        WeatherForecast forecast = new WeatherForecast
        {
            Date = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc),
            TemperatureC = 25,
            Summary = "Sunny"
        };
        string json = JsonSerializer.Serialize(forecast);
        Debug.Assert(json.Contains("TemperatureC", StringComparison.Ordinal));
        WeatherForecast? back = JsonSerializer.Deserialize<WeatherForecast>(json);
        Debug.Assert(back is { TemperatureC: 25, Summary: "Sunny" });
        Console.WriteLine($"  json length={json.Length}; round-trip Temp={back.TemperatureC}");
    }

    private static async Task DemoAsyncStream()
    {
        Console.WriteLine("-- SerializeAsync / DeserializeAsync on temp file --");
        string path = Path.Join(Path.GetTempPath(), $"learn-stj-{Guid.NewGuid():N}.json");
        try
        {
            WeatherForecast forecast = new WeatherForecast { Date = DateTime.UtcNow, TemperatureC = 18, Summary = "Cool" };
            await using (FileStream fs = File.Create(path))
                await JsonSerializer.SerializeAsync(fs, forecast);

            await using FileStream rs = File.OpenRead(path);
            WeatherForecast? loaded = await JsonSerializer.DeserializeAsync<WeatherForecast>(rs);
            Debug.Assert(loaded is { TemperatureC: 18 });
            Console.WriteLine($"  loaded Summary={loaded.Summary}");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    private static async Task DemoAsyncEnumerable()
    {
        Console.WriteLine("-- DeserializeAsyncEnumerable for JSON array --");
        string path = Path.Join(Path.GetTempPath(), $"learn-stj-arr-{Guid.NewGuid():N}.json");
        try
        {
            WeatherForecast[] items =
            [
                new() { Date = DateTime.UtcNow, TemperatureC = 1, Summary = "A" },
                new() { Date = DateTime.UtcNow, TemperatureC = 2, Summary = "B" }
            ];
            await using (FileStream ws = File.Create(path))
                await JsonSerializer.SerializeAsync(ws, items);

            await using FileStream rs = File.OpenRead(path);
            List<int> list = new List<int>();
            await foreach (WeatherForecast? item in JsonSerializer.DeserializeAsyncEnumerable<WeatherForecast>(rs))
            {
                if (item is not null)
                    list.Add(item.TemperatureC);
            }
            Debug.Assert(list is [1, 2]);
            Console.WriteLine($"  streamed temps: [{string.Join(", ", list)}]");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
