// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : AotFriendlyZeroRuntime
// Topic id : stage13/section02/aot_friendly_zero_runtime
//
// Lesson: generated static code = zero runtime reflection + trimmer/AOT can see it.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage13.Section02;

internal static partial class AotFriendlyZeroRuntime
{
    [LearnTopic("stage13/section02/aot_friendly_zero_runtime")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AotFriendlyZeroRuntime ===");
        DemoZeroRuntimeCost();
        DemoAotVisibility();
        DemoBuiltInJsonSourceGen();
        return 0;
    }

    private static void DemoZeroRuntimeCost()
    {
        Console.WriteLine("-- zero runtime cost --");
        Console.WriteLine("  Generated C# is ordinary IL: no GetMethod, no Invoke, no discovery.");
        Console.WriteLine("  Startup: no reflection warm-up; hot path: direct calls, often zero alloc.");

        // Hand-written static path vs reflection path for the same shape
        var p = new Point(1, 2);
        string staticJson = StaticSerialize(p);
        string reflectJson = ReflectSerialize(p);
        Debug.Assert(staticJson == reflectJson);
        Console.WriteLine($"  static serialize: {staticJson}");
        Console.WriteLine($"  reflect serialize: {reflectJson} (same result; reflect pays discovery/Invoke)");
    }

    private static void DemoAotVisibility()
    {
        Console.WriteLine("-- AOT / trimmer visibility --");
        Console.WriteLine("  Static generated methods are call-graph visible → not trimmed away.");
        Console.WriteLine("  No DynamicallyAccessedMembers needed for pure generated paths.");
        Console.WriteLine("  Native AOT: no runtime IL stubs / no dynamic code gen required.");
    }

    private static void DemoBuiltInJsonSourceGen()
    {
        Console.WriteLine("-- built-in STJ source generation (real, in-process) --");
        var dto = new WeatherForecast { TemperatureC = 22, Summary = "Mild" };
        string json = JsonSerializer.Serialize(dto, Stage13WeatherJsonContext.Default.WeatherForecast);
        WeatherForecast? back = JsonSerializer.Deserialize(json, Stage13WeatherJsonContext.Default.WeatherForecast);
        Debug.Assert(back is { TemperatureC: 22, Summary: "Mild" });
        Console.WriteLine($"  JsonSerializerContext path: {json}");
        Console.WriteLine("  Metadata emitted at compile time → AOT-friendly serialization.");
    }

    private static string StaticSerialize(Point p) => $"{{\"X\":{p.X},\"Y\":{p.Y}}}";

    private static string ReflectSerialize(Point p)
    {
        // educational contrast only
        System.Reflection.PropertyInfo x = typeof(Point).GetProperty(nameof(Point.X))!;
        System.Reflection.PropertyInfo y = typeof(Point).GetProperty(nameof(Point.Y))!;
        return $"{{\"X\":{x.GetValue(p)},\"Y\":{y.GetValue(p)}}}";
    }

    private readonly record struct Point(int X, int Y);

    private sealed class WeatherForecast
    {
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
    }

    [JsonSerializable(typeof(WeatherForecast))]
    private partial class Stage13WeatherJsonContext : JsonSerializerContext;
}
