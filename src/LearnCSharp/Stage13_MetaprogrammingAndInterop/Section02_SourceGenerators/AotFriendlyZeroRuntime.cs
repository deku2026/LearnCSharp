// LearnCSharp example (filled)
// Doc      : CSharp-阶段13-元编程与互操作-第2部分-源生成器.md
// Stage    : Stage13_MetaprogrammingAndInterop
// Section  : Section02_SourceGenerators
// Item     : AotFriendlyZeroRuntime
// Topic id : stage13/section02/aot_friendly_zero_runtime
//
// Lesson: generated static code = zero runtime reflection + trimmer/AOT can see it.

using System.Diagnostics;
using System.Reflection.Emit;
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
        DemoStaticVsReflectionTiming();
        DemoBuiltInJsonSourceGen();
        DemoRuntimeEmitContrast();
        return 0;
    }

    private static void DemoStaticVsReflectionTiming()
    {
        Console.WriteLine("-- static path vs reflection path (measured) --");
        var p = new Point(1, 2);
        // Warm
        _ = StaticSerialize(p);
        _ = ReflectSerialize(p);

        const int N = 20_000;
        long t0 = Stopwatch.GetTimestamp();
        string lastStatic = "";
        for (int i = 0; i < N; i++)
            lastStatic = StaticSerialize(p);
        double staticMs = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;

        t0 = Stopwatch.GetTimestamp();
        string lastReflect = "";
        for (int i = 0; i < N; i++)
            lastReflect = ReflectSerialize(p);
        double reflectMs = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;

        Debug.Assert(lastStatic == lastReflect);
        Console.WriteLine($"  static  {N}×: {staticMs:F3}ms → {lastStatic}");
        Console.WriteLine($"  reflect {N}×: {reflectMs:F3}ms → {lastReflect}");
        Debug.Assert(staticMs >= 0 && reflectMs >= 0);
        Console.WriteLine("  Generators emit the static path at compile time.");
    }

    private static void DemoBuiltInJsonSourceGen()
    {
        Console.WriteLine("-- STJ source generation (real) --");
        var dto = new WeatherForecast { TemperatureC = 22, Summary = "Mild" };
        string json = JsonSerializer.Serialize(dto, Stage13WeatherJsonContext.Default.WeatherForecast);
        WeatherForecast? back = JsonSerializer.Deserialize(json, Stage13WeatherJsonContext.Default.WeatherForecast);
        Debug.Assert(back is { TemperatureC: 22, Summary: "Mild" });
        Console.WriteLine($"  {json}");
        Console.WriteLine("  Compile-time metadata → AOT-friendly; no runtime type graph walk.");
    }

    private static void DemoRuntimeEmitContrast()
    {
        Console.WriteLine("-- runtime emit exists but is not AOT-zero-cost --");
        var dm = new DynamicMethod("Id", typeof(int), [typeof(int)], typeof(AotFriendlyZeroRuntime).Module, true);
        ILGenerator il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ret);
        Func<int, int> id = dm.CreateDelegate<Func<int, int>>();
        Debug.Assert(id(9) == 9);
        Console.WriteLine($"  DynamicMethod Id(9)={id(9)} (requires IsDynamicCodeCompiled)");
        Console.WriteLine($"  RuntimeFeature.IsDynamicCodeCompiled={System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled}");
    }

    private static string StaticSerialize(Point p) => $"{{\"X\":{p.X},\"Y\":{p.Y}}}";

    private static string ReflectSerialize(Point p)
    {
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
