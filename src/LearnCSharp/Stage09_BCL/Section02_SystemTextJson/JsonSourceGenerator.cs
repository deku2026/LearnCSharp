// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第2部分-System.Text.Json.md
// Stage    : Stage09_BCL
// Section  : Section02_SystemTextJson
// Item     : JsonSourceGenerator
// Topic id : stage09/section02/json_source_generator
//
// 步骤 4：JsonSerializerContext 源生成 — AOT 友好 + 无反射

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section02;

internal static partial class JsonSourceGenerator
{
    internal sealed class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
    }

    [JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(Product))]
    [JsonSerializable(typeof(Product[]))]
    private partial class AppJsonContext : JsonSerializerContext;

    [LearnTopic("stage09/section02/json_source_generator")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== JsonSourceGenerator ===");
        DemoSourceGenSerialize();
        DemoViaTypeInfoResolver();
        return 0;
    }

    private static void DemoSourceGenSerialize()
    {
        Console.WriteLine("-- Serialize with Context.Default.Product --");
        var product = new Product { Id = 1, Name = "Widget", Price = 9.99m };
        string json = JsonSerializer.Serialize(product, AppJsonContext.Default.Product);
        Debug.Assert(json.Contains("\"id\"", StringComparison.Ordinal));
        Debug.Assert(json.Contains("widget", StringComparison.OrdinalIgnoreCase)
                     || json.Contains("Widget", StringComparison.Ordinal));
        Product? back = JsonSerializer.Deserialize(json, AppJsonContext.Default.Product);
        Debug.Assert(back is { Id: 1, Name: "Widget" });
        Console.WriteLine($"  round-trip Id={back.Id}, Name={back.Name}");
    }

    private static void DemoViaTypeInfoResolver()
    {
        Console.WriteLine("-- options.TypeInfoResolver = Context.Default --");
        var options = new JsonSerializerOptions { TypeInfoResolver = AppJsonContext.Default };
        Product[] list = [new() { Id = 2, Name = "Gadget", Price = 1m }];
        string json = JsonSerializer.Serialize(list, options);
        Product[]? back = JsonSerializer.Deserialize<Product[]>(json, options);
        Debug.Assert(back is [{ Id: 2 }]);
        Console.WriteLine($"  array length={back.Length}; source-gen avoids runtime reflection");
    }
}
